using Quinn.Common;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.VFX;

namespace Quinn.CardSystem.Effect
{
	public class AOEEffect : SpellEffect
	{
		[AssetsOnly]
		public VisualEffectAsset CenterVFX, PerTargetVFX;
		public float VFXLifespan = 3f;

		[Space]
		public LayerMask Mask = DamageUtility.HostileMask;
		public float Radius = 3f;

		[Space]
		public float Damage = 30f;
		[HideIf(nameof(Damage), 0f)]
		public float Knockback = 10f;

		[Space]
		public float Health = 0f;

		public override void Activate(Vector2 position)
		{
			if (CenterVFX != null)
			{
				CenterVFX.Clone(position, lifespan: VFXLifespan);
			}

			var targets = DamageUtility.GetHealthInRadius(position, Radius, Mask);

			if (Damage > 0f)
			{
				foreach (var target in targets)
				{
					target.TakeDamage(Damage, position.DirectionTo(target.transform.position), Knockback);

					if (PerTargetVFX != null)
					{
						PerTargetVFX.Clone(target.gameObject.GetColliderCenter(), lifespan: VFXLifespan);
					}
				}
			}

			if (Health > 0f)
			{
				foreach (var target in targets)
				{
					target.Heal(Health);

					if (PerTargetVFX != null)
					{
						PerTargetVFX.Clone(target.gameObject.GetColliderCenter(), lifespan: VFXLifespan);
					}
				}
			}
		}
	}
}
