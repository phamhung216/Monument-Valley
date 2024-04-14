using System.Collections;
using System.Collections.Generic;
using MVCommon;
using UnityCommon;
using UnityEngine;

public class SettingsPage : UIViewController
{
	public UILayout restorePurchasesGroup;

	public UIText restoreText;

	public UILayout googlePlayLoginGroup;

	public UIDropdown resolutionDropdown;

	public UIDropdown antiAliasingDropdown;

	public UIButton languageButton;

	public UILayout pcSettingsSection;

	public UILayout mobileSettingsSection;

	public LanguageSelectPage languageSelectPage;

	public AnniversarySettings anniversarySettings;

	private void Awake()
	{
		if (OrientationOverrideManager.IsLandscape())
		{
			restorePurchasesGroup.gameObject.SetActive(value: false);
			pcSettingsSection.gameObject.SetActive(value: true);
			mobileSettingsSection.gameObject.SetActive(value: false);
		}
		else
		{
			pcSettingsSection.gameObject.SetActive(value: false);
			mobileSettingsSection.gameObject.SetActive(value: true);
		}
		googlePlayLoginGroup.gameObject.SetActive(value: false);
	}

	public override void InitContent(UILayout layout)
	{
		languageSelectPage.onLanguageChanged.AddListener(delegate(string name)
		{
			OnLanguageChanged(name);
		});
		resolutionDropdown.options.Clear();
		List<Vector2Int> resolutions = PerformanceSettings.Instance.GetAvailableResolutions();
		int value2 = 0;
		foreach (Vector2Int item in resolutions)
		{
			resolutionDropdown.options.Add($"{item.x} x {item.y}");
			if (item.x == Screen.width && item.y == Screen.height)
			{
				value2 = resolutionDropdown.options.Count - 1;
			}
		}
		resolutionDropdown.UpdateOptions();
		resolutionDropdown.SetValue(value2);
		resolutionDropdown.onValueChanged.AddListener(delegate(int value)
		{
			if (value < resolutions.Count)
			{
				PerformanceSettings.Instance.SetAndSaveResolution(resolutions[value]);
			}
		});
		antiAliasingDropdown.options.Clear();
		PerformanceSettings.AALevel[] aaOptions = PerformanceSettings.Instance.GetAvailableAntiAliasingLevels();
		int value3 = 0;
		PerformanceSettings.AALevel[] array = aaOptions;
		foreach (PerformanceSettings.AALevel aALevel in array)
		{
			antiAliasingDropdown.options.Add($"{(int)aALevel}x");
			if (aALevel == PerformanceSettings.Instance.currentAALevel)
			{
				value3 = antiAliasingDropdown.options.Count - 1;
			}
		}
		antiAliasingDropdown.UpdateOptions();
		antiAliasingDropdown.SetValue(value3);
		antiAliasingDropdown.onValueChanged.AddListener(delegate(int value)
		{
			PerformanceSettings.Instance.SetAndSaveAntiAliasingLevel(aaOptions[value]);
		});
		anniversarySettings.Init();
	}

	[TriggerableAction]
	public IEnumerator RefreshTexts()
	{
		if ((bool)restoreText)
		{
			restoreText.text = "$(iap_restore)";
		}
		return null;
	}

	public void OnLanguageChanged(string newName)
	{
		languageButton.GetComponentInChildren<UIText>().SetText(newName);
	}
}
