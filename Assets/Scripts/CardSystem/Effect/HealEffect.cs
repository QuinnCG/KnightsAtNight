using Sirenix.OdinInspector;
using UnityEngine;

namespace Quinn.CardSystem.Effect
{
	public class HealEffect : SpellEffect
	{
		[InlineProperty]
		public StatusEffectEntry[] ApplyStatusEffects;
		[InlineProperty]
		public StatusEffectEntry[] RemoveStatusEffects;

		[Space]
		public bool HealFull = false;
		[HideIf(nameof(HealFull))]
		public Vector2 Health = new(10f, 10f);

		protected override void OnActivate(EffectContext context)
		{
			Debug.Assert(context.Target, "HealEffect requires a target to be passed!");
			
			if (HealFull)
			{
				context.Target.Heal();
			}
			else
			{
				context.Target.Heal(Health.GetRandom());
			}

			if (context.Target.TryGetComponent(out StatusEffectManager manager))
			{
				if (ApplyStatusEffects != null)
				{
					foreach (var entry in ApplyStatusEffects)
					{
						manager.Apply(entry.Type, entry.IsInfinite ? float.PositiveInfinity : entry.Duration);
					}
				}

				if (RemoveStatusEffects != null)
				{
					foreach (var entry in RemoveStatusEffects)
					{
						manager.Remove(entry.Type);
					}
				}
			}
		}
	}
}
