using Sirenix.OdinInspector;
using UnityEngine;

namespace Quinn.AI
{
	[System.Serializable]
	public class Wave
	{
		[Tooltip("Before this wave count, this wave will not be spawned.")]
		public int DebutWave = 1;

		[Tooltip("The number of parts to split this wave into.")]
		public Vector2Int SubwaveCount = new(1, 1);

		// Spawn enemies in chunks of a size randomly chosen between 3 and 5.
		public Vector2Int GroupSize = new(3, 5);

		[InlineProperty]
		public WaveEnemyEntry[] Enemies;
	}
}
