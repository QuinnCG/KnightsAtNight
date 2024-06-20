using Sirenix.OdinInspector;
using UnityEngine;

namespace Quinn.CardSystem.Effect
{
	public class DamageEffect : SpellEffect
	{
		public bool InstaKill = false;
		[HideIf(nameof(InstaKill))]
		public Vector2 Damage = new(10f, 10f);
		public float Knockback = 10f;

		protected override void OnActivate(EffectContext context)
		{
			Debug.Assert(context.Target, "DamageEffect requires a target to be passed!");

			float dmg = InstaKill ? context.Target.Current : Damage.GetRandom();
			Vector2 dir = context.Position.DirectionTo(context.Target.transform.position);

			context.Target.TakeDamage(dmg, dir, Knockback);
		}
	}
}
