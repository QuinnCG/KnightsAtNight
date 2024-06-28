using UnityEngine;

namespace Quinn.AI
{
	[RequireComponent(typeof(Collider2D))]
	public class InvaderAI : AgentAI
	{
		[SerializeField]
		private LayerMask TargetMask;

		[SerializeField]
		private float AttackRange = 1f, TowerAttackRange = 2f;

		[SerializeField]
		private float EngageRadius = 3f;

		[SerializeField]
		private float DisengageRange = 5f;

		private Collider2D _collider;
		private Transform _target;
		private PathNode _next;
		private Health _targetHealth;

		protected override void Awake()
		{
			base.Awake();

			_collider = GetComponent<Collider2D>();
			TransitionTo(OnCharge);
		}

		public void SetStartingNode(PathNode node)
		{
			_next = node;
		}

		// Traverse to tower.
		private void OnCharge(bool isStart)
		{
			if (isStart)
			{
				_target = Tower.Instance.transform;
				_targetHealth = _target.GetComponent<Health>();
			}

			//var target = Tower.Instance.GetClosestPoint(transform.position);
			//Movement.MoveTo(target);

			//if (transform.position.DistanceTo(target) < AttackRange)
			//{
			//	TransitionTo(OnAttack);
			//	return;
			//}

			// TODO: Stop moving when close to tower's closest point on hitbox.

			if (_next != null)
			{
				if (transform.position.DistanceTo(_next.Position) < Movement.StoppingDistance)
				{
					_next = _next.Parent;
				}

				Movement.MoveTo(_next.Position);
			}
			else
			{
				TransitionTo(OnAttack);
			}

			// Attack if close to tower.
			if (GetDistanceToTower() < TowerAttackRange)
			{
				_target = Tower.Instance.transform;
				_targetHealth = _target.GetComponent<Health>();
				TransitionTo(OnAttack);
			}

			// Look for enemies to attack.
			var colliders = Physics2D.OverlapCircleAll(transform.position, EngageRadius, TargetMask);
			foreach (var collider in colliders)
			{
				if (collider.TryGetComponent(out Health health) && !health.IsDead)
				{
					_target = collider.transform;
					_targetHealth = health;

					TransitionTo(OnAttack);
					return;
				}
			}
		}

		// Go to and attack target.
		private void OnAttack(bool isStart)
		{
			if (_target == null)
			{
				TransitionTo(OnCharge);
				return;
			}

			float dstToTarget = _collider.bounds.center.DistanceTo(_target.position);

			if (dstToTarget > DisengageRange)
			{
				TransitionTo(OnCharge);
			}
			else if (_targetHealth.IsDead)
			{
				if (_target == Tower.Instance.transform)
				{
					TransitionTo(null);
				}
				else
				{
					TransitionTo(OnCharge);
				}
			}
			else if (dstToTarget < AttackRange 
				|| (_target.gameObject == Tower.Instance.gameObject 
				&& transform.position.DistanceTo(Tower.Instance.GetClosestPoint(transform.position)) < AttackRange))
			{
				Combat.Attack(_target.gameObject);
			}
			else
			{
				Movement.MoveTo(_target.position);
			}
		}

		private float GetDistanceToTower()
		{
			var pos = Tower.Instance.GetClosestPoint(transform.position);
			return _collider.bounds.center.DistanceTo(pos);
		}
	}
}
