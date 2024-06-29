using System.Text;
using UnityEngine;

namespace Quinn
{
	public static class Logger
	{
		public static void LogGroup(string header, string color, params string[] parts)
		{
			var builder = new StringBuilder();
			builder.Append($"{header}: [ ".Bold().Color(color));

			for (int i = 0; i < parts.Length; i++)
			{
				if (i == parts.Length - 1)
				{
					builder.Append(parts[i]);
				}
				else
				{
					builder.Append($"{parts[i]}, ");
				}
			}

			builder.AppendLine(" ]".Bold().Color(color));

			Debug.Log(builder.ToString());
		}
	}
}