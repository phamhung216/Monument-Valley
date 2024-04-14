using System.Collections;
using UnityEngine;

public class FinaleSpiralLogic : MonoBehaviour
{
	public AutoRotator pillarRotator;

	[TriggerableAction(true)]
	public IEnumerator Trigger0()
	{
		pillarRotator.endAngle = 90f;
		return pillarRotator.Move();
	}

	[TriggerableAction(true)]
	public IEnumerator Trigger1()
	{
		pillarRotator.endAngle = 180f;
		return pillarRotator.Move();
	}

	[TriggerableAction(true)]
	public IEnumerator Trigger2()
	{
		pillarRotator.endAngle = 360f;
		return pillarRotator.Move();
	}

	[TriggerableAction(true)]
	public IEnumerator TriggerBase()
	{
		pillarRotator.endAngle = 0f;
		return pillarRotator.Move();
	}
}
