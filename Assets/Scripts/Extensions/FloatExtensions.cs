using UnityEngine;

namespace Quinn
{
	public static class FloatExtensions
	{
		public static float Normalized(this float f)
		{
			return Mathf.Clamp01(f);
		}

		public static float Clamped(this float f, float min, float max)
		{
			return Mathf.Clamp(f, min, max);
		}

		public static float Sqrt(this float f)
		{
			return Mathf.Sqrt(f);
		}

		public static float Pow(this float f, float x)
		{
			return Mathf.Pow(f, x);
		}

		public static float Squared(this float f)
		{

			return Mathf.Pow(f, 2);
		}

		public static float Cubed(this float f)
		{

			return Mathf.Pow(f, 3);
		}
	}
}
