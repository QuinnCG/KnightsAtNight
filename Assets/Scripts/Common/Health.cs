using Quinn.AI;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Quinn
{
	public class Health : MonoBehaviour
	{
		[ShowInInspector, ReadOnly]
		public float Current { get; private set; }

		[field: SerializeField, BoxGroup("General", ShowLabel = false)]
		public float Max { get; set; } = 100f;

		[SerializeField, BoxGroup("General", ShowLabel = false)]
		private bool DestroyOnDeath = true;

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

		public bool IsDead => Current == 0f;
		public float Percent => Current / Max;
		public event Action OnDamaged, OnHealed;
		public event Action OnDeath;
		public event Action OnFullHealed;

		private Movement _movement;
		private float _nextRegenTime;

		private void Awake()
		{
			TryGetComponent(out _movement);
		}

		private void Start()
		{
			Current = Max;
		}

		private void Update()
		{
			if (Time.time > _nextRegenTime && EnableRegen)
			{
				Heal(Time.deltaTime * RegenRate);
			}
		}

		public void TakeDamage(float amount, Vector2 direction, float knockbackSpeed)
		{
			// TODO: Directional affects?

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

				if (DestroyOnDeath)
				{
					Destroy(gameObject);
				}
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
	}
}
