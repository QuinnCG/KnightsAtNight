using Sirenix.OdinInspector;

namespace Quinn.CardSystem.Effect
{
	public class RepeatEffect : SpellEffect
	{
		[InlineProperty]
		public SpellEffect Effect;
		public int Count = 2;

		protected override void OnActivate(EffectContext context)
		{
			for (int i = 0; i < Count; i++)
			{
				Effect.Activate(context);
			}
		}
	}
}
