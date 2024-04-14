using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LanguageSelectPage : UIViewController
{
	[Serializable]
	public class AvailableLanguage
	{
		public DeviceLanguage language;

		public string name;
	}

	public List<AvailableLanguage> availableLanguages;

	public UILayout languageButtonList;

	public UILayout selectionIndicator;

	public UnityEvent<string> onLanguageChanged;

	public override void InitContent(UILayout layout)
	{
		UILayout child = languageButtonList.GetChild(0);
		for (int i = 1; i < availableLanguages.Count; i++)
		{
			UILayout component = UnityEngine.Object.Instantiate(child.gameObject, languageButtonList.transform).GetComponent<UILayout>();
			languageButtonList.AddChild(component);
			component.OnSpawnedAtRuntime();
		}
		AvailableLanguage availableLanguage = null;
		for (int j = 0; j < availableLanguages.Count; j++)
		{
			UILayout child2 = languageButtonList.GetChild(j);
			child2.GetComponentInChildren<UIText>().SetText(availableLanguages[j].name);
			int index = j;
			AvailableLanguage langInfo = availableLanguages[index];
			child2.GetComponentInChildren<UIButton>().onPressedUIEvent.AddListener(delegate
			{
				SelectLanguage(langInfo.language, langInfo.name);
			});
			if (langInfo.language == LocalisationManager.Instance.currentLanguage)
			{
				availableLanguage = availableLanguages[index];
			}
			TextMesh componentInChildren = child2.GetComponentInChildren<TextMesh>();
			int num = j;
			if (j <= availableLanguages.Count / 2)
			{
				child2.horizontalAlignMode = UILayout.HorizontalAlignMode.ParentLeft;
				componentInChildren.alignment = TextAlignment.Left;
				componentInChildren.anchor = TextAnchor.MiddleLeft;
			}
			else
			{
				num = j - 1 - availableLanguages.Count / 2;
				child2.horizontalAlignMode = UILayout.HorizontalAlignMode.ParentRight;
				componentInChildren.alignment = TextAlignment.Right;
				componentInChildren.anchor = TextAnchor.MiddleRight;
			}
			child2.layoutWidth = 0.5f * languageButtonList.layoutWidth;
			child2.layoutWidthMode = UILayout.SizeMode.Fixed;
			child2.verticalAlignMode = UILayout.VerticalAlignMode.ParentTop;
			child2.layoutMarginTop = (float)num * child2.layoutHeight;
		}
		languageButtonList.layoutHeight = Mathf.Ceil((float)availableLanguages.Count / 2f) * languageButtonList.GetChild(0).layoutHeight;
		UpdateSelectionIndicatorPosition(availableLanguage.language);
		if (availableLanguage != null)
		{
			onLanguageChanged.Invoke(availableLanguage.name);
		}
	}

	private void UpdateSelectionIndicatorPosition(DeviceLanguage language)
	{
		UILayout child = languageButtonList.GetChild(0);
		for (int i = 0; i < availableLanguages.Count; i++)
		{
			if (language == availableLanguages[i].language)
			{
				int num = i;
				if (i <= availableLanguages.Count / 2)
				{
					selectionIndicator.horizontalAlignMode = UILayout.HorizontalAlignMode.ParentLeft;
				}
				else
				{
					selectionIndicator.horizontalAlignMode = UILayout.HorizontalAlignMode.ParentRight;
					num -= 1 + availableLanguages.Count / 2;
				}
				selectionIndicator.layoutMarginTop = (float)num * child.layoutHeight;
				selectionIndicator.layoutMarginTop += 0.5f * (child.layoutHeight - selectionIndicator.layoutHeight);
				break;
			}
		}
	}

	private void SelectLanguage(DeviceLanguage language, string name)
	{
		if (language != LocalisationManager.Instance.currentLanguage)
		{
			LocalisationManager.Instance.SetAndSaveCurrentLanguage(language);
			UnityEngine.Object.FindObjectOfType<UICamera>().OnLanguageChanged();
			onLanguageChanged.Invoke(name);
			UpdateSelectionIndicatorPosition(language);
			selectionIndicator.parentLayout.Layout();
		}
	}
}
