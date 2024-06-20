namespace Quinn
{
	public class StatusEffect
	{
		public StatusEffectType Type { get; }
		public float EndTime { get; }

		public StatusEffect(StatusEffectType type, float endTime)
		{
			Type = type;
			EndTime = endTime;
		}
	}
}
