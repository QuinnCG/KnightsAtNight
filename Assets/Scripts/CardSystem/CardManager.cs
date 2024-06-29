using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Quinn.CardSystem
{
	public class CardManager : MonoBehaviour
	{
		[field: SerializeField]
		public int MaxHandSize { get; private set; } = 7;

		[field: SerializeField]
		public int MaxMana { get; private set; } = 10;

		[SerializeField]
		private float ManaRegenInterval = 1f;

		[SerializeField]
		private float CardDrawInterval = 5f;

		[SerializeField]
		private Card[] StartingHand;

		[SerializeField]
		private CardEntry[] DeckPool;

		public IEnumerable<Card> Hand => _hand;
		public int HandSize => _hand.Count;

		public int Mana { get; private set; }
		public float ManaPercent => (float)Mana / MaxMana;
		public event Action<Card> OnCardAdded, OnCardRemoved;

		private readonly List<Card> _hand = new();

		private float _nextManaRegenTime;
		private float _nextCardDraw;

		private void Awake()
		{
			Mana = MaxMana;
			_nextCardDraw = CardDrawInterval;
		}

		private IEnumerator Start()
		{
			yield return new WaitForSeconds(0.1f);

			foreach (var card in StartingHand)
			{
				AddCard(card);
				yield return new WaitForSeconds(0.1f);
			}
		}

#if UNITY_EDITOR
		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.G))
			{
				AddCard(GetRandomCard());
			}
		}
#endif

		private void FixedUpdate()
		{
			if (Time.time > _nextManaRegenTime && Mana < MaxMana)
			{
				Mana++;
				_nextManaRegenTime = Time.time + ManaRegenInterval;
			}

			if (Time.time > _nextCardDraw && HandSize < MaxHandSize)
			{
				_nextCardDraw = Time.time + CardDrawInterval;
				AddCard(GetRandomCard());
			}
		}

		public void AddCard(params Card[] cards)
		{
			foreach (var card in cards)
			{
				if (_hand.Count + 1 >= MaxHandSize)
					return;

				_hand.Add(card);
				OnCardAdded?.Invoke(card);
			}
		}

		public void RemoveCard(Card card, bool supressEvents = false)
		{
			_hand.Remove(card);

			if (!supressEvents)
			{
				OnCardRemoved?.Invoke(card);
			}
		}

		public bool HasCard(Card card)
		{
			return _hand.Contains(card);
		}

		public Card GetAt(int index)
		{
			return _hand[index];
		}

		public void ConsumeMana(int cost)
		{
			Mana = Mathf.Max(0, Mana - cost);
		}

		private Card GetRandomCard()
		{
			int wave = WaveManager.Instance.WaveNumber;
			var pool = DeckPool.Where(x => x.Card.DebutWave <= wave);

			float sum = pool.Sum(x => x.Weight);
			foreach (var entry in pool)
			{
				float weight = entry.Weight;
				float chance = weight / sum;

				if (UnityEngine.Random.value <= chance)
				{
					return entry.Card;
				}
			}

			return pool.GetRandom().Card;
		}
	}
}
