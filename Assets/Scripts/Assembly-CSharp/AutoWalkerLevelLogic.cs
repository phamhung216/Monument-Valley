using System.Collections;
using UnityEngine;

public class AutoWalkerLevelLogic : TriggerItem
{
	public AIController walkerA;

	public AIController walkerIntroA;

	public AIController walker2A;

	public AIController walker2B;

	public AIController walker3A;

	public AIController walker3B;

	public AIController walker4A;

	public AIController walker4B;

	public ActionSequence ActivatePlayerPath1 = new ActionSequence();

	public ActionSequence ActivatePlayerPath2 = new ActionSequence();

	public ActionSequence ActivatePlayerPath3 = new ActionSequence();

	public ActionSequence ActivatePlayerPath4 = new ActionSequence();

	public ActionSequence ActivatePlayerPath5 = new ActionSequence();

	public TriggerAllCrows triggerButton;

	private bool complete;

	private int stage = -1;

	private NavBrushComponent[] allBrushes;

	private void Start()
	{
		allBrushes = Object.FindObjectsOfType(typeof(NavBrushComponent)) as NavBrushComponent[];
	}

	[TriggerableAction]
	public IEnumerator TriggerPlayerPath1()
	{
		triggerButton.StopAllCrows();
		StartCoroutine(ActivatePlayerPath1.DoSequence());
		return null;
	}

	[TriggerableAction]
	public IEnumerator TriggerPlayerPath2()
	{
		triggerButton.StopAllCrows();
		StartCoroutine(ActivatePlayerPath2.DoSequence());
		return null;
	}

	[TriggerableAction]
	public IEnumerator TriggerPlayerPath3()
	{
		triggerButton.StopAllCrows();
		StartCoroutine(ActivatePlayerPath3.DoSequence());
		return null;
	}

	[TriggerableAction]
	public IEnumerator TriggerPlayerPath4()
	{
		triggerButton.StopAllCrows();
		StartCoroutine(ActivatePlayerPath4.DoSequence());
		return null;
	}

	[TriggerableAction]
	public IEnumerator TriggerPlayerPath5()
	{
		triggerButton.StopAllCrows();
		StartCoroutine(ActivatePlayerPath5.DoSequence());
		complete = true;
		return null;
	}

	[TriggerableAction]
	public IEnumerator ActivateStage0()
	{
		stage = 0;
		return null;
	}

	[TriggerableAction]
	public IEnumerator ActivateStage1()
	{
		stage = 1;
		return null;
	}

	[TriggerableAction]
	public IEnumerator ActivateStage2()
	{
		stage = 2;
		return null;
	}

	[TriggerableAction]
	public IEnumerator ActivateStage3()
	{
		stage = 3;
		return null;
	}

	[TriggerableAction]
	public IEnumerator ActivateStage4()
	{
		stage = 4;
		return null;
	}

	private void Update()
	{
		if (stage == 0 && walkerIntroA.lastValidBrush == walkerIntroA.PathCompleteNavPoints[0] && !triggerButton.initialState)
		{
			TriggerPlayerPath1();
		}
		if (stage == 1 && walkerA.lastValidBrush == walkerA.PathCompleteNavPoints[0] && !triggerButton.initialState)
		{
			TriggerPlayerPath2();
		}
		if (stage == 2 && walker2A.lastValidBrush == walker2A.PathCompleteNavPoints[0] && walker2B.lastValidBrush == walker2B.PathCompleteNavPoints[0] && !triggerButton.initialState)
		{
			TriggerPlayerPath3();
		}
		if (stage == 3 && walker3A.lastValidBrush == walker3A.PathCompleteNavPoints[0] && walker3B.lastValidBrush == walker3B.PathCompleteNavPoints[0] && !triggerButton.initialState)
		{
			TriggerPlayerPath4();
		}
		if (stage == 4 && !complete && walker4A.lastValidBrush == walker4A.PathCompleteNavPoints[0] && walker4B.lastValidBrush == walker4B.PathCompleteNavPoints[0] && !triggerButton.initialState)
		{
			TriggerPlayerPath5();
		}
		NavBrushComponent[] array = allBrushes;
		foreach (NavBrushComponent navBrushComponent in array)
		{
			if (navBrushComponent.gameObject.activeSelf)
			{
				if (navBrushComponent.transform.position.y < 0f)
				{
					navBrushComponent.gameObject.SetActive(value: false);
				}
			}
			else if (navBrushComponent.transform.position.y > 0f)
			{
				navBrushComponent.gameObject.SetActive(value: true);
			}
		}
	}
}
