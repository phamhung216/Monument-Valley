using System;
using System.Collections;
using UnityEngine;

public class AutoInterp : Interpolation
{
	private float _snapStartTime;

	[HideInInspector]
	public bool snapping;

	[HideInInspector]
	public bool reverting;

	public float snapDuration = 2f;

	public float GetNormalizedAmount(float startTime)
	{
		if (snapDuration == 0f)
		{
			return 1f;
		}
		return Mathf.Clamp((Time.time - startTime) / snapDuration, 0f, 1f);
	}

	public void Interp()
	{
		try
		{
			StartCoroutine(DoInterp());
		}
		catch (Exception ex)
		{
			D.Error("Interp " + base.name + " Coroutine threw exception " + ex, base.gameObject);
		}
	}

	public void ReverseInterp()
	{
		try
		{
			StartCoroutine(DoReverseInterp());
		}
		catch (Exception ex)
		{
			D.Error("Interp " + base.name + " Coroutine threw exception " + ex, base.gameObject);
		}
	}

	[TriggerableAction(true)]
	public IEnumerator DoInterp()
	{
		if (TriggerAction.FastForward)
		{
			interpAmount = 1f;
			yield break;
		}
		snapping = true;
		reverting = false;
		_snapStartTime = Time.time;
		while (snapping)
		{
			yield return null;
		}
	}

	[TriggerableAction(true)]
	public IEnumerator DoReverseInterp()
	{
		if (TriggerAction.FastForward)
		{
			interpAmount = 0f;
			yield break;
		}
		snapping = false;
		reverting = true;
		_snapStartTime = Time.time;
		while (reverting)
		{
			yield return null;
		}
	}

	private void Update()
	{
		if (snapping)
		{
			interpAmount = GetNormalizedAmount(_snapStartTime);
			if (interpAmount == 1f)
			{
				snapping = false;
			}
		}
		else if (reverting)
		{
			interpAmount = 1f - GetNormalizedAmount(_snapStartTime);
			if (interpAmount == 0f)
			{
				reverting = false;
			}
		}
	}
}
