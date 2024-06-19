using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;

namespace Quinn
{
	public class Draw : MonoBehaviour
	{
		abstract class Command
		{
			public Color Color { get; }

			public Command(Color color)
			{
				Color = color;
			}

			public abstract void Draw();
		}
		class RectCommand : Command
		{
			public Vector2 Center { get; }
			public Vector2 Size { get; }

			public RectCommand(Vector2 center, Vector2 size, Color color)
				: base(color)
			{
				Center = center;
				Size = size;
			}

			public override void Draw()
			{
				Gizmos.DrawWireCube(Center, Size);
			}
		}
		class CircleCommand : Command
		{
			public Vector2 Center { get; }
			public float Radius { get; }

			public CircleCommand(Vector2 center, float radius, Color color)
				: base(color)
			{
				Center = center;
				Radius = radius;
			}

			public override void Draw()
			{
				Gizmos.DrawWireSphere(Center, Radius);
			}
		}
		class LineCommand : Command
		{
			public Vector2 Start { get; }
			public Vector2 End { get; }

			public LineCommand(Vector2 start, Vector2 end, Color color)
				: base(color)
			{
				Start = start;
				End = end;
			}

			public override void Draw()
			{
				Gizmos.DrawLine(Start, End);
			}
		}
		class StripCommand : Command
		{
			public Vector2[] Points { get; }
			public bool Loop { get; }

			public StripCommand(Color color, bool loop, params Vector2[] points)
				: base(color)
			{
				Points = points;
				Loop = loop;
			}

			public override void Draw()
			{
				var points = new System.ReadOnlySpan<Vector3>(Points.Select(x => new Vector3(x.x, x.y)).ToArray());
				Gizmos.DrawLineStrip(points, Loop);
			}
		}

		private static Draw _instance;
		private readonly List<Command> _commands = new();

		private void Awake()
		{
			_instance = this;
		}

		public static void Rect(Vector2 center, Vector2 size, Color color)
		{
			Submit(new RectCommand(center, size, color));
		}

		public static void Circle(Vector2 center, float radius, Color color)
		{
			Submit(new CircleCommand(center, radius, color));
		}

		public static void Line(Vector2 start, Vector2 end, Color color)
		{
			Submit(new LineCommand(start, end, color));
		}

		public static void Strip(Color color, bool loop, params Vector2[] points)
		{
			Submit(new StripCommand(color, loop, points));
		}

		[Conditional("UNITY_EDITOR")]
		private static void Submit(Command command)
		{
			_instance._commands.Add(command);
		}

		private void OnDrawGizmos()
		{
			while (_commands.Count > 0)
			{
				var cmd = _commands[^1];
				_commands.RemoveAt(_commands.Count - 1);

				Gizmos.color = cmd.Color;
				cmd.Draw();
			}
		}
	}
}
