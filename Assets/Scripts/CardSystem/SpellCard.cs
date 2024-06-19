using Quinn.CardSystem.Effect;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Quinn.CardSystem
{
	[CreateAssetMenu(menuName = "Cards/Spell Card")]
	public class SpellCard : Card
	{
		[InlineProperty]
		public SpellEffect[] OnCast;

		public override void Cast()
		{
			//Tower.Instance.CastSpell();
			// TODO: Uncommenting causes issues when dragging cards.

			foreach (var effect in OnCast)
			{
				effect.Activate(Player.MousePos);
			}
		}
	}
}
