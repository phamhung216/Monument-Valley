using System.Collections;
using Fabric;
using UnityEngine;

public class WaterSection : MonoBehaviour
{
	public float fillSpeed = 8f;

	public float unFillSpeed = 8f;

	public bool isEnabled;

	public float fillAmount;

	public float fillOpacity;

	public WaterSection parentWaterSection;

	public MaterialInstantiator[] materialInstantiators;

	public bool muteAudio = true;

	public float audioAmplitude = 1f;

	public bool fillChildrenAfterTheshold;

	public static string waterLoopEvent = "World/WaterStream/Loop";

	[TriggerableAction]
	public IEnumerator StartFlow()
	{
		isEnabled = true;
		return null;
	}

	[TriggerableAction]
	public IEnumerator StopFlow()
	{
		isEnabled = false;
		return null;
	}

	[TriggerableAction]
	public IEnumerator DiscardParent()
	{
		parentWaterSection = null;
		return null;
	}

	[TriggerableAction]
	public IEnumerator MuteAudio()
	{
		muteAudio = true;
		if (EventManager.Instance.IsEventActive(waterLoopEvent, base.gameObject))
		{
			EventManager.Instance.PostEvent(waterLoopEvent, EventAction.StopSound, base.gameObject);
		}
		return null;
	}

	[TriggerableAction]
	public IEnumerator UnmuteAudio()
	{
		muteAudio = false;
		return null;
	}

	public bool hasInFlow()
	{
		if ((bool)parentWaterSection)
		{
			if (isEnabled)
			{
				return parentWaterSection.SuppliesChildren();
			}
			return false;
		}
		return isEnabled;
	}

	public bool SuppliesChildren()
	{
		if (fillChildrenAfterTheshold)
		{
			return fillAmount > 0.5f;
		}
		return hasInFlow();
	}

	private void Update()
	{
		if (!hasInFlow() && fillAmount > 0f)
		{
			fillAmount -= Time.deltaTime * unFillSpeed;
			if (fillOpacity < 0f)
			{
				fillAmount = 0f;
			}
		}
		else if (hasInFlow() && fillAmount < 1f)
		{
			fillAmount += Time.deltaTime * fillSpeed;
			if (fillAmount > 1f)
			{
				fillAmount = 1f;
			}
		}
		if (!muteAudio && (bool)EventManager.Instance)
		{
			if (!EventManager.Instance.IsEventActive(waterLoopEvent, base.gameObject) && isEnabled)
			{
				EventManager.Instance.PostEvent(waterLoopEvent, EventAction.PlaySound, base.gameObject);
			}
			EventManager.Instance.SetParameter(waterLoopEvent, "fillAmount", fillAmount * audioAmplitude, base.gameObject);
		}
		MaterialInstantiator[] array = materialInstantiators;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].instantiatedMaterial.SetFloat("_FillAmount", fillAmount);
		}
	}
}
