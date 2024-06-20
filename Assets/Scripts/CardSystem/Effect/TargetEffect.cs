using FMODUnity;
using UnityEngine;

namespace Quinn.CardSystem.Effect
{
	public class TargetEffect : SpellEffect
	{
		public const float SelectionRadius = 1f;

		public SpellEffect Effect;
		public LayerMask Mask = CollisionUtility.HostileMask;
		[Tooltip("If true, the position passed to subeffects will be this effect's spell, otherwise it will be the position of afflicted targets.")]
		public bool PositionFromOrigin = false;

		[Space]
		public VFX VFX;
		public EventReference Sound;

		protected override void OnActivate(EffectContext context)
		{
			Sound.PlayOnce(context.Position);
			if (VFX.IsValid) VFX.Spawn(context.Position);

			var targets = CollisionUtility.GetHealthInRadius(context.Position, SelectionRadius, Mask);
			foreach (var target in targets)
			{
				Vector2 pos = PositionFromOrigin ? context.Position : target.transform.position;
				Effect.Activate(new(pos, target, context.Source, this));
			}
		}
	}
}
