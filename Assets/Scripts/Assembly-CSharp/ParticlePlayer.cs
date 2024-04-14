using System.Collections;
using UnityEngine;

public class ParticlePlayer : MonoBehaviour
{
	public ParticleSystem target;

	public WaterSection waterSection;

	[TriggerableAction]
	public IEnumerator PlayParticle()
	{
		if (!TriggerAction.FastForward)
		{
			target.Play();
		}
		return null;
	}

	[TriggerableAction]
	public IEnumerator StopParticle()
	{
		target.Stop();
		return null;
	}

	private void Update()
	{
		if (!waterSection)
		{
			return;
		}
		if (target.isPlaying)
		{
			if (!waterSection.hasInFlow())
			{
				target.Stop();
			}
		}
		else if (waterSection.hasInFlow())
		{
			target.Play();
		}
	}
}
