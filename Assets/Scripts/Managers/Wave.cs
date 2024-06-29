using Sirenix.OdinInspector;
using UnityEngine;

namespace Quinn
{
	[System.Serializable]
	public class Wave
	{
		[SerializeField]
		public string Title = "Wave Title";

		[Tooltip("Before this wave count, this wave will not be spawned.")]
		public int DebutWave = 1;

		// Spawn enemies in chunks of a size randomly chosen between 3 and 5.
		public Vector2Int GroupSize = new(3, 5);

		[InlineProperty]
		public WaveEnemyEntry[] Enemies;
	}
}
