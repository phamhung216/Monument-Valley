using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroCrowsLogic : MonoBehaviour
{
	public AIController crowAI;

	public NavBrushComponent doorCloseTriggerTarget;

	public NavBrushComponent idlePositionTarget;

	public Rotatable crowRotator;

	public AxisDragger blockingDragger;

	public ParticleSystem alertParticle;

	private bool idaHasTriggeredPath;

	[TriggerableAction]
	public IEnumerator IdaHasTriggeredPath()
	{
		idaHasTriggeredPath = true;
		return null;
	}

	[TriggerableAction]
	public IEnumerator CrowHasTriggeredPath()
	{
		idaHasTriggeredPath = false;
		return null;
	}

	[TriggerableAction(true)]
	public IEnumerator WalkCrowToTrigger()
	{
		if (crowAI.ForceIdle && CrowDoesntLikePath())
		{
			if ((bool)alertParticle)
			{
				alertParticle.Play();
			}
			yield return new WaitForSeconds(0.5f);
			crowAI.WalkTo(doorCloseTriggerTarget);
			crowAI.ForceIdle = false;
		}
	}

	[TriggerableAction]
	public IEnumerator WalkCrowToIdle()
	{
		crowAI.PathCompleteNavPoints = new List<NavBrushComponent> { idlePositionTarget };
		return null;
	}

	[TriggerableAction]
	public IEnumerator ResetCrow()
	{
		if (crowAI.PathCompleteNavPoints != null)
		{
			crowAI.PathCompleteNavPoints.Clear();
		}
		crowAI.ForceIdle = true;
		return null;
	}

	private bool CrowDoesntLikePath()
	{
		if ((idaHasTriggeredPath || (crowRotator != null && crowRotator.AtMaximum())) && crowAI.lastValidBrush == idlePositionTarget)
		{
			return true;
		}
		return false;
	}
}
