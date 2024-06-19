using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UIElements;
using UnityEngine.VFX;

namespace Quinn
{
	public static partial class InstantiationExtensions
	{
		public static GameObject Clone(this GameObject gameObject, Vector2 position, Transform parent = null, string name = null)
		{
			var instance = Object.Instantiate(gameObject, position, Quaternion.identity, parent);
			if (name != null) instance.name = name;

			return instance;
		}
		/// <summary>
		/// Clones the game object and returns the first instance of a component of type T.
		/// </summary>
		public static T Clone<T>(this GameObject gameObject, Vector2 position, Transform parent = null, string name = null)
			where T : MonoBehaviour
		{
			var instance = Clone(gameObject, position, parent, name);
			return instance.GetComponentInChildren<T>();
		}
		/// <summary>
		/// Clones the game object and returns the first instance of a component of type T.
		/// </summary>
		public static T Clone<T>(this GameObject gameObject, Transform parent = null, string name = null)
			where T : MonoBehaviour
		{
			var instance = Object.Instantiate(gameObject, parent);
			if (name != null) instance.name = name;

			return instance.GetComponentInChildren<T>();
		}

		public static GameObject Clone(this string key, Vector2 position, Transform parent = null, string name = null)
		{
			var instance = Addressables.InstantiateAsync(key, position, Quaternion.identity, parent).WaitForCompletion();
			if (name != null) instance.name = name;

			return instance;
		}
		/// <summary>
		/// Clones the game object and returns the first instance of a component of type T.
		/// </summary>
		public static T Clone<T>(this string key, Vector2 position, Transform parent = null, string name = null)
			where T : MonoBehaviour
		{
			var instance = Clone(key, position, parent, name);
			return instance.GetComponentInChildren<T>();
		}
		public static GameObject Clone(this string key, Transform parent = null, string name = null)
		{
			var instance = Addressables.InstantiateAsync(key, parent).WaitForCompletion();
			if (name != null) instance.name = name;

			return instance;
		}

		public static GameObject Clone(this VisualEffectAsset vfx, Vector2 position, Transform parent = null, float lifespan = float.PositiveInfinity)
		{
			var instance = new GameObject(vfx.name);
			instance.transform.position = position;
			instance.transform.parent = parent;

			var v = instance.AddComponent<VisualEffect>();
			v.visualEffectAsset = vfx;
			v.Play();

			if (lifespan is >= 0f and < float.PositiveInfinity)
			{
				Object.Destroy(instance, lifespan);
			}

			return instance;
		}
	}
}
