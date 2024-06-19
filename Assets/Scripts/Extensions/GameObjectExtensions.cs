using Quinn.Common;
using UnityEngine;

namespace Quinn
{
	public static class GameObjectExtensions
	{
		public static void Damage(this GameObject gameObject, float damage, Vector2 direction, float knockbackSpeed)
		{
			if (gameObject.TryGetComponent(out Health health))
			{
				health.TakeDamage(damage, direction, knockbackSpeed);
			}
		}

		public static Vector2 GetColliderCenter(this GameObject gameObject)
		{
			return gameObject.GetComponent<Collider2D>().bounds.center;
		}
	}
}
