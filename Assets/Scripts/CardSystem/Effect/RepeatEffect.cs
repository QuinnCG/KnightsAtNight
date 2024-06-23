using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Quinn.CardSystem.Effect
{
	public class RepeatEffect : SpellEffect
	{
		[InlineProperty]
		public SpellEffect Effect;
		[Min(0)]
		public int Count = 2;
		[Min(0f)]
		public float Interval = 0f;

		protected override void OnActivate(EffectContext context)
		{
			Effect.Activate(new(context, this));

			for (int i = 0; i < Count - 1; i++)
			{
				DOVirtual.DelayedCall(Interval, () => Effect.Activate(new(context, this)), false);
			}
		}
	}
}
