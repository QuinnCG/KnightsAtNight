using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Quinn
{
	[RequireComponent(typeof(Light2D))]
	public class LightFlicker : MonoBehaviour
	{
		[SerializeField, Tooltip("The min and max factors of the base light intensity.")]
		private Vector2 FlickerFactor = new(0.5f, 1f);
		[SerializeField]
		private float FlickerFrequency = 1f;

		[Space, SerializeField, Tooltip("For performance.")]
		private int UpdateInterval = 2;

		private Light2D _light;
		private float _defaultIntensity;
		private float _offset;

		private void Awake()
		{
			_light = GetComponent<Light2D>();
			_defaultIntensity = _light.intensity;

			_offset = Random.value;
		}

		private void Update()
		{
			if (Time.frameCount % UpdateInterval != 0) return;

			float t = Mathf.PerlinNoise1D((Time.time + _offset) * FlickerFrequency);
			float a = Mathf.Lerp(_defaultIntensity * FlickerFactor.y, _defaultIntensity * FlickerFactor.x, t);
			_light.intensity = a;
		}
	}
}
