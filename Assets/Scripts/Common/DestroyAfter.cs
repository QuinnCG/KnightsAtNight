using UnityEngine;

namespace Quinn
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
