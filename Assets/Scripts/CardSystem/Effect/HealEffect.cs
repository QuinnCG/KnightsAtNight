using Sirenix.OdinInspector;
using UnityEngine;

namespace Quinn.CardSystem.Effect
{
	public class HealEffect : SpellEffect
	{
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
		}
	}
}
