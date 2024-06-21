using Sirenix.OdinInspector;

namespace Quinn.CardSystem.Effect
{
	public abstract class SpellEffect
	{
		[BoxGroup(Order = float.PositiveInfinity), InlineProperty, LabelWidth(80f)]
		public SpellEffect ChainEffect;

		public void Activate(EffectContext context)
		{
			OnActivate(context);
			ChainEffect?.Activate(context);
		}

		protected abstract void OnActivate(EffectContext context);
	}
}
