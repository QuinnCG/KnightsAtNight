using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Quinn
{
	public class PathManager : MonoBehaviour
	{
		public static PathManager Instance { get; private set; }

		[SerializeField, Required]
		private Tilemap PathMap;

		[SerializeField, Required]
		private Tile PathTile;

		[SerializeField]
		private Vector2Int InitialSegmentLength = new(5, 6), SegmentLength = new(1, 3);

		[SerializeField]
		private int MaxPathGeneration = 3;

		[SerializeField]
		private float PathTurnChance = 0.5f;

		[SerializeField, Range(0f, 0.6f)]
		private float PathBranchChance = 0.2f;

		[SerializeField]
		private int MaxGenerationToBranch = 4;

		[Tooltip("Nodes of this generation will act as spawn points. This should be higher than MaxGenerationToBranch.")]
		[SerializeField, ValidateInput("@SpawnGeneration > MaxGenerationToBranch")]
		private int SpawnGeneration = 10;

		[Tooltip("Generating after this generation is spread over multiple frames for performance. This should be higher than SpawnGeneration.")]
		[SerializeField, ValidateInput("@InstantlyGenerateUpToGen > SpawnGeneration")]
		private int InstantlyGenerateUpToGen = 15;

		public IEnumerable<PathNode> PathSpawns => _spawns;

		private readonly List<PathNode> _open = new();
		private PathNode _origin;
		private readonly List<PathNode> _spawns = new();

		private void Awake()
		{
			Instance = this;

			// 0, 0.
			_origin = new PathNode(new(), null, PathMap);

			// Initial open nodes.
			_open.Add(CreateNode(_origin, Vector3Int.up));
			_open.Add(CreateNode(_origin, Vector3Int.down));
			_open.Add(CreateNode(_origin, Vector3Int.left));
			_open.Add(CreateNode(_origin, Vector3Int.right));

			// Loop open list.
			var thresholdNodes = new List<PathNode>();

			while (_open.Count > 0)
			{
				var newNodes = new List<PathNode>();

				foreach (var parent in _open)
				{
					// Only generate up to LowerPriorityGenerationTheshold.
					// The rest is deferred to Start().
					if (parent.Generation < InstantlyGenerateUpToGen)
					{
						var dir = GetRandomDirection(parent);
						var node = CreateNode(parent, dir);

						newNodes.Add(node);
						if (node.Generation == SpawnGeneration)
						{
							_spawns.Add(node);
						}

						if (parent.Generation <= MaxGenerationToBranch && Random.value < PathBranchChance)
						{
							newNodes.Add(parent);
						}
					}
					else
					{
						thresholdNodes.Add(parent);
					}
				}

				_open.Clear();
				_open.AddRange(newNodes);
			}

			_open.AddRange(thresholdNodes);
		}

		private IEnumerator Start()
		{
			while (_open.Count > 0)
			{
				var newNodes = new List<PathNode>();

				foreach (var parent in _open)
				{
					// Generate remainig nodes up to max generation.
					if (parent.Generation < MaxPathGeneration)
					{
						var dir = GetRandomDirection(parent);
						var node = CreateNode(parent, dir);

						newNodes.Add(node);
						if (node.Generation == SpawnGeneration)
						{
							_spawns.Add(node);
						}

						if (parent.Generation <= MaxGenerationToBranch && Random.value < PathBranchChance)
						{
							newNodes.Add(parent);
						}
					}

					yield return null;
				}

				_open.Clear();
				_open.AddRange(newNodes);
			}
		}

		private Vector3Int GetRandomDirection(PathNode parent)
		{
			Vector2 parentPos = (Vector3)parent.Index;
			Vector2 grandparentPos = (Vector3)parent.Parent.Index;

			Vector3Int toOrigin = parent.DirectionToOrigin;
			Vector3Int toGrandparent = parentPos.DirectionTo(grandparentPos)
				.ToCardinal()
				.ToVector3Int();

			var directions = new HashSet<Vector3Int>
			{
				Vector3Int.up,
				Vector3Int.down,
				Vector3Int.left,
				Vector3Int.right,
			};
			directions.Remove(toGrandparent);
			directions.Remove(toOrigin);

			if (Random.value < PathTurnChance)
			{
				directions.Remove(-toGrandparent);
			}

			return directions.GetRandom();
		}

		private PathNode CreateNode(PathNode parent, Vector3Int dir)
		{
			float length;
			if (parent.Generation == 0)
			{
				length = InitialSegmentLength.GetRandom(false);
			}
			else
			{
				length = SegmentLength.GetRandom(false);
			}

			Vector3 pos = (Vector3)dir * length;
			pos += (Vector3)parent.Index;

			var index = new Vector3Int(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y));
			var node = new PathNode(index, parent, PathMap);

			CreatePath(parent, node);
			return node;
		}

		private void CreatePath(PathNode a, PathNode b)
		{
			static Color GetColor(Vector3Int v)
			{
				if (Mathf.Abs(v.x) > Mathf.Abs(v.y))
				{
					return Color.Lerp(Color.red, Color.magenta, v.x);
				}
				else
				{
					return Color.Lerp(Color.green, Color.yellow, v.y);
				}
			}

			SetFill(a.Index, b.Index);

			var half = Vector3.one * 0.5f;
			var start = PathMap.CellToWorld(a.Index) + half;
			var end = PathMap.CellToWorld(b.Index) + half;

			var color = GetColor(b.DirectionToOrigin);
			color.a = 1f - ((float)a.Generation / MaxPathGeneration);
			Debug.DrawLine(start, end, color, float.PositiveInfinity);
		}

		private void SetFill(Vector3Int start, Vector3Int end)
		{
			var lower = new Vector3Int()
			{
				x = Mathf.Min(start.x, end.x),
				y = Mathf.Min(start.y, end.y)
			};
			var upper = new Vector3Int()
			{
				x = Mathf.Max(start.x, end.x),
				y = Mathf.Max(start.y, end.y)
			};

			for (int x = lower.x; x <= upper.x; x++)
			{
				for (int y = lower.y; y <= upper.y; y++)
				{
					PathMap.SetTile(new(x, y), PathTile);
				}
			}
		}
	}
}
