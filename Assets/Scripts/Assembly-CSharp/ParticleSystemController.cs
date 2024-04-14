using System.Collections;
using UnityEngine;

public class ParticleSystemController : MonoBehaviour
{
	[TriggerableAction]
	public IEnumerator StartSequence()
	{
		GetComponent<ParticleSystem>().Play();
		return null;
	}
}
