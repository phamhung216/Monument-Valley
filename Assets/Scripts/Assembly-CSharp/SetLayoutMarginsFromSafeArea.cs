using UnityEngine;

public class SetLayoutMarginsFromSafeArea : MonoBehaviour
{
	public enum OffsetSource
	{
		TopUnsafeBorderHeight = 0,
		BottomUnsafeBorderHeight = 1,
		MinimalTopUnsafeBorderHeight = 2
	}

	private bool _layoutInitialised;

	[Header("Top Margin")]
	public bool setTopMargin;

	public OffsetSource topMarginOffsetSource;

	[Header("Bottom Margin")]
	public bool setBottomMargin;

	public OffsetSource bottomMarginOffsetSource = OffsetSource.BottomUnsafeBorderHeight;

	private void Update()
	{
		if (!_layoutInitialised)
		{
			_layoutInitialised = true;
			UpdateSafeArea();
		}
	}

	private void UpdateSafeArea()
	{
		Rect safeArea = Screen.safeArea;
		UILayout component = GetComponent<UILayout>();
		UICamera componentInParent = component.GetComponentInParent<UICamera>();
		bool flag = false;
		float num = ((float)Screen.height - safeArea.yMax) / (float)Screen.height * componentInParent.layoutHeight;
		float num2 = Mathf.RoundToInt(0.5f * num);
		float num3 = safeArea.y / (float)Screen.height * componentInParent.layoutHeight;
		if (setTopMargin && safeArea.y > 0f)
		{
			float num4 = component.layoutMarginTop;
			switch (topMarginOffsetSource)
			{
			case OffsetSource.TopUnsafeBorderHeight:
				num4 += num;
				break;
			case OffsetSource.MinimalTopUnsafeBorderHeight:
				num4 += num2;
				break;
			case OffsetSource.BottomUnsafeBorderHeight:
				num4 -= num3;
				break;
			}
			component.layoutMarginTop = num4;
			flag = true;
		}
		if (setBottomMargin && safeArea.yMax < (float)Screen.height)
		{
			float num5 = component.layoutMarginBottom;
			switch (bottomMarginOffsetSource)
			{
			case OffsetSource.TopUnsafeBorderHeight:
				num5 -= num;
				break;
			case OffsetSource.MinimalTopUnsafeBorderHeight:
				num5 -= num2;
				break;
			case OffsetSource.BottomUnsafeBorderHeight:
				num5 += num3;
				break;
			}
			component.layoutMarginBottom = num5;
			flag = true;
		}
		if (flag)
		{
			component.Unfinalise();
			component.Layout();
			component.UpdateIsFinalised();
		}
	}
}
