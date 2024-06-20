using Sirenix.OdinInspector;
using UnityEngine;

namespace Quinn.AI
{
	[RequireComponent(typeof(Collider2D))]
	public class UnitAI : AgentAI
	{
		[SerializeField]
		private bool DebugLog;

		[SerializeField]
		private LayerMask TargetMask;

		[SerializeField]
		private float AttackRange = 0.8f;

		[SerializeField]
		private float PatrolSightRadius = 3f, AttackSightRadius = 5f;

		[SerializeField]
		private float WanderDistance = 1f;

		[SerializeField, MinMaxSlider(0f, 10f, ShowFields = true)]
		private Vector2 WanderInterval = new(0.5f, 4f);

		private Collider2D _collider;

		private Vector2 _rallyPoint;
		private Vector2 _wanderPos;
		private Vector2 _orderPos;
		private Transform _targetTransform;
		private Health _targetHealth;
		private float _nextWanderTime;

		// TODO: Stop units from getting stuck on tower sometimes.

		protected override void Awake()
		{
			base.Awake();

			_collider = GetComponent<Collider2D>();
			TransitionTo(OnPatrol);
		}

		private void Start()
		{
			_rallyPoint = transform.position;
			_wanderPos = transform.position;
		}

		protected override void Update()
		{
			base.Update();

			if (DebugLog && Time.frameCount % 4 == 0)
			{
				Debug.Log($"State ({name}): {ActiveState.Method.Name.Bold()}");
			}
		}

		public void Order(Vector2 target)
		{
			_orderPos = target;
			TransitionTo(OnOrder);
		}

		private void OnPatrol(bool isStart)
		{
			if (isStart)
			{
				_wanderPos = _rallyPoint + (Random.insideUnitCircle * WanderDistance);
				_nextWanderTime = Time.time + Random.Range(WanderInterval.x, WanderInterval.y);
			}

			// Look for enemies, goto attack if found
			var hostile = FindTarget(PatrolSightRadius);

			if (hostile)
			{
				_targetTransform = hostile.transform;
				_targetHealth = _targetTransform.GetComponent<Health>();
				TransitionTo(OnAttack);
				return;
			}

			// Move to wander target position.
			float dst = Vector2.Distance(transform.position, _wanderPos);

			if (Time.time > _nextWanderTime)
			{
				if (dst > Movement.StoppingDistance)
				{
					// Disabling wandering.
					//Movement.MoveTo(_wanderPos);
				}
				else
				{
					_nextWanderTime = Time.time + Random.Range(WanderInterval.x, WanderInterval.y);
					TransitionTo(OnPatrol);
				}
			}
		}

		private void OnOrder(bool isStart)
		{
			if (isStart)
			{
				_rallyPoint = _orderPos;
			}

			Movement.MoveTo(_orderPos);

			if (transform.position.DistanceTo(_orderPos) < Movement.StoppingDistance)
			{
				TransitionTo(OnPatrol);
			}
		}

		private void OnAttack(bool isStart)
		{
			_rallyPoint = transform.position;

			if (_targetTransform == null || _targetHealth.Current == 0f)
			{
				TransitionTo(OnPatrol);
				return;
			}

			float dstToTarget = transform.position.DistanceTo(_targetTransform.position);
			if (dstToTarget < AttackRange)
			{
				Combat.Attack(_targetTransform.gameObject);
			}
			else if (dstToTarget > AttackSightRadius)
			{
				TransitionTo(OnPatrol);
			}
			else
			{
				Movement.MoveTo(_targetTransform.position);
			}
		}

		private GameObject FindTarget(float radius)
		{
			Vector2 origin = _collider.bounds.center;
			var hit = Physics2D.OverlapCircle(origin, radius, TargetMask);

			if (hit) return hit.gameObject;
			return null;
		}
	}
}
