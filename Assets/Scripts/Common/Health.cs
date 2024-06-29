using DG.Tweening;
using FMODUnity;
using Quinn.AI;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Quinn
{
	public class Health : MonoBehaviour
	{
		[ShowInInspector, ReadOnly, BoxGroup("General", ShowLabel = false)]
		public float Current { get; private set; }

		[field: SerializeField, BoxGroup("General", ShowLabel = false)]
		public float Max { get; set; } = 100f;
		[field: SerializeField, BoxGroup("General", ShowLabel = false), Tooltip("Optional.")]
		private Slider HealthBar;

		[SerializeField, BoxGroup("General/FX", ShowLabel = false), DisableIf(nameof(FadeOut))]
		private bool DestroyOnDeath = true;

		[SerializeField, BoxGroup("General/FX", ShowLabel = false), Space]
		private bool FadeOut = false;
		[SerializeField, BoxGroup("General/FX", ShowLabel = false), ShowIf(nameof(FadeOut))]
		private float FadeOutDuration = 3f;
		[SerializeField, BoxGroup("General/FX", ShowLabel = false), ShowIf(nameof(FadeOut))]
		private float FadeOutDelay = 3f;
		[SerializeField, BoxGroup("General/FX", ShowLabel = false), ShowIf(nameof(FadeOut))]
		private GameObject[] HideDuringFadeOut;

		[SerializeField, BoxGroup("Events")]
		private UnityEvent OnDamagedEvent, OnHealedEvent;

		[SerializeField, BoxGroup("Hit Flash")]
		private float FlashDuration = 0.25f;

		[SerializeField, BoxGroup("Hit Flash")]
		private SpriteRenderer[] FlashSpriteRenderers;

		[SerializeField, BoxGroup("Regeneration")]
		private bool EnableRegen = false;

		[SerializeField, BoxGroup("Regeneration"), ShowIf(nameof(EnableRegen))]
		private float RegenCombatDelay = 3f;

		[SerializeField, BoxGroup("Regeneration"), ShowIf(nameof(EnableRegen))]
		private float RegenRate = 10f;

		[SerializeField, BoxGroup("Animator", ShowLabel = false)]
		private string DeathTrigger = "Death";

		[SerializeField, BoxGroup("FX")]
		private EventReference HurtSound, DeathSound;

		public bool IsDead => Current == 0f;
		public float Percent => Current / Max;
		public event Action OnDamaged, OnHealed;
		public event Action OnDeath;
		public event Action OnFullHealed;

		private Movement _movement;
		private Animator _animator;
		private float _nextRegenTime;

		private void Awake()
		{
			TryGetComponent(out _movement);
			TryGetComponent(out _animator);

			Current = Max;
		}

		private void Update()
		{
			if (Time.time > _nextRegenTime && EnableRegen && !IsDead)
			{
				Heal(Time.deltaTime * RegenRate);
			}

			if (HealthBar != null)
			{
				HealthBar.value = Percent;
				HealthBar.gameObject.SetActive(Current != Max);
			}
		}

		public void TakeDamage(float amount, Vector2 direction, float knockbackSpeed)
		{
			// TODO: Directional affects?

			if (IsDead) return;
			_nextRegenTime = Time.time + RegenCombatDelay;

			Current = Mathf.Max(0f, Current - amount);
			OnDamaged?.Invoke();
			OnDamagedEvent?.Invoke();

			if (FlashSpriteRenderers.Length > 0)
			{
				StopAllCoroutines();
				StartCoroutine(HurtSequence());
			}

			if (Current == 0f)
			{
				OnDeath?.Invoke();

				if (_animator)
				{
					_animator.SetBool(DeathTrigger, true);
					
					if (FadeOut)
					{
						StartCoroutine(DeathSequence());
					}
					else if (DestroyOnDeath)
					{
						Destroy(gameObject);
					}
				}

				DeathSound.PlayOnce(transform.position);
			}
			else
			{
				HurtSound.PlayOnce(transform.position);
			}

			if (_movement != null)
			{
				_movement.ApplyKnockback(direction, knockbackSpeed);
			}
		}

		public void Heal()
		{
			Heal(Max - Current);
		}
		public void Heal(float amount)
		{
			Current = Mathf.Min(Max, Current + amount);
			OnHealed?.Invoke();
			OnHealedEvent?.Invoke();

			if (Current == Max)
			{
				OnFullHealed?.Invoke();
			}
		}

		// Flash white.
		private IEnumerator HurtSequence()
		{
			for (float f = 0f; f < 1f; f += Time.deltaTime / FlashDuration / 2f)
			{
				foreach (var renderer in FlashSpriteRenderers)
				{
					renderer.material.SetFloat("_Flash", Mathf.Min(f, 1f));
				}

				yield return null;
			}

			yield return new WaitForSeconds(FlashDuration * 0.1f);

			for (float f = 1f; f > 0f; f -= Time.deltaTime / FlashDuration / 2f)
			{
				foreach (var renderer in FlashSpriteRenderers)
				{
					renderer.material.SetFloat("_Flash", Mathf.Max(0f, f));
				}
			}

			foreach (var renderer in FlashSpriteRenderers)
			{
				renderer.material.SetFloat("_Flash", 0f);
			}
		}

		// Fade out.
		private IEnumerator DeathSequence()
		{
			foreach (var gameObject in HideDuringFadeOut)
			{
				gameObject.SetActive(false);
			}

			yield return new WaitForSeconds(FadeOutDelay);
			var renderers = GetComponentsInChildren<SpriteRenderer>();

			Tween firstFade = null;

			foreach (var renderer in renderers)
			{
				var tween = renderer.DOFade(0f, FadeOutDuration);
				firstFade ??= tween;
			}

			firstFade.onComplete += () =>
			{
				Destroy(gameObject);
			};
		}
	}
}
