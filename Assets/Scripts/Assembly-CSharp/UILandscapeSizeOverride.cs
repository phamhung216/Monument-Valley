public class UILandscapeSizeOverride : UIViewController
{
	public UILayout.SizeMode portraitWidthMode = UILayout.SizeMode.Fixed;

	public float portraitWidth;

	public UILayout.SizeMode landscapeWidthMode = UILayout.SizeMode.Fixed;

	public float landscapeWidth;

	public override void SetupLayout(UILayout layout)
	{
		bool flag = layout.rootLayout.layoutWidth > layout.rootLayout.layoutHeight;
		layout.layoutWidthMode = (flag ? landscapeWidthMode : portraitWidthMode);
		layout.layoutWidth = (flag ? landscapeWidth : portraitWidth);
	}
}
