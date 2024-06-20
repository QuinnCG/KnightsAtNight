using Quinn.CardSystem.Effect;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Quinn.CardSystem
{
	[CreateAssetMenu(menuName = "Cards/Spell Card")]
	public class SpellCard : Card
	{
		[Space, InlineProperty]
		public SpellEffect[] OnCast;

		public override void Cast()
		{
			Tower.Instance.CastSpell();

			foreach (var effect in OnCast)
			{
				effect.Activate(new(Player.MousePos, null, this, null));
			}
		}
	}
}
