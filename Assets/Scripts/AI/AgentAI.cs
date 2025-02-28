﻿using FMODUnity;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace Quinn.AI
{
	[RequireComponent(typeof(Health))]
	[RequireComponent(typeof(Movement))]
	[RequireComponent(typeof(Combat))]
	public class AgentAI : MonoBehaviour
	{
		[field: SerializeField, Required]
		protected Slider HealthBar { get; private set; }

		[BoxGroup("Grunts")]
		private EventReference AttackGrunt, DeathGrunt;

		protected Health Health { get; private set; }
		protected Movement Movement { get; private set; }
		protected Combat Combat { get; private set; }
		protected State ActiveState { get; private set; }
		protected virtual bool UpdateOnDeath => false;

		private bool _isStart = true;

		protected virtual void Awake()
		{
			Health = GetComponent<Health>();
			Movement = GetComponent<Movement>();
			Combat = GetComponent<Combat>();

			Health.OnDeath += () => DeathGrunt.PlayOnce(transform.position);
			Combat.OnAttack += _ => AttackGrunt.PlayOnce(transform.position);
		}

		protected virtual void Update()
		{
			HealthBar.value = Health.Percent;

			if (ActiveState != null && (UpdateOnDeath || !Health.IsDead))
			{
				if (_isStart)
				{
					ActiveState(true);
					_isStart = false;
				}
				else
				{
					ActiveState(false);
				}
			}
		}

		protected void TransitionTo(State state)
		{
			ActiveState = state;
			_isStart = true;
		}
	}
}
