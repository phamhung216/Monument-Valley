using UnityCommon;

public class MainPageController : UIViewController
{
	public UILayout quitGameButton;

	public override void InitContent(UILayout layout)
	{
		quitGameButton.gameObject.SetActive(OrientationOverrideManager.IsLandscape());
	}
}
