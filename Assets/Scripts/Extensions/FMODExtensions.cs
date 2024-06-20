using FMODUnity;
using UnityEngine;

namespace Quinn
{
	public static class FMODExtensions
	{
		public static void PlayOnce(this EventReference sound, Vector2 pos = default)
		{
			if (!sound.IsNull)
			{
				RuntimeManager.PlayOneShot(sound, pos);
			}
		}
		public static void PlayOnce(this EventReference sound, Transform parent)
		{
			if (!sound.IsNull)
			{
				RuntimeManager.PlayOneShotAttached(sound, parent.gameObject);
			}
		}
	}
}
