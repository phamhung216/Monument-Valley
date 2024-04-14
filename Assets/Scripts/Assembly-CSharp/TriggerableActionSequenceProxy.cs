using System.Collections;
using UnityEngine;

public class TriggerableActionSequenceProxy : MonoBehaviour
{
	public TriggerableActionSequence target;

	[TriggerableAction]
	public IEnumerator TriggerActions()
	{
		if (target != null)
		{
			return target.TriggerActions();
		}
		return null;
	}

	[TriggerableAction(true)]
	public IEnumerator RunSequence()
	{
		if (target != null)
		{
			yield return target.RunSequence();
		}
	}
}
