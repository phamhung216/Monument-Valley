using System;
using UnityEngine;

public class CombinationLock : TriggerItem
{
	public Rotatable[] rotators;

	public float[] solution;

	[HideInInspector]
	public bool solved;

	public ActionSequence actions = new ActionSequence();

	private void Update()
	{
		if (solved)
		{
			return;
		}
		solved = true;
		for (int i = 0; i < rotators.Length; i++)
		{
			if (Mathf.Abs(Mathf.DeltaAngle(rotators[i].currentAngle, solution[i])) > 1f)
			{
				solved = false;
			}
		}
		if (solved)
		{
			try
			{
				StartCoroutine(actions.DoSequence());
			}
			catch (Exception)
			{
				D.Error("Trigger " + base.name + " Coroutine threw exception", base.gameObject);
			}
		}
	}
}
