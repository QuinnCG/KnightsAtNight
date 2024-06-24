using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Quinn
{
	[RequireComponent(typeof(Light2D))]
	public class LightFadeOut : MonoBehaviour
	{
		[SerializeField]
		private float Duration = 5f;
		[SerializeField]
		private bool DestroyOnFinish = true;

		private Light2D _light;
		private float _decayRate;

		private void Awake()
		{
			_light = GetComponent<Light2D>();
			_decayRate = _light.intensity / Duration;
		}

		private void Update()
		{
			if (_light.intensity > 0f)
			{
				_light.intensity -= Time.deltaTime * _decayRate;
			}
			else if (DestroyOnFinish)
			{
				Destroy(gameObject);
			}
		}
	}
}
