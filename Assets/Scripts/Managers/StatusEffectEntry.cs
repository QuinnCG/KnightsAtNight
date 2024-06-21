using Sirenix.OdinInspector;

namespace Quinn
{
	[System.Serializable]
	public struct StatusEffectEntry
	{
		[HorizontalGroup, HideLabel]
		public StatusEffectType Type;
		[HorizontalGroup, HideLabel]
		public bool IsInfinite;
		[HorizontalGroup, HideLabel, HideIf(nameof(IsInfinite)), Unit(Units.Second)]
		public float Duration;
	}
}
