using FMODUnity;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.VFX;

namespace Quinn.CardSystem.Effect
{
	public class MissileEffect : SpellEffect
	{
		[AssetsOnly]
		public Sprite Sprite;
		public VisualEffectAsset Trail;
		[InlineProperty]
		public SpellEffect HitEffect;
		[AssetsOnly]
		public GameObject Prefab;

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

			GameObject trail = null;
			if (Trail)
			{
				trail = Trail.Clone(new(), parent: missile.transform).gameObject;
			}

			if (Prefab)
			{
				Prefab.Clone(missile.transform);
			}

			missile.OnHit += () =>
			{
				HitEffect?.Activate(new(missile.transform.position, null, context.Source, this));

				if (HitVFX.IsValid) HitVFX.Spawn(context.Position);
				HitSound.PlayOnce(context.Position);

				if (trail)
				{
					trail.transform.parent = null;
					Object.Destroy(trail, 3f);
				}
			};

			missile.Launch(Player.MousePos, this);
			if (SpawnVFX.IsValid) SpawnVFX.Spawn(context.Position);
			SpawnSound.PlayOnce(context.Position);
		}
	}
}
