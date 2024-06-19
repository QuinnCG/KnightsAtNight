using UnityEngine;

namespace Quinn
{
	public static class VectorExtensions
	{
		public static Vector2 DirectionTo(this Vector2 origin, Vector2 target)
		{
			return (target - origin).normalized;
		}
		public static Vector3 DirectionTo(this Vector3 origin, Vector3 target)
		{
			return (target - origin).normalized;
		}

		public static float DistanceTo(this Vector2 origin, Vector2 target)
		{
			return Vector2.Distance(origin, target);
		}
		public static float DistanceTo(this Vector3 origin, Vector3 target)
		{
			return Vector3.Distance(origin, target);
		}

		public static float GetRandom(this Vector2 source)
		{
			return Random.Range(source.x, source.y);
		}
		/// <param name="exclusive">True to reduce the max by one.</param>
		public static int GetRandom(this Vector2Int source, bool exclusive = true)
		{
			return Random.Range(source.x, source.y + (exclusive ? 0 : 1));
		}

		/// <summary>
		/// Returns a copy of the same vector but formed into an exactly: up, down, left, or right direction.
		/// </summary>
		public static Vector2 ToCardinal(this Vector2 v)
		{
			if (Mathf.Abs(v.x) > Mathf.Abs(v.y))
			{
				return Vector2.right * Mathf.Sign(v.x);
			}
			else
			{
				return Vector2.up * Mathf.Sign(v.y);
			}
		}

		/// <summary>
		/// Returns a copy of the same vector but formed into an exactly: up, down, left, or right direction.
		/// </summary>
		public static Vector2Int ToCardinal(this Vector2Int v)
		{
			if (Mathf.Abs(v.x) > Mathf.Abs(v.y))
			{
				return new((int)Mathf.Sign(v.x), 0);
			}
			else
			{
				return new(0, (int)Mathf.Sign(v.y));
			}
		}

		public static Vector3Int ToVector3Int(this Vector2 v)
		{
			return new(Mathf.RoundToInt(v.x), Mathf.RoundToInt(v.y));
		}
		public static Vector3Int ToVector3Int(this Vector3 v)
		{
			return new(Mathf.RoundToInt(v.x), Mathf.RoundToInt(v.y), Mathf.RoundToInt(v.z));
		}
	}
}
