using System.Linq;
using UnityEngine;

namespace Quinn
{
	public static class CollisionUtility
	{
		public static LayerMask FriendlyMask = LayerMask.GetMask("Friendly");
		public static LayerMask HostileMask = LayerMask.GetMask("Hostile");

		public static Health[] GetHealthInRadius(Vector2 center, float radius, LayerMask mask)
		{
			var colliders = Physics2D.OverlapCircleAll(center, radius, mask);
			return colliders.Select(x => x.GetComponent<Health>()).ToArray();
		}
	}
}
