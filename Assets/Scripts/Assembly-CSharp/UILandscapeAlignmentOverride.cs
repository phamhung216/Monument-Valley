using UnityEngine;

public class UILandscapeAlignmentOverride : MonoBehaviour
{
	public UILayout layout;

	public UILayout.Axis axis;

	public UILayout.AlignMode portraitAlignMode;

	public float portraitMarginLower;

	public float portraitMarginUpper;

	public UILayout.AlignMode landscapeAlignMode;

	public float landscapeMarginLower;

	public float landscapeMarginUpper;

	private void Awake()
	{
		bool flag = Screen.width > Screen.height;
		layout.SetAlignMode((int)axis, flag ? landscapeAlignMode : portraitAlignMode);
		layout.SetMargin((int)axis, UILayout.Margin.Lower, flag ? landscapeMarginLower : portraitMarginLower);
		layout.SetMargin((int)axis, UILayout.Margin.Upper, flag ? landscapeMarginUpper : portraitMarginUpper);
	}
}
