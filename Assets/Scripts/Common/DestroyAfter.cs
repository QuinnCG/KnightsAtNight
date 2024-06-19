using UnityEngine;

namespace Quinn.Common
{
	public class DestroyAfter : MonoBehaviour
	{
		[SerializeField]
		private float Lifespan = 5f;

		private void Start()
		{
			Destroy(gameObject, Lifespan);
		}
	}
}
