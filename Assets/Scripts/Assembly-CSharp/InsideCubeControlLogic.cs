using System.Collections;
using UnityEngine;

public class InsideCubeControlLogic : TriggerItem
{
	public Rotatable rotExit;

	public Rotatable rotMainStairs;

	public Rotatable rotStairPillar;

	public CharacterLocomotion player;

	public NavBrushComponent[] MainStairNavNodes;

	public NavBrushComponent[] MainStairOccludedNavNodes;

	public ActionSequence closeCubeSequence = new ActionSequence();

	private bool MainStairsOpen = true;

	[Range(0f, 100f)]
	public int test = 42;

	private void Start()
	{
	}

	[TriggerableAction]
	public IEnumerator DoTrigger()
	{
		Trigger();
		return null;
	}

	public override void Trigger()
	{
		StartCoroutine(closeCubeSequence.DoSequence());
	}

	private void Update()
	{
		if (rotStairPillar.currentAngle > 1f && (rotMainStairs.snapping || rotMainStairs.dragging))
		{
			rotMainStairs.ApplyAngle(0f);
		}
		if (rotExit.currentAngle > 1f && (rotMainStairs.snapping || rotMainStairs.dragging))
		{
			rotMainStairs.ApplyAngle(0f);
		}
		if (IsPlayerOnOccludedNodes())
		{
			rotMainStairs.ApplyAngle(0f);
		}
		else if (IsPlayerStairNodes())
		{
			rotMainStairs.ApplyAngle(90f);
		}
		if (rotMainStairs.currentAngle > 1f)
		{
			if (rotExit.snapping || rotExit.dragging)
			{
				rotExit.ApplyAngle(0f);
			}
			if (!MainStairsOpen)
			{
				NavBrushComponent[] mainStairNavNodes = MainStairNavNodes;
				for (int i = 0; i < mainStairNavNodes.Length; i++)
				{
					mainStairNavNodes[i].gameObject.SetActive(value: true);
				}
				mainStairNavNodes = MainStairOccludedNavNodes;
				for (int i = 0; i < mainStairNavNodes.Length; i++)
				{
					mainStairNavNodes[i].gameObject.SetActive(value: false);
				}
				MainStairsOpen = true;
			}
		}
		else
		{
			if (!MainStairsOpen)
			{
				return;
			}
			NavBrushComponent[] mainStairNavNodes = MainStairNavNodes;
			foreach (NavBrushComponent navBrushComponent in mainStairNavNodes)
			{
				if (navBrushComponent.gameObject != null)
				{
					navBrushComponent.gameObject.SetActive(value: false);
				}
			}
			mainStairNavNodes = MainStairOccludedNavNodes;
			foreach (NavBrushComponent navBrushComponent2 in mainStairNavNodes)
			{
				if (navBrushComponent2.gameObject != null)
				{
					navBrushComponent2.gameObject.SetActive(value: true);
				}
			}
			MainStairsOpen = false;
		}
	}

	private bool IsPlayerOnOccludedNodes()
	{
		NavBrushComponent[] mainStairOccludedNavNodes = MainStairOccludedNavNodes;
		foreach (NavBrushComponent navBrushComponent in mainStairOccludedNavNodes)
		{
			if (player.lastValidBrush == navBrushComponent)
			{
				return true;
			}
		}
		return false;
	}

	private bool IsPlayerStairNodes()
	{
		NavBrushComponent[] mainStairNavNodes = MainStairNavNodes;
		foreach (NavBrushComponent navBrushComponent in mainStairNavNodes)
		{
			if (player.lastValidBrush == navBrushComponent)
			{
				return true;
			}
		}
		return false;
	}
}
