using Quinn.Common;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.VFX;

namespace Quinn.CardSystem.Effect
{
	public class CursorSpawnEffect : SpellEffect
	{
		[AssetsOnly]
		public VisualEffectAsset VFX;
		public float VFXLifespan = float.PositiveInfinity;
		public float Damage = 50f;
		public float Radius = 2f;
		public float KnockbackSpeed = 12f;

		public override void Activate(Vector2 position)
		{
			VFX.Clone(position, lifespan: VFXLifespan);
			DamageUtility.DamageAll(position, Radius, DamageUtility.HostileMask, Damage, KnockbackSpeed);
		}
	}
}
