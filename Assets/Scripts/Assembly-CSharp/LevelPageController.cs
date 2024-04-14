using UnityCommon;

public class LevelPageController : UIViewController
{
	public UILayout pauseButton;

	public UILayout quitLevelButton;

	public bool disablePauseMenuOnMobile;

	public override void InitContent(UILayout layout)
	{
		if (disablePauseMenuOnMobile)
		{
			if (OrientationOverrideManager.IsPortrait())
			{
				pauseButton.gameObject.SetActive(value: false);
				return;
			}
			layout.opacity = 1f;
			quitLevelButton.gameObject.SetActive(value: false);
		}
	}
}
