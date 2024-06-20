using UnityEngine;

namespace Quinn
{
	public static class DamageUtility
	{
		public static void DamageClosest(Vector2 center, float radius, LayerMask mask, float damage, Vector2 direction, float knockbackSpeed)
		{
			var colliders = Physics2D.OverlapCircleAll(center, radius, mask);
			var collider = colliders.Lowest(x => center.DistanceTo(x.transform.position));

			var health = collider.GetComponent<Health>();
			health.TakeDamage(damage, direction, knockbackSpeed);
		}

		public static void DamageAll(Vector2 center, float radius, LayerMask mask, float damage, float knockbackSpeed)
		{
			var colliders = Physics2D.OverlapCircleAll(center, radius, mask);

			foreach (var collider in colliders)
			{
				Vector2 targetCenter = collider.GetComponent<Collider2D>().bounds.center;
				Vector2 dir = center.DirectionTo(targetCenter);

				var health = collider.GetComponent<Health>();
				health.TakeDamage(damage, dir, knockbackSpeed);
			}
		}
	}
}
