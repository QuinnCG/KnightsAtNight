using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Quinn
{
	public static class CollectionExtensions
	{
		public static T Highest<T>(this IEnumerable<T> collection, Func<T, float> get)
		{
			T best = default;
			float bValue = 0f;

			foreach (var item in collection)
			{
				float value = get(item);
				if (value >= bValue)
				{
					best = item;
					bValue = value;
				}
			}

			return best;
		}

		public static T Lowest<T>(this IEnumerable<T> collection, Func<T, float> get)
		{
			T best = default;
			float bValue = float.PositiveInfinity;

			foreach (var item in collection)
			{
				float value = get(item);
				if (value <= bValue)
				{
					best = item;
					bValue = value;
				}
			}

			return best;
		}

		public static T GetRandom<T>(this IEnumerable<T> collection)
		{
			return collection.ElementAt(UnityEngine.Random.Range(0, collection.Count()));
		}

		public static string GetString<T>(this IEnumerable<T> collection)
		{
			return GetString(collection, x => $"<{x}>");
		}
		public static string GetString<T>(this IEnumerable<T> collection, Func<T, string> toString)
		{
			int count = collection.Count();

			var builder = new StringBuilder();
			builder.Append("[ ");

			int i = 0;
			foreach (var item in collection)
			{
				string seperator = (i < count) ? ", " : string.Empty;

				builder.Append(toString(item));
				builder.Append(seperator);

				i++;
			}

			builder.AppendLine(" ]");
			return builder.ToString();
		}
	}
}
