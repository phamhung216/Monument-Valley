using System;
using System.Collections;
using UnityEngine;

public class Highlight : MonoBehaviour
{
	public ParticleSystem highlightVFX;

	public bool hasTapped;

	public string highLightHasTappedKey = "";

	private InAppPurchaseLogic _inAppPurchaseLogic;

	public UILayout newChaptersTextLayout;

	public bool mainGameCompleteRequired = true;

	private string _latestHighlightVersion = "2.3.0";

	private void Start()
	{
		Version value = new Version(_latestHighlightVersion);
		bool flag = LevelManager.Instance.IsMainGameComplete();
		bool flag2 = true;
		if (highLightHasTappedKey.Length > 0 && SharedPlayerPrefs.HasKey(highLightHasTappedKey))
		{
			flag2 = SharedPlayerPrefs.GetInt(highLightHasTappedKey) == 1 || new Version(SharedPlayerPrefs.GetString(highLightHasTappedKey)).CompareTo(value) < 0;
		}
		if (mainGameCompleteRequired)
		{
			hasTapped = !(flag && flag2);
		}
		else
		{
			hasTapped = !flag2;
		}
		_inAppPurchaseLogic = UnityEngine.Object.FindObjectOfType<InAppPurchaseLogic>();
	}

	[TriggerableAction]
	public IEnumerator StartWorldSelectHighlight()
	{
		if (!hasTapped && (bool)highlightVFX)
		{
			highlightVFX.Play();
		}
		return null;
	}

	[TriggerableAction]
	public IEnumerator StartExpansionHighlight()
	{
		if (!hasTapped && (bool)_inAppPurchaseLogic && _inAppPurchaseLogic.HasPurchasedContentPack1() && (bool)highlightVFX)
		{
			highlightVFX.Play();
		}
		return null;
	}

	[TriggerableAction]
	public IEnumerator StopHighLight()
	{
		if (!hasTapped && (bool)highlightVFX)
		{
			highlightVFX.Stop();
		}
		return null;
	}

	[TriggerableAction]
	public IEnumerator TappedWorldSelect()
	{
		if (!hasTapped)
		{
			hasTapped = true;
			if ((bool)highlightVFX)
			{
				highlightVFX.Stop();
			}
			if (highLightHasTappedKey.Length > 0)
			{
				SharedPlayerPrefs.SetString(highLightHasTappedKey, ApplicationInfo.bundleVersion);
			}
			if ((bool)newChaptersTextLayout)
			{
				newChaptersTextLayout.opacity = 0f;
			}
		}
		return null;
	}

	[TriggerableAction]
	public IEnumerator ShowNewChaptersText()
	{
		if (!hasTapped && (bool)newChaptersTextLayout)
		{
			newChaptersTextLayout.opacity = 1f;
		}
		return null;
	}
}
