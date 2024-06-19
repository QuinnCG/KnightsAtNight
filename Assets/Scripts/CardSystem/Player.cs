using UnityEngine;

namespace Quinn
{
	public static class Player
	{
		/// <summary>
		/// The position of the mouse in the world.
		/// </summary>
		public static Vector2 MousePos => Camera.main.ScreenToWorldPoint(Input.mousePosition);
	}
}
