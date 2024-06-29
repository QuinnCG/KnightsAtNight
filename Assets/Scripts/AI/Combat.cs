using FMODUnity;
using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace Quinn.AI
{
	[RequireComponent(typeof(StatusEffectManager), typeof(Animator))]
	public class Combat : MonoBehaviour
	{
		[field: SerializeField, InfoBox("@\"DPS: \" + (Damage/Interval).ToString(\"0.00\")")]
		public float Interval { get; set; } = 0.5f;

		[field: SerializeField]
		public float Damage { get; set; } = 30;
		[SerializeField]
		private float WeakenedStatusDamageFactor = 0.3f;

		[SerializeField, Tooltip("The knockback speed of the target we hit when attacking.")]
		private float KnockbackSpeed = 14f;

		[SerializeField]
		private string AttackTrigger = "Attack";

		[SerializeField]
		private EventReference SwingSound;

		[SerializeField]
		private StatusEffectEntry[] ApplyStatuses;

		public event Action<Health> OnAttack;
		public event Action<Health> OnDamage;
		public event Action<Health> OnKill;

		private StatusEffectManager _statusManager;
		private Animator _animator;

		private float _nextAttackTime;
		private Health _target;

		private void Awake()
		{
			_statusManager = GetComponent<StatusEffectManager>();
			_animator = GetComponent<Animator>();
		}

		public void Attack(GameObject target)
		{
			// Should not be null.
			Debug.Assert(target != null, "Combat.Attack(GameObject) was passed a null target!");

			// Ignore self as target.
			if (target == gameObject) return;

			// Only attack if cooldown is complete, target has Health component, and target isn't dead.
			if (Time.time > _nextAttackTime &&
				target.TryGetComponent(out Health health) && 
				!health.IsDead)
			{
				_nextAttackTime = Time.time + Interval;

				_target = health;
				_animator.SetTrigger(AttackTrigger);

				OnAttack?.Invoke(health);
			}
		}

		public void ApplyDamage()
		{
			if (_target != null)
			{
				SwingSound.PlayOnce(transform.position);

				// Calculate damage.
				float dmg = Damage
					* (_statusManager.Has(StatusEffectType.Weakened)
					? WeakenedStatusDamageFactor
					: 1f);

				// Calculate direction of attack.
				var dir = transform.position.DirectionTo(_target.transform.position);

				// Status effects.
				if (_target.TryGetComponent(out StatusEffectManager statusManager))
				{
					if (ApplyStatuses != null)
					{
						foreach (var entry in ApplyStatuses)
						{
							statusManager.Apply(entry.Type, entry.Duration);
						}
					}
				}

				// Apply damage.
				_target.TakeDamage(dmg, dir, KnockbackSpeed);

				// Events.
				OnDamage?.Invoke(_target);
				if (_target.IsDead) OnKill?.Invoke(_target);
			}
		}
	}
}
