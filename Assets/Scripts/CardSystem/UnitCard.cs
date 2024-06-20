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

		// TODO: Spawn effects?

		public override void Cast()
		{
			for (int i = 0; i < Count; i++)
			{
				Prefab.Clone(Player.MousePos);
			}
		}
	}
}
