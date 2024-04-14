using System.Collections;
using UnityEngine;

public class Lamp : TriggerItem
{
	public bool lit;

	public GameObject onObject;

	public GameObject offObject;

	public ParticleSystem particle;

	[TriggerableAction]
	public IEnumerator TurnOn()
	{
		lit = true;
		onObject.SetActive(value: true);
		offObject.SetActive(value: false);
		if ((bool)particle)
		{
			particle.Play();
		}
		return null;
	}

	[TriggerableAction]
	public IEnumerator TurnOff()
	{
		lit = false;
		onObject.SetActive(value: false);
		offObject.SetActive(value: true);
		if ((bool)particle)
		{
			particle.Stop();
		}
		return null;
	}
}
