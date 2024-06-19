using DG.Tweening;
using Quinn.CardSystem.Effect;
using System;
using UnityEngine;

namespace Quinn.CardSystem
{
	public class Missile : MonoBehaviour
	{
		public event Action OnHit;

		public void Launch(Vector2 target, MissileSpellEffect settings)
		{
			float dst = transform.position.DistanceTo(target);
			float h = Mathf.Sqrt(dst.Squared() + settings.ArchHeight.Squared());
			dst = h * 2f;

			float dur = dst / settings.Speed;

			transform.DOJump(target, settings.ArchHeight, 1, dur)
				.SetEase(Ease.Linear)
				.onComplete += () =>
				{
					OnHit?.Invoke();
					Destroy(gameObject);
				};
		}
	}
}
