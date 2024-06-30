using FMODUnity;
using Quinn.AI;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Quinn
{
	public class WaveManager : MonoBehaviour
	{
		public static WaveManager Instance { get; private set; }

		[SerializeField]
		private Vector2 GroupSpawnInterval = new(2f, 3f);

		[SerializeField]
		private float PerInvaderSpawnInterval = 0.5f;

		[SerializeField]
		private float WaveDowntime = 10f;

		[SerializeField]
		private float WaveSpawnCountFactor = 0.2f;

		[SerializeField]
		private EventReference GroupSpawnSound;

		[SerializeField]
		private Wave[] Waves;

		public int WaveNumber { get; private set; }
		public int AliveCount => _spawnedEnemies.Count;

		private readonly Queue<GameObject> _toSpawn = new();
		private readonly HashSet<GameObject> _spawnedEnemies = new();

		private bool _inWave;
		private float _nextWaveTime;
		private float _nextGroupSpawn;

		private Wave _activeWave;
		private IEnumerable<PathNode> _spawnPoints;

		private void Awake()
		{
			Instance = this;
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
				if (Time.time > _nextGroupSpawn && _toSpawn.Count > 0)
				{
					_nextGroupSpawn = Time.time + GroupSpawnInterval.GetRandom();
					StartCoroutine(SpawnGroup());
				}

				if (_toSpawn.Count == 0 && _spawnedEnemies.Count == 0)
				{
					_nextWaveTime = Time.time + WaveDowntime;
					_inWave = false;
				}
			}
			else if (Time.time > _nextWaveTime)
			{
				WaveNumber++;
				GameManager.Instance.WavesSurvived = WaveNumber - 1;

				_activeWave = GetRandomWave();
				_inWave = true;

				Logger.LogGroup(
					$"New Wave (#{WaveNumber})", "green",
					$"Spawn Factor: {CalculateSpawnFactor():0.00}");

				GenerateSpawnQueue();
			}
		}

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
			var waves = Waves.Where(x => x.DebutWave <= WaveNumber);
			return waves.GetRandom();
		}

		private void GenerateSpawnQueue()
		{
			_toSpawn.Clear();

			foreach (var entry in _activeWave.Enemies)
			{
				int count = entry.Count.GetRandom(false);
				int countScaled = count * Mathf.RoundToInt(CalculateSpawnFactor());

				for (int i = 0; i < countScaled; i++)
				{
					_toSpawn.Enqueue(entry.Prefab);
				}
			}

			Logger.LogGroup(
				"Generating Spawn Queue", "green",
				$"ToSpawn: {_toSpawn.Count}x",
				$"From Wave: {_activeWave.Title}");
		}

		private IEnumerator SpawnGroup()
		{
			var node = GetRandomSpawn();
			GroupSpawnSound.PlayOnce(node.Position);

			for (int i = 0; _toSpawn.Count != 0 && i < _activeWave.GroupSize.GetRandom(); i++)
			{
				Spawn(_toSpawn.Dequeue(), node);
				yield return new WaitForSeconds(PerInvaderSpawnInterval);

				// TODO: Will this function finished if its called again too soon?
			}
		}

		private float CalculateSpawnFactor()
		{
			if (WaveNumber == 1) return 1f;
			return ((WaveNumber - 1) * WaveSpawnCountFactor) + 1f;
		}
	}
}
