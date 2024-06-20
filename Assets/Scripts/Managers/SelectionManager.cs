using Quinn.AI;
using Quinn.UI;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Quinn
{
	[RequireComponent(typeof(UIDocument))]
	public class SelectionManager : MonoBehaviour
	{
		public static SelectionManager Instance { get; private set; }

		public bool IsSelecting => _box != null;
		public bool AnySelected => _selected.Count > 0;

		private readonly HashSet<UnitAI> _selected = new();
		private readonly Dictionary<UnitAI, GameObject> _outlines = new();

		private VisualElement _root;
		private IPanel _panel;
		private VisualElement _box;

		private Vector2 _upperBound, _lowerBound;
		private Vector2 _initialDownPos;

		private void Awake()
		{
			Instance = this;

			_root = GetComponent<UIDocument>().rootVisualElement;
			_panel = _root.panel;
			UnityEngine.Cursor.lockState = CursorLockMode.Confined;
		}

		private void Update()
		{
			if (_box == null)
			{
				if (Input.GetMouseButtonDown(0) && !CardLayoutManager.Instance.IsHoveringOnCard)
				{
					_box = CreateSelectionBox();
					_initialDownPos = Input.mousePosition;

					// TODO: Should this be used instead of referencing CardLayoutManager?
					//_panel.Pick(RuntimePanelUtils.ScreenToPanel(_panel, Input.mousePosition));
				}
			}
			else
			{
				UpdateSelectionBox();
				UpdateSelection();

				if (Input.GetMouseButtonUp(0))
				{
					_box.RemoveFromHierarchy();
					_box = null;
				}
			}

			if (_selected.Count > 0 && Input.GetMouseButtonDown(1))
			{
				Vector2 cursorPos = ScreenToWorld(Input.mousePosition);

				// The MoveOrder prefab will handle its own lifecycle.
				"MoveOrder.prefab".Clone(cursorPos);

				foreach (var unit in _selected)
				{
					unit.Order(cursorPos);
				}
			}

			if (Input.GetKeyDown(KeyCode.Escape) && !GameManager.Instance.IsPaused)
			{
				foreach (var unit in _selected)
				{
					SetOutline(unit, false);
				}

				_selected.Clear();
			}
		}

		private VisualElement CreateSelectionBox()
		{
			var box = new VisualElement();
			box.AddToClassList("selection-box");

			_root.Add(box);
			return box;
		}

		private void UpdateSelectionBox()
		{
			Vector2 cursorPos = Input.mousePosition;

			_lowerBound = new(Mathf.Min(_initialDownPos.x, cursorPos.x), Mathf.Min(_initialDownPos.y, cursorPos.y));
			_upperBound = new(Mathf.Max(_initialDownPos.x, cursorPos.x), Mathf.Max(_initialDownPos.y, cursorPos.y));

			var lower = ScreenToUI(_lowerBound);
			var upper = ScreenToUI(_upperBound);

			Vector2 boxPos = lower;
			Vector2 boxSize = upper - lower;

			_box.style.left = boxPos.x;
			_box.style.bottom = boxPos.y;

			_box.style.width = boxSize.x;
			_box.style.height = boxSize.y;
		}

		private Vector2 ScreenToUI(Vector2 pos)
		{
			return RuntimePanelUtils.ScreenToPanel(_panel, pos);
		}

		public Vector2 ScreenToWorld(Vector2 pos)
		{
			return Camera.main.ScreenToWorldPoint(pos);
		}

		private void UpdateSelection()
		{
			// TODO: Profile the performance for this method.

			var lower = ScreenToWorld(_lowerBound);
			var upper = ScreenToWorld(_upperBound);

			var delta = upper - lower;

			Vector2 center = lower + (delta / 2f);
			Vector2 size = delta;

			var colliders = Physics2D.OverlapBoxAll(center, size, 0f, CollisionUtility.FriendlyMask);

			// Get all units in selection box, currently.
			var inSelectionBox = new HashSet<UnitAI>();
			foreach (var collider in colliders)
			{
				if (collider.TryGetComponent(out UnitAI unit))
				{
					inSelectionBox.Add(unit);
				}
			}

			// Figure which units are missing from current selection box.
			var missingFromSelectionBox = new HashSet<UnitAI>();
			foreach (var unit in _selected)
			{
				if (!inSelectionBox.Contains(unit))
				{
					missingFromSelectionBox.Add(unit);
				}
			}

			// Remove missing units and disable their outlines.
			foreach (var unit in missingFromSelectionBox)
			{
				_selected.Remove(unit);
				SetOutline(unit, false);
			}

			// Save units currently in selection box and enable their outlines.
			foreach (var unit in inSelectionBox)
			{
				if (!_selected.Contains(unit))
				{
					_selected.Add(unit);
					SetOutline(unit, true);
				}
			}
		}

		private void SetOutline(UnitAI unit, bool selected)
		{
			if (!selected && _outlines.TryGetValue(unit, out GameObject outline))
			{
				Destroy(outline);
				_outlines.Remove(unit);
			}
			else if (selected && !_outlines.ContainsKey(unit))
			{
				var instance = "Selection.prefab".Clone(unit.transform);
				_outlines.Add(unit, instance);
			}
		}
	}
}
