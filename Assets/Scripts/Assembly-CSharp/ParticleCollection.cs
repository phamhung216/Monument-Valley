using UnityEngine;

public class ParticleCollection : MonoBehaviour
{
	public ParticleSystem startParticle;

	public ParticleSystem endParticle;

	public void PlayStartParticle()
	{
		if ((bool)startParticle)
		{
			startParticle.Play();
		}
	}

	public void PlayEndParticle()
	{
		if ((bool)endParticle)
		{
			endParticle.Play();
		}
	}
}
