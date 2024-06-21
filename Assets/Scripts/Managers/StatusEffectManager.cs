using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

namespace Quinn
{
	[RequireComponent(typeof(Health))]
	public class StatusEffectManager : MonoBehaviour
	{
		public const float DestroyVFXDelay = 3f;

		[SerializeField]
		private float BurningDamageInterval = 2f;
		[SerializeField]
		private float BurningDamage = 10f;

		[SerializeField, Required]
		private VisualEffectAsset BurningVFX, WeakenedVFX, SlowedVFX, ShockedVFX;

		private readonly Dictionary<StatusEffectType, float> _statuses = new();
		private readonly Dictionary<StatusEffectType, VisualEffect> _vfx = new();

		private Health _health;
		private float _nextBurningHurtTime;

		private void Awake()
		{
			_health = GetComponent<Health>();
		}

		private void FixedUpdate()
		{
			if (!Has(StatusEffectType.Burning))
			{
				_nextBurningHurtTime = Time.time + BurningDamageInterval;
			}
			else if (Time.time > _nextBurningHurtTime)
			{
				_health.TakeDamage(BurningDamage, Vector2.zero, 0f);
				_nextBurningHurtTime = Time.time + BurningDamageInterval;
			}

			if (Time.frameCount % 2 == 0)
			{
				var toRemove = new HashSet<StatusEffectType>();

				foreach (var pair in _statuses)
				{
					if (Time.time >= pair.Value)
					{
						toRemove.Add(pair.Key);
					}
				}

				foreach (var type in toRemove)
				{
					Remove(type);
				}
			}
		}

		public float GetRemainingTime(StatusEffectType type)
		{
			if (Has(type))
			{
				return _statuses[type] - Time.time;
			}
			else
			{
				return 0f;
			}
		}

		public bool Has(StatusEffectType type)
		{
			return _statuses.ContainsKey(type);
		}

		public void Apply(StatusEffectType type, float duration = float.PositiveInfinity)
		{
			if (Has(type))
			{
				_statuses[type] += duration;
			}
			else
			{
				_statuses.Add(type, Time.time + duration);
				CreateVFX(type);
			}
		}

		public void Remove(StatusEffectType type)
		{
			if (_statuses.Remove(type))
			{
				DestroyVFX(type);
			}
		}

		private void CreateVFX(StatusEffectType type)
		{
			if (!_vfx.ContainsKey(type))
			{
				var vfx = type switch
				{
					StatusEffectType.Burning => BurningVFX.Clone(transform),
					StatusEffectType.Weakened => WeakenedVFX.Clone(transform),
					StatusEffectType.Slowed => SlowedVFX.Clone(transform),
					StatusEffectType.Shocked => ShockedVFX.Clone(transform),
					_ => throw new System.NotImplementedException(),
				};

				_vfx.Add(type, vfx);
				vfx.transform.localPosition += 0.5f * gameObject.GetColliderSize().y * Vector3.up;
				vfx.gameObject.name = $"Status ({type})";
			}
		}

		private void DestroyVFX(StatusEffectType type)
		{
			if (_vfx.TryGetValue(type, out VisualEffect vfx))
			{
				vfx.SetBool("Enabled", false);
				Destroy(vfx.gameObject, DestroyVFXDelay);
			}
		}
	}
}
