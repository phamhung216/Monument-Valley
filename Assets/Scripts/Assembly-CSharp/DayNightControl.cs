using System.Collections;
using UnityEngine;

public class DayNightControl : MonoBehaviour
{
	public enum TimePeriod
	{
		DAY = 0,
		NIGHT = 1
	}

	public Material[] blendMats;

	public float blendProgress;

	private bool acceptInput = true;

	public TimePeriod currentTimePeriod;

	private float blendAmount;

	public Doppelganger doppelganger;

	private void Start()
	{
		Material[] array = blendMats;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetFloat("_Blend", 0f);
		}
	}

	[TriggerableAction]
	public IEnumerator Trigger()
	{
		if (acceptInput)
		{
			currentTimePeriod = 1 - currentTimePeriod;
			StartCoroutine("ChangeToTimePeriod", currentTimePeriod);
		}
		return null;
	}

	public IEnumerator ChangeToTimePeriod(TimePeriod period)
	{
		acceptInput = false;
		doppelganger.Flip(period);
		if (period == TimePeriod.NIGHT)
		{
			blendAmount = 0f;
			while (blendAmount < 1f)
			{
				blendAmount += 0.2f;
				Material[] array = blendMats;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].SetFloat("_Blend", blendAmount);
				}
				yield return new WaitForSeconds(0.05f);
			}
		}
		else
		{
			blendAmount = 1f;
			while (blendAmount > 0f)
			{
				blendAmount -= 0.2f;
				Material[] array = blendMats;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].SetFloat("_Blend", blendAmount);
				}
				yield return new WaitForSeconds(0.05f);
			}
		}
		GameScene.navManager.ScanAllConnections();
		acceptInput = true;
		yield return null;
	}
}
