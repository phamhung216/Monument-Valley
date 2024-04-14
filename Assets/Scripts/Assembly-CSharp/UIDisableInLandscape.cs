public class UIDisableInLandscape : UIViewController
{
	public enum Orientation
	{
		Portrait = 0,
		Landscape = 1
	}

	public Orientation enabledInOrientation;

	public override void SetupLayout(UILayout layout)
	{
		bool flag = layout.rootLayout.layoutWidth > layout.rootLayout.layoutHeight;
		layout.gameObject.SetActive(flag == (enabledInOrientation == Orientation.Landscape));
	}
}
