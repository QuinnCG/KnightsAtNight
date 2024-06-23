using Quinn.CardSystem;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

namespace Quinn.UI
{
	[RequireComponent(typeof(UIDocument))]
	[RequireComponent(typeof(CardManager))]
	public class CardLayoutManager : MonoBehaviour
	{
		public static CardLayoutManager Instance { get; private set; }

		[SerializeField]
		private VisualTreeAsset CardPrefab;

		[SerializeField]
		private float CardWidth = 300f, CardHeight = 450f;

		[SerializeField]
		private float CardGap = 30f, CardYOffset = 50f, CardCurvePeak = 50f;

		[SerializeField]
		private float CardRotation = 15f;

		[SerializeField]
		private float EdgeCardDrop = 15f;

		[SerializeField]
		private float DraggedCardOpacity = 0.4f;

		[SerializeField, Tooltip("The lower percentage of the screen, that casting a card will instead cancel, in.")]
		private float CancelCastScreenZone = 0.1f;

		[SerializeField]
		private float CardHoverScaleFactor = 1.2f;

		[SerializeField]
		private Vector2 CardDrawSpawn = new(20f, -200f);

		[SerializeField]
		private float CardHoveredYOffset = 6f;

		public bool IsHoveringOnCard => _raiseCards;

		private readonly List<(Card card, VisualElement element)> _cards = new();

		private VisualElement _root;
		private IPanel _panel;
		private CardManager _manager;

		private VisualElement _dragged;
		private bool _raiseCards;

		private void Awake()
		{
			Instance = this;

			_root = GetComponent<UIDocument>().rootVisualElement;
			_panel = _root.panel;

			_manager = GetComponent<CardManager>();
			_manager.OnCardAdded += OnCardAdded;
			_manager.OnCardRemoved += OnCardRemoved;
		}

		private void Update()
		{
			UpdateLayout();

			if (_dragged != null)
			{
				Vector2 pos = RuntimePanelUtils.ScreenToPanel(_panel, Input.mousePosition);
				float screenHeight = _root.resolvedStyle.height;

				_dragged.style.left = pos.x - (CardWidth / 2f);
				_dragged.style.top = screenHeight - pos.y - (CardHeight / 2f);

				// Cast card.
				if (Input.GetMouseButtonUp(0))
				{
					if (GetCursorScreenYNorm() < CancelCastScreenZone)
					{
						CancelCard();
					}
					else
					{
						var card = GetCardFromElement(_dragged);
						if (card != null && _manager.Mana >= card.Cost)
						{
							_manager.ConsumeMana(card.Cost);
							card.Cast();

							// Handle removing the card element ourselves, instead of through the OnCardRemoved callback.
							_manager.RemoveCard(card, true);
							_root.Remove(_dragged);
							RemoveTupleEntry(_dragged);
						}

						CancelCard();
					}
				}
				// Cancel card.
				else if (Input.GetMouseButtonDown(1))
				{
					CancelCard();
				}
			}
		}

		private void OnCardAdded(Card card)
		{
			var instance = CardPrefab.Instantiate()[0];
			instance.RemoveFromHierarchy();

			instance.style.left = CardDrawSpawn.x;
			instance.style.top = _root.resolvedStyle.height - CardDrawSpawn.y;

			InitializeCard(instance, card);
			_cards.Add((card, instance));

			_root.Add(instance);
		}

		private void OnCardRemoved(Card card)
		{
			var element = GetElementFromCard(card);

			if (element != null)
			{
				_root.Remove(element);
			}
		}

