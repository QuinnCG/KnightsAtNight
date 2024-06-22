using UnityEngine;

namespace Quinn.AI
{
	[RequireComponent(typeof(StatusEffectManager))]
	public class Combat : MonoBehaviour
	{
		[field: SerializeField]
		public float Interval { get; set; } = 0.5f;

		[field: SerializeField]
		public float Damage { get; set; } = 30;
		[SerializeField]
		private float WeakenedStatusDamageFactor = 0.3f;

		[SerializeField]
		private float KnockbackSpeed = 14f;

		private StatusEffectManager _statusManager;
		private float _nextAttackTime;

		private void Awake()
		{
			_statusManager = GetComponent<StatusEffectManager>();
		}

		public void Attack(GameObject target)
		{
			if (target == gameObject) return;

			if (Time.time > _nextAttackTime && target.TryGetComponent(out Health health))
			{
				_nextAttackTime = Time.time + Interval;

				float dmg = Damage
					* (_statusManager.Has(StatusEffectType.Weakened)
					? WeakenedStatusDamageFactor
					: 1f);

				var dir = transform.position.DirectionTo(target.transform.position);
				health.TakeDamage(Damage, dir, KnockbackSpeed);
			}
		}
	}
}
