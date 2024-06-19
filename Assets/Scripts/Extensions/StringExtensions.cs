namespace Quinn
{
	public static class StringExtensions
	{
		public static string Bold(this string s)
		{
			return $"<b>{s}</b>";
		}

		public static string Italic(this string s)
		{
			return $"<i>{s}</i>";
		}

		public static string Color(this string s, string color)
		{
			return $"<color={color}>{s}</color>";
		}
	}
}
