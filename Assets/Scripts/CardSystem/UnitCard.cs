using Quinn.AI;
using Quinn.CardSystem.Effect;
using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace Quinn.CardSystem
{
	[CreateAssetMenu(menuName = "Cards/Unit Card")]
	public class UnitCard : Card
	{
		[Space, AssetsOnly]
		public GameObject Prefab;
		public int Count = 1;

		[InlineProperty]
		public SpellEffect[] OnSpawnEffects = Array.Empty<SpellEffect>();
		[InlineProperty]
		public SpellEffect[] OnKillEffects = Array.Empty<SpellEffect>();
		[InlineProperty]
		public SpellEffect[] OnDeathEffects = Array.Empty<SpellEffect>();

		public override void Cast()
		{
			for (int i = 0; i < Count; i++)
			{
				Vector2 pos = Player.MousePos;
				GameObject unit = Prefab.Clone(pos);

				InitializeEffects(unit, pos);
			}
		}

		private void InitializeEffects(GameObject unit, Vector2 pos)
		{
			var health = unit.GetComponent<Health>();
			var combat = unit.GetComponent<Combat>();

			// Triggered when this unit is spawned.
			if (OnSpawnEffects != null)
			{
				foreach (var effect in OnSpawnEffects)
				{
					effect.Activate(new(pos, health, this, null));
				}
			}

			// Triggered when something is killed by this unit.
			if (OnKillEffects != null)
			{
				foreach (var effect in OnKillEffects)
				{
					combat.OnKill += _ =>
					{
						effect.Activate(new(pos, health, this, null));
					};
				}
			}

			// Triggered when this unit dies.
			if (OnDeathEffects != null)
			{
				foreach (var effect in OnDeathEffects)
				{
					health.OnDeath += () =>
					{
						effect.Activate(new(pos, health, this, null));
					};
				}
			}
		}
	}
}
