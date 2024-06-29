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

		[Space, SerializeField]
		private float YOffset = 0.3f;

		[Space, SerializeField, Required]
		private Transform Body;

		[Space, SerializeField, Required]
		private VisualEffectAsset BurningVFX;
		[SerializeField, Required]
		private VisualEffectAsset WeakenedVFX, SlowedVFX, ShockedVFX, GhostFireBurningVFX;

		private readonly Dictionary<StatusEffectType, float> _statuses = new();
		private readonly Dictionary<StatusEffectType, VisualEffect> _vfx = new();

		private Health _health;
		private float _nextBurningHurtTime;

		private void Awake()
		{
			_health = GetComponent<Health>();
			_health.OnDeath += () => Clear();
		}

		private void FixedUpdate()
		{
			if (!Has(StatusEffectType.Burning) && !Has(StatusEffectType.GhostFireBurning))
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
			if (_health.IsDead) return;

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

		public void Clear()
		{
			foreach (var status in _statuses.Keys)
			{
				DestroyVFX(status);
			}

			_statuses.Clear();
		}

		private void CreateVFX(StatusEffectType type)
		{
			if (!_vfx.ContainsKey(type))
			{
				var vfx = type switch
				{
					StatusEffectType.Burning => BurningVFX.Clone(Body),
					StatusEffectType.Weakened => WeakenedVFX.Clone(Body),
					StatusEffectType.Slowed => SlowedVFX.Clone(Body),
					StatusEffectType.Shocked => ShockedVFX.Clone(Body),
					StatusEffectType.GhostFireBurning => GhostFireBurningVFX.Clone(Body),
					_ => throw new System.NotImplementedException(),
				};

				_vfx.Add(type, vfx);
				vfx.transform.localPosition += 0.5f * gameObject.GetColliderSize().y * Vector3.up;
				vfx.transform.localPosition += YOffset * Vector3.up;
				vfx.gameObject.name = $"Status ({type})";
			}
		}

		private void DestroyVFX(StatusEffectType type)
		{
			if (_vfx.TryGetValue(type, out VisualEffect vfx))
			{
				if (vfx != null)
				{
					vfx.SetBool("Enabled", false);
					Destroy(vfx.gameObject, DestroyVFXDelay);
				}
			}
		}
	}
}
