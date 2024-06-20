using Quinn.AI;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Quinn
{
	public class WaveManager : MonoBehaviour
	{
		public static WaveManager Instance { get; private set; }

		[SerializeField, Tooltip("Invaders will spawn outside of this zone.")]
		private float SafeZone = 10f;

		[SerializeField]
		private Vector2 GroupSpawnInterval = new(2f, 3f);

		[SerializeField]
		private float PerInvaderSpawnInterval = 0.5f;

		[SerializeField]
		private float WaveDowntime = 10f;

		[SerializeField]
		private float WaveSpawnCountFactor = 0.2f;

		[SerializeField, InlineProperty]
		private Wave[] Waves;

		public int WaveNumber { get; private set; } = 1;
		public int AliveCount => _spawnedEnemies.Count;

		private readonly Queue<GameObject> _toSpawn = new();
		private readonly HashSet<GameObject> _spawnedEnemies = new();

		private bool _inWave = true;
		private float _nextWaveTime;
		private float _nextGroupSpawn;

		private Wave _activeWave;
		private IEnumerable<PathNode> _spawnPoints;

		private void Awake()
		{
			Instance = this;

			_activeWave = GetRandomWave();
			GenerateToSpawnQueue(_activeWave);
		}

		private void Start()
		{
			_spawnPoints = PathManager.Instance.PathSpawns;
		}

		private void FixedUpdate()
		{
			if (Time.frameCount % 2 != 0) return;

			if (_inWave)
			{
				if (Time.time > _nextGroupSpawn)
				{
					_nextGroupSpawn = Time.time + GroupSpawnInterval.GetRandom();
					StartCoroutine(SpawnGroup());
				}

				if (_toSpawn.Count == 0 && _spawnedEnemies.Count == 0)
				{
					_nextWaveTime = Time.time + WaveDowntime;

					_activeWave = GetRandomWave();
					GenerateToSpawnQueue(_activeWave);

					_inWave = false;
				}
			}
			else if (Time.time > _nextWaveTime)
			{
				_inWave = true;
				WaveNumber++;

				Debug.Log($"New wave spawn factor: {WaveNumber * WaveSpawnCountFactor:0.00}.");
			}
		}

		private void OnDrawGizmosSelected()
		{
			Gizmos.color = Color.red;
			Gizmos.DrawWireSphere(transform.position, SafeZone);
		}

		// TODO: Finish wave system.
		/* Data:
		 * Describes types of waves (enemies in wave and ranges of enemies and ratios).
		 * Describes weight for a given wave to be chosen over others.
		 * Describes after what wave count can this wave actually be put in the spawning pool.
		 * 
		 * Behavior:
		 * Get all valid waves.
		 * Select random by weights.
		 * Spawn part of wave, when enough enemies are dead spawn another part. Continue until all are spawned.
		 */

		private GameObject Spawn(GameObject prefab, PathNode node)
		{
			var instance = prefab.Clone(node.Position);
			instance.GetComponent<Health>().OnDeath += () => _spawnedEnemies.Remove(instance);
			instance.GetComponent<InvaderAI>().SetStartingNode(node);

			_spawnedEnemies.Add(instance);
			return instance;
		}

		private PathNode GetRandomSpawn()
		{
			//float angle = Random.Range(0f, Mathf.PI * 2f);
			//var dir = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));

			//float dst = SafeZone + Random.Range(1f, 3f);
			//Vector2 pos = (Vector2)transform.position + (dir * dst);

			//return pos;

			return _spawnPoints.GetRandom();
		}

		private Wave GetRandomWave()
		{
			return Waves.GetRandom();
		}

		private void GenerateToSpawnQueue(Wave wave)
		{
			_toSpawn.Clear();

			foreach (var entry in wave.Enemies)
			{
				int count = entry.Count.GetRandom(false);
				count *= Mathf.CeilToInt(WaveNumber * WaveSpawnCountFactor);

				for (int i = 0; i < count; i++)
				{
					_toSpawn.Enqueue(entry.Prefab);
				}
			}
		}

		private IEnumerator SpawnGroup()
		{
			var node = GetRandomSpawn();

			for (int i = 0; _toSpawn.Count != 0 && i < _activeWave.GroupSize.GetRandom(); i++)
			{
				Spawn(_toSpawn.Dequeue(), node);
				yield return new WaitForSeconds(PerInvaderSpawnInterval);

				// TODO: Will this function finished if its called again too soon?
			}
		}
	}
}
