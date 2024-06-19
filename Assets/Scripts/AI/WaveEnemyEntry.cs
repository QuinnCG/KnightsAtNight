using Sirenix.OdinInspector;
using UnityEngine;

namespace Quinn.AI
{
	[System.Serializable]
	public class WaveEnemyEntry
	{
		[HorizontalGroup, HideLabel, AssetsOnly, AssetSelector(Paths = "Assets/Prefabs/Invaders")]
		public GameObject Prefab;

		[HorizontalGroup, HideLabel]
		public Vector2Int Count = new(2, 4);
	}
}
