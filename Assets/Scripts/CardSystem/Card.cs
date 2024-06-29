using Sirenix.OdinInspector;
using UnityEngine;

namespace Quinn.CardSystem
{
	public abstract class Card : SerializedScriptableObject
	{
		public string Title = "Card's Title";
		[Multiline]
		public string Description = "This is the card's description.\nIt supports <b>rich</b> text.";
		public Sprite Art;
		public int Cost = 1;
		[Tooltip("Won't be drawn before this wave.")]
		public int DebutWave = 1;

		public abstract void Cast();
	}
}
