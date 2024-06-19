using Sirenix.OdinInspector;
using System.Linq;
using UnityEngine;

namespace Quinn.AI
{
	[RequireComponent(typeof(Rigidbody2D))]
	public class Movement : MonoBehaviour
	{
		[field: SerializeField, BoxGroup("Basic", ShowLabel = false)]
		public float Speed { get; set; } = 2f;

		[field: SerializeField, BoxGroup("Basic", ShowLabel = false)]
		public float StoppingDistance { get; private set; } = 0.1f;

		[field: SerializeField, BoxGroup("Basic", ShowLabel = false)]
		private bool FaceRandomOnSpawn = false;

		[field: SerializeField, BoxGroup("Basic", ShowLabel = false), Required, Tooltip("The transform to flip towards movement direction.")]
		private Transform FacingTransform;

		[SerializeField, BoxGroup("Avoidance", Order = 1f)]
		private bool EnableAvoidance = true;

		[SerializeField, BoxGroup("Avoidance", Order = 1f), ShowIf(nameof(EnableAvoidance))]
		private LayerMask AvoidanceTarget;

		[SerializeField, BoxGroup("Avoidance", Order = 1f), ShowIf(nameof(EnableAvoidance))]
		private float AvoidanceRadius = 3f;

		[SerializeField, BoxGroup("Avoidance", Order = 1f), ShowIf(nameof(EnableAvoidance))]
		private float AvoidanceFactor = 0.3f;

		[SerializeField, BoxGroup("Knockback", Order = 2f)]
		private bool EnableKnockback = true;

		[SerializeField, BoxGroup("Knockback", Order = 2f), ShowIf(nameof(EnableKnockback))]
		private float KnockbackDuration = 0.3f;

		[SerializeField, BoxGroup("Knockback", Order = 2f), ShowIf(nameof(EnableKnockback))]
		private AnimationCurve KnockbackDecayCurve;

		[SerializeField, BoxGroup("Knockback", Order = 2f), ShowIf(nameof(EnableKnockback))]
		private float KnockbackSpeedFactor = 1f;

		public Vector2 Direction => _vel.normalized;
		public float Angle => Mathf.Atan2(_vel.y, _vel.x);
		public float AngleDeg => Angle * Mathf.Rad2Deg;

		private Rigidbody2D _rb;
		private Vector2 _vel;

		private Vector2 _knockbackDir;
		private float _knockbackInitialSpeed;
		private float _knockbackSpeed;
		private float _knockbackStartTime;

		// Zeroed out every frame.
		private Vector2 _knockbackVel;

		private void Awake()
		{
			_rb = GetComponent<Rigidbody2D>();

			// Spawn facing a random direction (left or right).
			if (FaceRandomOnSpawn)
			{
				FacingTransform.localScale = new Vector3(Random.value > 0.5f ? 1f : -1f, 1f, 1f);
			}
		}

		private void Update()
		{
			if (EnableKnockback && _knockbackSpeed > 0f)
			{
				float elapsedPercent = (Time.time - _knockbackStartTime) / KnockbackDuration;
				elapsedPercent = Mathf.Min(elapsedPercent, 1f);

				_knockbackSpeed = KnockbackDecayCurve.Evaluate(elapsedPercent);
				_knockbackSpeed *= _knockbackInitialSpeed;
				_knockbackSpeed *= KnockbackSpeedFactor;

				_knockbackVel = _knockbackDir * _knockbackSpeed;
			}
		}

		private void LateUpdate()
		{
			if (_vel.x != 0f)
			{
				FacingTransform.localScale = new Vector3(Mathf.Sign(_vel.x), 1f, 1f);
			}

			_rb.velocity = _vel + _knockbackVel;
			_vel = Vector2.zero;
			_knockbackVel = Vector2.zero;
		}

		public void Move(Vector2 dir)
		{
			_vel = Speed * dir.normalized;
		}

		public bool MoveTo(Vector2 pos)
		{
			var dirToTarget = transform.position.DirectionTo(pos);

			if (EnableAvoidance)
			{
				if (GetClosestObstacle(out Vector2 obstacle))
				{
					Vector2 obstacleDir = transform.position.DirectionTo(obstacle);
					float obstacleAngle = Mathf.Atan2(obstacleDir.y, obstacleDir.x);

					float deltaAngle = Angle - (obstacleAngle * AvoidanceFactor);
					var avoidDir = new Vector2(Mathf.Cos(deltaAngle), Mathf.Sin(deltaAngle));

					dirToTarget = avoidDir;

					float dstToObstacle = transform.position.DistanceTo(obstacle);
					Draw.Line(transform.position, transform.position + ((Vector3)obstacleDir * dstToObstacle), Color.red);
				}

				Draw.Line(transform.position, transform.position + dirToTarget, Color.blue);
			}

			float dst = Vector2.Distance(transform.position, pos);
			if (dst > StoppingDistance)
			{
				Move(dirToTarget);
			}

			return dst <= StoppingDistance;
		}

		public void ApplyKnockback(Vector2 direction, float speed)
		{
			if (EnableKnockback)
			{
				_knockbackDir = direction;
				_knockbackInitialSpeed = speed;
				_knockbackSpeed = speed;
				_knockbackStartTime = Time.time;
			}
		}

		private bool GetClosestObstacle(out Vector2 pos)
		{
			var colliders = Physics2D.OverlapCircleAll(transform.position, AvoidanceRadius, AvoidanceTarget).ToList();
			colliders.Remove(GetComponent<Collider2D>());
			var collider = colliders.Lowest(x => transform.position.DistanceTo(x.transform.position));

			if (collider != null)
			{
				pos = collider.transform.position;
				return true;
			}

			pos = transform.position;
			return false;
		}
	}
}
