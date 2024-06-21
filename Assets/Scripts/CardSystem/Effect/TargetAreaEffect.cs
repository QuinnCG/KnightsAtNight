using FMODUnity;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Quinn.CardSystem.Effect
{
	public class TargetAreaEffect : SpellEffect
	{
		[InlineProperty]
		public SpellEffect Effect;
		public float Radius = 3f;
		public LayerMask Mask = CollisionUtility.HostileMask;

		[Space]
		public VFX VFX;
		public EventReference Sound;

		protected override void OnActivate(EffectContext context)
		{
			if (VFX.IsValid) VFX.Spawn(context.Position);
			Sound.PlayOnce(context.Position);

			var targets = CollisionUtility.GetHealthInRadius(context.Position, Radius, Mask);
			foreach (var target in targets)
			{
				Effect.Activate(new(target.transform.position, target, context.Source, this));
			}
		}
	}
}
