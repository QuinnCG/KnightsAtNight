using UnityEngine;
using UnityEngine.Tilemaps;

namespace Quinn
{
	public class PathNode
	{
		public Vector3Int Index { get; }
		public PathNode Parent { get; }
		public int Generation { get; }
		public Vector2 Position { get; }
		public Vector3Int DirectionToOrigin
		{
			get
			{
				if (_dirToOrigin.HasValue)
				{
					return _dirToOrigin.Value;
				}
				else
				{
					return new();
				}
			}
		}

		private Vector3Int? _dirToOrigin = null;

		public PathNode(Vector3Int index, PathNode parent, Tilemap map)
		{
			Index = index;
			Parent = parent;

			if (parent == null) Generation = 0;
			else Generation = parent.Generation + 1;

			if (parent != null && !parent._dirToOrigin.HasValue)
			{
				_dirToOrigin = ((Vector3)Index).DirectionTo(Vector3.zero).ToVector3Int();
			}
			else if (parent != null)
			{
				_dirToOrigin = parent._dirToOrigin;
			}

			Position = map.CellToWorld(index) + (Vector3.one * 0.5f);
		}
	}
}
