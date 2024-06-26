using FMODUnity;
using UnityEngine;

namespace Quinn
{
	public class AudioPlayer : MonoBehaviour
	{
		public void Play(string path)
		{
			RuntimeManager.PlayOneShot(path, transform.position);
		}
	}
}
