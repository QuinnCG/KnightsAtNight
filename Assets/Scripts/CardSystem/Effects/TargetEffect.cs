using UnityEngine;

namespace Quinn.CardSystem.Effect
{
	public class TargetEffect : SpellEffect
	{
		[Tooltip("This can be negative to deal damage.")]
		public float Health;
		[Tooltip("Set this to true to make this spell about damaging or mending buildings.")]
		public bool TargetBuldings = false;

		public override void Activate(Vector2 position)
		{
			throw new System.NotImplementedException();
		}
	}
}
