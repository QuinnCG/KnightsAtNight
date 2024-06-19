using Sirenix.OdinInspector;
using UnityEngine;

namespace Quinn.CardSystem.Effect
{
	public class MissileSpellEffect : SpellEffect
	{
		[AssetsOnly]
		public Sprite Sprite;

		[InlineProperty]
		public SpellEffect[] OnHitEffects;

		public float ArchHeight = 4f;
		public float Speed = 10f;
		public override void Activate(Vector2 position)
		{
			var missile = "Missile.prefab".Clone<Missile>(Tower.Instance.SpellSpawnPoint);
			missile.GetComponentInChildren<SpriteRenderer>().sprite = Sprite;
			missile.OnHit += () =>
			{
				foreach (var effect in OnHitEffects)
				{
					effect.Activate(missile.transform.position);
				}
			};
			missile.Launch(Player.MousePos, this);
		}
	}
}
