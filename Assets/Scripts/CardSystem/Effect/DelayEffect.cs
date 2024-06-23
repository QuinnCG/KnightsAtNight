using DG.Tweening;
using Sirenix.OdinInspector;

namespace Quinn.CardSystem.Effect
{
	public class DelayEffect : SpellEffect
	{
		[InlineProperty]
		public SpellEffect Effect;
		public float Delay = 1f;

		protected override void OnActivate(EffectContext context)
		{
			DOVirtual.DelayedCall(Delay, () => Effect.Activate(new EffectContext(context, this)), false);
		}
	}
}
