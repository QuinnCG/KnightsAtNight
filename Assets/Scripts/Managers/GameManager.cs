using Quinn.CardSystem;
using System.Collections.Generic;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;

namespace Quinn
{
	public class GameManager : MonoBehaviour
	{
		public static GameManager Instance { get; private set; }

		public bool IsPaused { get; private set; }

		public float AttemptStartTime { get; private set; }
		public int WavesSurvived { get; set; }
		public int EnemiesSlain { get; set; }
		public int SoldiersSummoned { get; set; }
		public int SpellsCast { get; set; }
		public Dictionary<Card, int> CardUses { get; } = new();

		

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void Bootstrap()
		{
			var instance = Addressables.InstantiateAsync("GameManager.prefab").WaitForCompletion();
			DontDestroyOnLoad(instance);

			Instance = instance.GetComponent<GameManager>();
		}

		private async void Awake()
		{
			AttemptStartTime = Time.time;

			await UnityServices.InitializeAsync();
			await AuthenticationService.Instance.SignInAnonymouslyAsync();
		}

		public async void RestartGame()
		{
			await SceneManager.LoadSceneAsync("GameScene");
			AttemptStartTime = Time.time;

			EnemiesSlain = 0;
			SoldiersSummoned = 0;
			SpellsCast = 0;

			CardUses.Clear();
		}
	}
}
