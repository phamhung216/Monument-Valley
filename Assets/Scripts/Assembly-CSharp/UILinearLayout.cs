using UnityEngine;

public class UILinearLayout : UILayout
{
	private bool finalisedLinearLayout;

	public void ForceUpdate()
	{
		FindChildren();
		finalisedLinearLayout = false;
	}

	private void Update()
	{
		if (!_widthFinalised || finalisedLinearLayout)
		{
			return;
		}
		finalisedLinearLayout = true;
		int num = Mathf.FloorToInt(layoutWidth / (float)_childLayouts.Count);
		foreach (UILayout childLayout in _childLayouts)
		{
			childLayout.layoutWidth = num;
			childLayout.Layout();
		}
	}
}
