using System.Collections;
using UnityCommon;

public class PauseMenuController : UIViewController
{
	public UILayout quitConfirmPage;

	public TriggerableActionSequence hideQuitConfirmPage;

	public UILayout settingsPage;

	public TriggerableActionSequence hideSettingsPage;

	public UILayout pauseMenuPage;

	public TriggerableActionSequence hidePauseMenuPage;

	public UILayout buttonsGroup_Mobile;

	public UILayout buttonsGroup_PC;

	public override void InitContent(UILayout layout)
	{
		if (OrientationOverrideManager.IsLandscape())
		{
			buttonsGroup_Mobile.gameObject.SetActive(value: false);
			buttonsGroup_PC.gameObject.SetActive(value: true);
		}
		else
		{
			buttonsGroup_Mobile.gameObject.SetActive(value: true);
			buttonsGroup_PC.gameObject.SetActive(value: false);
		}
	}

	[TriggerableAction]
	public IEnumerator HideAllPauseMenus()
	{
		if (quitConfirmPage.opacity > 0f)
		{
			hideQuitConfirmPage.TriggerActions();
		}
		else if (settingsPage.opacity > 0f)
		{
			hideSettingsPage.TriggerActions();
		}
		else if (pauseMenuPage.opacity > 0f)
		{
			hidePauseMenuPage.TriggerActions();
		}
		return null;
	}
}
