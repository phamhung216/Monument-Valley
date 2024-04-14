using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UIDropdown : UIViewController
{
	public UILayout selectedItem;

	public UILayout optionContainer;

	public UIScrollView optionScrollView;

	public UILayout optionList;

	public UILayout optionSelectionMarker;

	private int _value;

	public List<string> options;

	public UnityEvent<int> onValueChanged;

	public void OnOptionPressed(int option)
	{
		if (option != _value)
		{
			SetValue(option);
			onValueChanged.Invoke(_value);
		}
		selectedItem.gameObject.SetActive(value: true);
		optionContainer.gameObject.SetActive(value: false);
	}

	public void OnControlPressed()
	{
		ShowOptionsScrollView();
	}

	private void ShowOptionsScrollView()
	{
		selectedItem.gameObject.SetActive(value: false);
		optionContainer.gameObject.SetActive(value: true);
		float num = (float)optionList.childLayoutCount * selectedItem.layoutHeight;
		float layoutMarginTop = 0f;
		if (num > selectedItem.rootLayout.layoutHeight)
		{
			num = selectedItem.rootLayout.layoutHeight;
			layoutMarginTop = 0f - optionScrollView.viewport.parentLayout.top;
		}
		else if (num > selectedItem.rootLayout.layoutHeight - selectedItem.top)
		{
			layoutMarginTop = selectedItem.rootLayout.layoutHeight - num - optionScrollView.viewport.parentLayout.top;
		}
		optionScrollView.viewport.layoutHeight = num;
		optionScrollView.viewport.layoutMarginTop = layoutMarginTop;
		optionScrollView.viewport.Unfinalise();
		optionScrollView.viewport.Layout();
	}

	public void OnSelectionCancelled()
	{
		selectedItem.gameObject.SetActive(value: true);
		optionContainer.gameObject.SetActive(value: false);
	}

	public void SetValue(int value)
	{
		_value = value;
		_ = selectedItem.layoutHeight;
		selectedItem.GetComponentInChildren<UIText>().SetText(options[value]);
		UpdateSelectionMarkerPosition();
	}

	public override void InitContent(UILayout layout)
	{
		UpdateOptionObjects();
		selectedItem.gameObject.SetActive(value: true);
		optionContainer.gameObject.SetActive(value: false);
	}

	public void UpdateOptions()
	{
		UpdateOptionObjects();
		optionContainer.Unfinalise();
		optionContainer.Layout();
	}

	private void UpdateOptionObjects()
	{
		float layoutHeight = selectedItem.layoutHeight;
		if (optionList.childLayoutCount != options.Count)
		{
			while (optionList.childLayoutCount > options.Count)
			{
				Object.Destroy(optionList.RemoveChild(optionList.childLayoutCount - 1).gameObject);
			}
			while (optionList.childLayoutCount < options.Count)
			{
				GameObject obj = Object.Instantiate(selectedItem.gameObject);
				int childLayoutCount = optionList.childLayoutCount;
				UILayout component = obj.GetComponent<UILayout>();
				component.layoutMarginTop = (float)childLayoutCount * layoutHeight;
				component.layoutHeightMode = UILayout.SizeMode.Fixed;
				component.layoutHeight = layoutHeight;
				optionList.AddChild(component);
				component.OnSpawnedAtRuntime();
			}
			optionScrollView.viewport.layoutHeight = layoutHeight;
			optionScrollView.viewport.layoutHeightMode = UILayout.SizeMode.Fixed;
			optionScrollView.content.layoutHeight = (float)optionList.childLayoutCount * layoutHeight;
			optionScrollView.content.layoutHeightMode = UILayout.SizeMode.Fixed;
			optionList.layoutHeight = optionScrollView.content.layoutHeight;
			optionList.layoutHeightMode = UILayout.SizeMode.Fixed;
		}
		int num = 0;
		foreach (string option in options)
		{
			_ = option;
			UILayout child = optionList.GetChild(num);
			child.GetComponentInChildren<UIText>().SetText(options[num]);
			UIButton componentInChildren = child.GetComponentInChildren<UIButton>();
			componentInChildren.onPressedUIEvent.RemoveAllListeners();
			int idx = num;
			componentInChildren.onPressedUIEvent.AddListener(delegate
			{
				OnOptionPressed(idx);
			});
			num++;
		}
		selectedItem.GetComponentInChildren<UIText>().SetText(options[_value]);
		UpdateSelectionMarkerPosition();
	}

	private void UpdateSelectionMarkerPosition()
	{
		float layoutHeight = selectedItem.layoutHeight;
		optionSelectionMarker.layoutMarginTop = (float)_value * layoutHeight + 0.5f * (layoutHeight - optionSelectionMarker.layoutHeight);
		optionSelectionMarker.Unfinalise();
		optionSelectionMarker.Layout();
	}
}
