using FMODUnity;
using Unity.Cinemachine;
using UnityEngine;

namespace Quinn
{
	public class CameraController : MonoBehaviour
	{
		[SerializeField]
		private float BasePanSpeed = 4f;
		[SerializeField]
		private AnimationCurve ZoomSpeedFactor;

		[Space, SerializeField]
		private float ZoomSpeed = 5f;
		[SerializeField]
		private float MinOrthoScale = 5f, MaxOrthoScale = 8f;
		[SerializeField]
		private float ZoomDamping = 0.3f;

		[Space, SerializeField]
		private Vector2 BoundsSize = new(16, 12f);

		private float _zoomVel;
		private float _targetZoomScale;
		
		private void Start()
		{
			var vcam = CinemachineBrain.GetActiveBrain(0).ActiveVirtualCamera as CinemachineCamera;
			_targetZoomScale = vcam.Lens.OrthographicSize;
		}

		void Update()
		{
			var vcam = CinemachineBrain.GetActiveBrain(0).ActiveVirtualCamera as CinemachineCamera;

			var inputDir = new Vector3()
			{
				x = Input.GetAxisRaw("Horizontal"),
				y = Input.GetAxisRaw("Vertical")
			}.normalized;

			float zoomPercent = (vcam.Lens.OrthographicSize - MinOrthoScale) / (MaxOrthoScale - MinOrthoScale);
			zoomPercent = 1f - Mathf.Clamp01(zoomPercent);

			RuntimeManager.StudioSystem.setParameterByName("zoom", zoomPercent);

			float speed = BasePanSpeed * ZoomSpeedFactor.Evaluate(zoomPercent);
			transform.position += speed * Time.deltaTime * inputDir;

			var half = BoundsSize / 2f;
			var pos = new Vector2()
			{
				x = Mathf.Clamp(transform.position.x, -half.x, half.x),
				y = Mathf.Clamp(transform.position.y, -half.y, half.y)
			};
			transform.position = pos;
			
			float deltaScroll = Input.GetAxisRaw("Mouse ScrollWheel");
			_targetZoomScale = Mathf.Clamp(_targetZoomScale - (deltaScroll * ZoomSpeed), MinOrthoScale, MaxOrthoScale);

			vcam.Lens.OrthographicSize = Mathf.SmoothDamp(vcam.Lens.OrthographicSize, _targetZoomScale, ref _zoomVel, ZoomDamping);
		}
	}
}
