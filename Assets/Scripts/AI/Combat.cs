using UnityEngine;

namespace Quinn.AI
{
	public class Combat : MonoBehaviour
	{
		[field: SerializeField]
		public float Interval { get; set; } = 0.5f;

		[field: SerializeField]
		public float Damage { get; set; } = 30;

		[SerializeField]
		private float KnockbackSpeed = 14f;

		private float _nextAttackTime;

		public void Attack(GameObject target)
		{
			if (target == gameObject) return;

			if (Time.time > _nextAttackTime && target.TryGetComponent(out Health health))
			{
				_nextAttackTime = Time.time + Interval;

				var dir = transform.position.DirectionTo(target.transform.position);
				health.TakeDamage(Damage, dir, KnockbackSpeed);
			}
		}
	}
}
