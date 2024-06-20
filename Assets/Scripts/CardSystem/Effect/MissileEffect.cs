using FMODUnity;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Quinn.CardSystem.Effect
{
	public class MissileEffect : SpellEffect
	{
		[AssetsOnly]
		public Sprite Sprite;
		[InlineProperty]
		public SpellEffect HitEffect;

		[Space]
		public EventReference SpawnSound;
		[InlineProperty]
		public VFX SpawnVFX = new();

		[Space]
		public EventReference HitSound;
		[InlineProperty]
		public VFX HitVFX = new();

		public float ArchHeight = 4f;
		public float Speed = 10f;

		protected override void OnActivate(EffectContext context)
		{
			var missile = "Missile.prefab".Clone<MissileController>(Tower.Instance.SpellSpawnPoint);
			missile.GetComponentInChildren<SpriteRenderer>().sprite = Sprite;
			missile.OnHit += () =>
			{
				HitEffect.Activate(new(missile.transform.position, null, context.Source, this));

				if (SpawnVFX.IsValid) HitVFX.Spawn(context.Position);
				HitSound.PlayOnce(context.Position);
			};

			missile.Launch(Player.MousePos, this);
			if (SpawnVFX.IsValid) SpawnVFX.Spawn(context.Position);
			SpawnSound.PlayOnce(context.Position);
		}
	}
}
