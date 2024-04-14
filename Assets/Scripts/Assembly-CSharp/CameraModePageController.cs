using UnityCommon;

public class CameraModePageController : UIViewController
{
	public string mouseZoomStringID;

	public string touchZoomStringID;

	public UIText zoomHintText;

	public UILayout viewportLayout;

	public UILayout cameraButtonsLayout;

	public UILayout screenshotButtonsLayout;

	public override void InitContent(UILayout layout)
	{
		zoomHintText.SetText(OrientationOverrideManager.IsLandscape() ? mouseZoomStringID : touchZoomStringID);
		cameraButtonsLayout.layoutWidthMode = UILayout.SizeMode.Fixed;
		if (OrientationOverrideManager.IsLandscape())
		{
			cameraButtonsLayout.layoutWidth = cameraButtonsLayout.rootLayout.layoutHeight / 3f;
		}
		else
		{
			cameraButtonsLayout.layoutWidth = cameraButtonsLayout.rootLayout.layoutWidth;
		}
		screenshotButtonsLayout.layoutWidthMode = UILayout.SizeMode.Fixed;
		screenshotButtonsLayout.layoutWidth = cameraButtonsLayout.layoutWidth;
	}

	public void Update()
	{
	}
}
