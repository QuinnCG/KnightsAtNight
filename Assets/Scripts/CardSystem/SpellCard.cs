using FMODUnity;
using Quinn.CardSystem.Effect;
using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace Quinn.CardSystem
{
	[CreateAssetMenu(menuName = "Cards/Spell Card")]
	public class SpellCard : Card
	{
		[Space]
		public EventReference SpawnSound;
		[InlineProperty]
		public SpellEffect[] OnCast = Array.Empty<SpellEffect>();

		public override void Cast()
		{
			Vector2 pos = Player.MousePos;

			Tower.Instance.CastSpell(() =>
			{
				SpawnSound.PlayOnce(Tower.Instance.SpellSpawnPoint);

				foreach (var effect in OnCast)
				{
					effect.Activate(new(pos, null, this, null));
				}
			});
		}
	}
}
