using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Quinn
{
	public class CameraController : SerializedMonoBehaviour
	{
		[OdinSerialize]
		public float PanSpeed { get; set; } = 4f;

		// TODO: Add camera zooming.
		// TODO: Add camera bounds.

		void Update()
		{
			var inputDir = new Vector3()
			{
				x = Input.GetAxisRaw("Horizontal"),
				y = Input.GetAxisRaw("Vertical")
			}.normalized;

			float speed = PanSpeed;
			transform.position += speed * Time.deltaTime * inputDir;
		}
	}
}
