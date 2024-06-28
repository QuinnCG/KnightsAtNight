using Quinn.AI;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Quinn
{
	[RequireComponent(typeof(Collider2D))]
	[RequireComponent(typeof(Health))]
	public class Tower : MonoBehaviour
	{
		public static Tower Instance { get; private set; }

		[SerializeField, Required]
		private Transform SpellSpawnTransform;

		[SerializeField, Required]
		private TowerWizardAI TowerWizard;

		public Vector2 SpellSpawnPoint => Instance.SpellSpawnTransform.position;
		public Health Health { get; private set; }

		private Collider2D _collider;

		private void Awake()
		{
			Instance = this;

			_collider = GetComponent<Collider2D>();
			Health = GetComponent<Health>();
			Health.OnDeath += OnDeath;
		}

		public Vector2 GetClosestPoint(Vector2 to)
		{
			return _collider.ClosestPoint(to);
		}

		public void CastSpell()
		{
			TowerWizard.PlayCast();
		}

		private void OnDeath()
		{
			GameManager.Instance.RestartGame();
		}
	}
}
