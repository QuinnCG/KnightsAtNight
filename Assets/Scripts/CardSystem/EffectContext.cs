using Quinn.CardSystem.Effect;
using UnityEngine;

namespace Quinn.CardSystem
{
	public class EffectContext
	{
		public Vector2 Position;
		public Health Target;
		public Card Source;
		public SpellEffect ParentEffect;

		public EffectContext(Vector2 position, Health target, Card source, SpellEffect parent)
		{
			Position = position;
			Target = target;
			Source = source;
			ParentEffect = parent;
		}
	}
}
