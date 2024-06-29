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

		[SerializeField, Space]
		private float TutorialDisplayDuration = 20f;
		[SerializeField, Tooltip("Only used when timer expires.")]
		private float TutorialFadeDuration = 3f;

		private CardManager _manager;
		private VisualElement _root;

		private ProgressBar _towerHealth;
		private Label _mana;
		private Label _wave;
		private Label _alive;
		private Label _tutorial;

		private Tween _manaLowTween;

		private Tween _tutorialTween;
		private bool _tutorialHidden;
		private float _tutorialExpireTime;
		private bool _animatingTutorial;

		private void Awake()
		{
			_manager = GetComponent<CardManager>();
			_root = GetComponent<UIDocument>().rootVisualElement;

			_towerHealth = _root.Q<ProgressBar>("tower-health");
			_mana = _root.Q<Label>("mana");
			_wave = _root.Q<Label>("wave");
			_alive = _root.Q<Label>("alive");
			_tutorial = _root.Q<Label>("tutorial");

			_tutorialExpireTime = Time.time + TutorialDisplayDuration;
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

			if (Input.GetKeyDown(KeyCode.Escape) && (!_tutorialHidden || _animatingTutorial))
			{
				_tutorialHidden = true;
				_tutorialTween?.Kill();

				_tutorial.style.opacity = 0f;
				_animatingTutorial = false;
			}

			if (!_tutorialHidden && Time.time > _tutorialExpireTime)
			{
				_tutorialHidden = true;
				_animatingTutorial = true;

				_tutorialTween = DOTween.To(() => _tutorial.resolvedStyle.opacity, x => _tutorial.style.opacity = x, 0f, TutorialFadeDuration);
				_tutorialTween.onComplete += () =>
				{
					_animatingTutorial = false;
					_tutorialHidden = true;
					_tutorialTween = null;
				};
			}
		}
	}
}
