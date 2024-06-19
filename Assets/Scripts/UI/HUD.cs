using Quinn.AI;
using Quinn.CardSystem;
using UnityEngine;
using UnityEngine.UIElements;

namespace Quinn.UI
{
	[RequireComponent(typeof(UIDocument))]
	[RequireComponent(typeof(CardManager))]
	public class HUD : MonoBehaviour
	{
		private CardManager _manager;
		private VisualElement _root;

		private ProgressBar _towerHealth;
		private Label _mana;
		private Label _wave;
		private Label _alive;

		private void Awake()
		{
			_manager = GetComponent<CardManager>();
			_root = GetComponent<UIDocument>().rootVisualElement;

			_towerHealth = _root.Q<ProgressBar>("tower-health");
			_mana = _root.Q<Label>("mana");
			_wave = _root.Q<Label>("wave");
			_alive = _root.Q<Label>("alive");
		}

		private void Update()
		{
			_towerHealth.value = Tower.Instance.Health.Percent;
			_mana.text = $"Mana: {_manager.Mana}/{_manager.MaxMana}";
			_wave.text = $"Wave: {WaveManager.Instance.WaveNumber}";
			_alive.text = $"Alive: {WaveManager.Instance.AliveCount}x";
		}
	}
}
