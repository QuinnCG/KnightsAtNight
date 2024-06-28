using DG.Tweening;
using Quinn.CardSystem;
using UnityEngine;
using UnityEngine.UIElements;

namespace Quinn.UI
{
	[RequireComponent(typeof(UIDocument))]
	[RequireComponent(typeof(CardManager))]
	public class HUD : MonoBehaviour
	{
		[SerializeField]
		private Gradient ManaColor;
		[SerializeField]
		private float LowManaScaleFactor = 1.1f;

		private CardManager _manager;
		private VisualElement _root;

		private ProgressBar _towerHealth;
		private Label _mana;
		private Label _wave;
		private Label _alive;

		private Tween _manaLowTween;

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
			_alive.text = $"Hostiles: {WaveManager.Instance.AliveCount}x";

			_mana.style.color = ManaColor.Evaluate(1f - ((float)_manager.Mana / _manager.MaxMana));

			// Tweening.
			float manaPercent = (float)_manager.Mana / _manager.MaxMana;

			if (manaPercent <= 0.3f && _manaLowTween == null)
			{
				_manaLowTween = DOTween.To(() => _mana.transform.scale, x => _mana.transform.scale = x, Vector3.one * LowManaScaleFactor, 1f)
					.SetLoops(-1, LoopType.Yoyo)
					.SetEase(Ease.InOutCubic);
			}
			else if (manaPercent > 0.3f && _manaLowTween != null)
			{
				_manaLowTween.Kill(true);
			}
		}
	}
}
