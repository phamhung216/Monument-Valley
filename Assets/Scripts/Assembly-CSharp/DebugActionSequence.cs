using System.Collections;
using UnityEngine;

public class DebugActionSequence : MonoBehaviour
{
	[TriggerableAction]
	public IEnumerator BreakHere()
	{
		return null;
	}

	[TriggerableAction]
	public IEnumerator DebugLog()
	{
		return null;
	}
}
