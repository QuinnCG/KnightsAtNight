using Quinn.Common;
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
		[SerializeField, Required]
		private Slider HealthBar;

		protected Health Health { get; private set; }
		protected Movement Movement { get; private set; }
		protected Combat Combat { get; private set; }
		protected State ActiveState { get; private set; }

		private bool _isStart = true;

		protected virtual void Awake()
		{
			Health = GetComponent<Health>();
			Health.OnDamaged += () => HealthBar.gameObject.SetActive(true);
			Health.OnFullHealed += () => HealthBar.gameObject.SetActive(false);
			HealthBar.gameObject.SetActive(false);

			Movement = GetComponent<Movement>();
			Combat = GetComponent<Combat>();
		}

		protected virtual void Update()
		{
			HealthBar.value = Health.Percent;

			if (ActiveState != null)
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
