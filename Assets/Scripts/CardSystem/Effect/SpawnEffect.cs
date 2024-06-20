using Sirenix.OdinInspector;
using UnityEngine;
using FMODUnity;

namespace Quinn.CardSystem.Effect
{
	public class SpawnEffect : SpellEffect
	{
		[AssetsOnly]
		public GameObject Prefab;
		public VFX VFX;
		public EventReference Sound;

		[Space]
		public bool DestroyAfter = false;
		[ShowIf(nameof(DestroyAfter))]
		public float Lifespan = 2f;

		protected override void OnActivate(EffectContext context)
		{
			if (VFX.IsValid)
			{
				VFX.Spawn(context.Position);
			}

			Sound.PlayOnce(context.Position);

			var instance = Prefab.Clone(context.Position);

			if (DestroyAfter)
			{
				Object.Destroy(instance, Lifespan);
			}
		}
	}
}
