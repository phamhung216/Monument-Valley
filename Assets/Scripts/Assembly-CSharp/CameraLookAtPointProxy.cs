using System.Collections;
using UnityEngine;

public class CameraLookAtPointProxy : MonoBehaviour
{
	public CameraLookAtPoint target;

	[TriggerableAction(true)]
	public IEnumerator LookAtWithZoomOut()
	{
		if (target != null)
		{
			yield return target.LookAtWithZoomOut();
		}
	}

	[TriggerableAction(true)]
	public IEnumerator LookAtWithZoomToDefault()
	{
		if (target != null)
		{
			yield return LookAtWithZoomToDefault();
		}
	}

	[TriggerableAction(true)]
	public IEnumerator LookAtWithZoomIn()
	{
		if (target != null)
		{
			yield return LookAtWithZoomIn();
		}
	}

	[TriggerableAction]
	public IEnumerator LookAtWithZoomOutImmediate()
	{
		if (target != null)
		{
			return target.LookAtWithZoomOutImmediate();
		}
		return null;
	}
}
