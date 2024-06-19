using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.VFX;

namespace Quinn.CardSystem.Effect
{
	public class AOESpellEffect : SpellEffect
	{
		[AssetsOnly]
		public VisualEffect VFX;

		public float Radius;

		[Tooltip("Make this negative to heal per second.")]
		public float DamagePerSecond;

		public bool AffectFriendlies = false;

		public override void Activate(Vector2 position)
		{
			throw new System.NotImplementedException();
		}
	}
}
