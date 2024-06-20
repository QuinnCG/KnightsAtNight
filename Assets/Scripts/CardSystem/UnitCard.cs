using Quinn.CardSystem.Effect;
using Sirenix.OdinInspector;
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
		public SpellEffect[] SpawnEffects;

		public override void Cast()
		{
			for (int i = 0; i < Count; i++)
			{
				Vector2 pos = Player.MousePos;
				var health = Prefab.Clone<Health>(pos);

				if (SpawnEffects != null)
				{
					foreach (var effect in SpawnEffects)
					{
						effect.Activate(new(pos, health, this, null));
					}
				}
			}
		}
	}
}
