using System.Collections;
using Fabric;
using UnityEngine;

public class PenroseStingLogic : MonoBehaviour
{
	public Rotatable penroseRotator;

	public bool bridgeLowered;

	public bool stingPlayed;

	public string stingEvent;

	[TriggerableAction]
	public IEnumerator BridgeLowered()
	{
		bridgeLowered = true;
		return null;
	}

	private void Update()
	{
		if (bridgeLowered && penroseRotator.isStationary && Mathf.Abs(Mathf.DeltaAngle(penroseRotator.currentAngle, 180f)) == 0f && !stingPlayed)
		{
			stingPlayed = true;
			if ((bool)EventManager.Instance)
			{
				EventManager.Instance.PostEvent(stingEvent, EventAction.PlaySound);
			}
		}
	}
}
