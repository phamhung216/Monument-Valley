using System.Collections;
using UnityEngine;

public class StatueLogic : MonoBehaviour
{
	public Rotatable statueA;

	public Rotatable statueB;

	public ActionSequence statueMatchActions;

	private void Awake()
	{
		base.enabled = false;
	}

	[TriggerableAction]
	public IEnumerator Enable()
	{
		base.enabled = true;
		return null;
	}

	[TriggerableAction]
	public IEnumerator Disable()
	{
		base.enabled = false;
		return null;
	}

	private void Start()
	{
		statueA.ApplyAngle(135f);
		statueB.ApplyAngle(90f);
	}

	private void Update()
	{
		if (statueA.isStationary && statueB.isStationary && Mathf.DeltaAngle(statueA.currentAngle, statueB.currentAngle) == 0f)
		{
			StartCoroutine(statueMatchActions.DoSequence());
		}
	}
}
