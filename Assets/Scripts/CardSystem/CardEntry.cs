using Sirenix.OdinInspector;

namespace Quinn.CardSystem
{
	[System.Serializable]
	public struct CardEntry
	{
		[Required, HorizontalGroup]
		public Card Card;
		[HorizontalGroup]
		public float Weight;
	}
}
