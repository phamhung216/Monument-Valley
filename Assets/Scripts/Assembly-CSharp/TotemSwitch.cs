using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TotemSwitch : MonoBehaviour
{
	public TotemPole totem;

	public List<NavBrushComponent> brushes;

	public ActionSequence onActions = new ActionSequence();

	public ActionSequence offActions = new ActionSequence();

	[TriggerableAction]
	public IEnumerator DoTrigger()
	{
		bool flag = false;
		if ((bool)totem)
		{
			for (int i = 0; i < brushes.Count; i++)
			{
				if (totem.lastValidBrush == brushes[i])
				{
					flag = true;
				}
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
		return null;
	}

	private void OnDrawGizmos()
	{
		Gizmos.DrawIcon(base.transform.position, "GizmoDistanceSwitch");
		Gizmos.color = Color.blue;
		foreach (NavBrushComponent brush in brushes)
		{
			Gizmos.DrawLine(base.transform.position, brush.transform.position);
		}
	}
}
