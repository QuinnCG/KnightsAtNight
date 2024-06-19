using Sirenix.OdinInspector;
using UnityEngine;

namespace Quinn.CardSystem
{
	[CreateAssetMenu(menuName = "Cards/Unit Card")]
	public class UnitCard : Card
	{
		[AssetsOnly]
		public GameObject Prefab;

		// TODO: Spawn effects?

		public override void Cast()
		{
			Prefab.Clone(Player.MousePos);
		}
	}
}
