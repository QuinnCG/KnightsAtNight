using Sirenix.OdinInspector;
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
			}
		}

		public void ApplyDamage()
		{
			if (_target != null)
			{
				// Calculate damage.
				float dmg = Damage
					* (_statusManager.Has(StatusEffectType.Weakened)
					? WeakenedStatusDamageFactor
					: 1f);

				// Calculate direction of attack.
				var dir = transform.position.DirectionTo(_target.transform.position);

				// Apply damage.
				_target.TakeDamage(dmg, dir, KnockbackSpeed);

				// Reset target.
				_target = null;
			}
		}

		[Button]
		public void SetIntervalToAttackAnim()
		{
			var animator = GetComponent<Animator>();
			var clipInfos = animator.GetCurrentAnimatorClipInfo(0);

			foreach (var info in clipInfos)
			{
				if (info.clip.name == "Attack")
				{
					Interval = info.clip.length;
					return;
				}
			}
		}
	}
}
