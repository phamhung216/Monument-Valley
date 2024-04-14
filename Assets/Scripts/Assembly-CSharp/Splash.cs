using UnityEngine;

public class Splash : MonoBehaviour
{
	public ParticleSystem[] particleSystems;

	public void Play()
	{
		ParticleSystem[] array = particleSystems;
		foreach (ParticleSystem obj in array)
		{
			obj.Stop();
			obj.Clear();
			obj.Play();
		}
	}
}
