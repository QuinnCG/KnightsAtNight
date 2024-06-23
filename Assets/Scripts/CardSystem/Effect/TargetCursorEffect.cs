using Sirenix.OdinInspector;

namespace Quinn.CardSystem.Effect
{
	public class TargetCursorEffect : SpellEffect
	{
		[InlineProperty]
		public SpellEffect Effect;

		protected override void OnActivate(EffectContext context)
		{
			Effect.Activate(new EffectContext(Player.MousePos, context.Target, context.Source, this));
		}
	}
}
