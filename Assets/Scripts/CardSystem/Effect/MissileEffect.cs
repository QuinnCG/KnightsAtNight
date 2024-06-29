using FMODUnity;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.VFX;

namespace Quinn.CardSystem.Effect
{
	public class MissileEffect : SpellEffect
	{
		public float ArchHeight = 4f;
		public float Speed = 10f;

		[Space, AssetsOnly]
		public Sprite Sprite;
		public VisualEffectAsset Trail;

		[Space, LabelWidth(80f)]
		public SpellEffect HitEffect;
		[AssetsOnly]
		public GameObject Prefab;
		[Tooltip("The target will be a random point within this radius. Set to 0 for pinpoint accuracy.")]
		public float TargetRadius = 0f;

		[InlineProperty, BoxGroup("Spawn", ShowLabel = false)]
		public VFX SpawnVFX = new();
		[BoxGroup("Spawn", ShowLabel = false)]
		public EventReference SpawnSound;

		[InlineProperty, BoxGroup("Hit", ShowLabel = false)]
		public VFX HitVFX = new();
		[BoxGroup("Hit", ShowLabel = false)]
		public EventReference HitSound;

		protected override void OnActivate(EffectContext context)
		{
			var missile = "Missile.prefab".Clone<MissileController>(Tower.Instance.SpellSpawnPoint);
			missile.GetComponentInChildren<SpriteRenderer>().sprite = Sprite;

			GameObject trail = null;
			if (Trail)
			{
				trail = Trail.Clone(missile.transform).gameObject;
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


			Vector2 targetPos = context.Position + (Random.insideUnitCircle * TargetRadius);
			missile.Launch(targetPos, this);

			if (SpawnVFX.IsValid) SpawnVFX.Spawn(context.Position);
			SpawnSound.PlayOnce(context.Position);
		}
	}
}
