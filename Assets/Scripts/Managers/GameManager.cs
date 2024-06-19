using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;

namespace Quinn
{
	public class GameManager : MonoBehaviour
	{
		public static GameManager Instance { get; private set; }

		public bool IsPaused { get; private set; }

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void Bootstrap()
		{
			var instance = Addressables.InstantiateAsync("GameManager.prefab").WaitForCompletion();
			DontDestroyOnLoad(instance);

			Instance = instance.GetComponent<GameManager>();
		}

		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.Escape) && !SelectionManager.Instance.AnySelected)
			{
				// TODO: Add pause menu.
			}
		}

		public void RestartGame()
		{
			SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
		}
	}
}