		private void InitializeCard(VisualElement element, Card card)
		{
			element.Q<Label>("title").text = card.Title;
			element.Q<Label>("cost").text = card.Cost.ToString();
			element.Q<Label>("description").text = card.Description;
			element.Q<VisualElement>("art").style.backgroundImage = new StyleBackground(card.Art);

			element.RegisterCallback<PointerDownEvent>(evnt =>
			{
				// Ignore right and middle mouse presses.
				if (evnt.button != 0) return;

				// Can't afford, so we can't drag.
				if (card.Cost > _manager.Mana) return;

				// Incase something breaks.
				_dragged = null;

				_dragged = element;
				_dragged.style.opacity = DraggedCardOpacity;

				SetLarge(element, false);
			});
			element.RegisterCallback<MouseEnterEvent>(_ =>
			{
				if (!SelectionManager.Instance.IsSelecting)
				{
					_raiseCards = true;
					SetLarge(element, true);
				}
			});
			element.RegisterCallback<MouseLeaveEvent>(_ =>
			{
				if (!SelectionManager.Instance.IsSelecting)
				{
					_raiseCards = false;
					SetLarge(element, false);
				}
			});
		}

		private void UpdateLayout()
		{
			float screenHeight = _root.resolvedStyle.height;
			float screenWidth = _root.resolvedStyle.width;

			int cardCount = _cards.Count;
			if (_dragged != null) cardCount--;

			float cardWidth = CardWidth;
			float cardGap = _raiseCards ? 0f : CardGap;
			float totalWidth = (cardCount * cardWidth) + ((cardCount - 1) * cardGap);

			float xOffset = (screenWidth / 2f) - (totalWidth / 2f);
			float yOffset = (_raiseCards && (_dragged == null)) ? 0f : CardYOffset;

			int i = 0;
			foreach (var (card, element) in _cards)
			{
				if (element == _dragged) continue;

				element.style.opacity = 1f;
				element.style.rotate = new Rotate(0f);
				float curveYOffset = 0f;

				SetActive(element, _manager.Mana >= card.Cost);

				if (!_raiseCards)
				{
					int c = Mathf.Max(1, cardCount - 1);
					float coord = (float)i / c;

					coord = (coord * 2f) - 1f;

					float absCoord = Mathf.Abs(coord);

					float y = Mathf.Sin(absCoord);
					y = (y + 1f) / 2f;

					curveYOffset = Mathf.Lerp(CardCurvePeak, 0f, y);
					if (absCoord > 0.5f)
					{
						curveYOffset -= absCoord * EdgeCardDrop;
					}

					if (cardCount > 1)
					{
						float rot = Mathf.Lerp(CardRotation / 2f, CardRotation, (float)cardCount / _manager.MaxHandSize);
						float rotMag = Mathf.Lerp(0f, rot, absCoord);
						element.style.rotate = new Rotate(rotMag * Mathf.Sign(coord));
					}
				}

				element.style.top = screenHeight - CardHeight - yOffset - curveYOffset;
				element.style.left = (i * (cardWidth + cardGap)) + xOffset;

				i++;
			}
		}

		private Card GetCardFromElement(VisualElement element)
		{
			foreach (var (c, e) in _cards)
			{
				if (e == element)
				{
					return c;
				}
			}

			return null;
		}

		private VisualElement GetElementFromCard(Card card)
		{
			foreach (var (c, e) in _cards)
			{
				if (c == card)
				{
					return e;
				}
			}

			return null;
		}

		private void RemoveTupleEntry(VisualElement element)
		{
			foreach (var (c, e) in _cards)
			{
				if (e == element)
				{
					_cards.Remove((c, e));
					return;
				}
			}
		}

		private void CancelCard()
		{
			_dragged.style.opacity = 1f;
			_dragged = null;
			_raiseCards = false;
		}

		// A 0-1 value representings what percent up the screen the cursor is from the bottom.
		private float GetCursorScreenYNorm()
		{
			return Input.mousePosition.y / Screen.height;
		}

		private void SetLarge(VisualElement element, bool large)
		{
			element.transform.position = new Vector2(0f, large ? -CardHoveredYOffset : 0f);
			element.transform.scale = Vector2.one * (large ? CardHoverScaleFactor : 1f);
			element.BringToFront();
		}

		private void SetActive(VisualElement element, bool active)
		{
			element.style.opacity = active ? 1f : 0.5f;
		}
	}
}
