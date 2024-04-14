using System;
using System.Collections;
using UnityEngine;

public class DistanceSwitch : MonoBehaviour
{
	public Transform targetA;

	public Transform targetB;

	public float distanceTheshold = 0.2f;

	public ProximityTrigger proximityTrigger;

	public bool forceOn;

	public ActionSequence onActions = new ActionSequence();

	public ActionSequence offActions = new ActionSequence();

	[TriggerableAction]
	public IEnumerator ForceOn()
	{
		forceOn = true;
		return null;
	}

	[TriggerableAction]
	public IEnumerator ForceOff()
	{
		forceOn = false;
		return null;
	}

	[TriggerableAction]
	public IEnumerator DoTrigger()
	{
		bool flag = forceOn;
		if (!forceOn)
		{
			if ((bool)targetA && (bool)targetB)
			{
				if (Vector3.Distance(targetA.position, targetB.position) <= distanceTheshold)
				{
					flag = flag || true;
				}
			}
			else if ((bool)proximityTrigger)
			{
				flag |= proximityTrigger.triggered;
			}
		}
		if (flag)
		{
			try
			{
				StartCoroutine(onActions.DoSequence());
			}
			catch (Exception ex)
			{
				D.Error("DistanceSwitch " + base.name + " Coroutine threw exception " + ex, base.gameObject);
			}
		}
		else
		{
			try
			{
				StartCoroutine(offActions.DoSequence());
			}
			catch (Exception ex2)
			{
				D.Error("DistanceSwitch " + base.name + " Coroutine threw exception " + ex2, base.gameObject);
			}
		}
		flag = false;
		return null;
	}

	private void OnDrawGizmos()
	{
		Gizmos.DrawIcon(base.transform.position, "GizmoDistanceSwitch");
	}
}
