using System;
using UnityEngine;

namespace Quinn.AI
{
	[RequireComponent(typeof(Animator))]
	public class TowerWizardAI : MonoBehaviour
	{
		private Animator _animator;
		private Action _callback;

		private void Awake()
		{
			_animator = GetComponent<Animator>();
		}

		public void PlayCast(Action callback)
		{
			_callback = callback;

			_animator.SetTrigger("Cast");
			transform.localScale = new Vector3(Mathf.Sign(transform.position.DirectionTo(Player.MousePos).x), 1f, 1f);
		}

		public void TriggerSpell()
		{
			_callback?.Invoke();
			_callback = null;
		}
	}
}
