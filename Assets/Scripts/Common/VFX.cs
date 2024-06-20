using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.VFX;

namespace Quinn
{
	[System.Serializable]
	public struct VFX
	{
		[HorizontalGroup, HideLabel]
		public VisualEffectAsset Asset;
		[HorizontalGroup, HideLabel, Unit(Units.Second)]
		public float Lifespan;

		public readonly bool IsValid => Asset != null;

		public readonly void Spawn(Vector2 pos)
		{
			Asset.Clone(pos, lifespan: Lifespan);
		}
	}
}
