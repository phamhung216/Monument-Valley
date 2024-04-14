using System.Collections;
using UnityEngine;

public class CloudSave : MonoBehaviour
{
	public UIText signInText;

	public UILayout googlePlusLayout;

	public UILayout googlePlusSignedInLayout;

	private void OnEnable()
	{
	}

	private void OnDisable()
	{
	}

	private void Start()
	{
		googlePlusLayout.opacity = 0f;
	}

	[TriggerableAction]
	public IEnumerator HandleAuthentication()
	{
		return null;
	}

	[TriggerableAction]
	public IEnumerator SignOut()
	{
		return null;
	}

	[TriggerableAction]
	public IEnumerator ShowAchievementsDashboard()
	{
		return null;
	}

	[TriggerableAction]
	public IEnumerator RefreshText()
	{
		DebugLog("RefreshText");
		return null;
	}

	private void UpdateStatusText()
	{
	}

	private void DebugLog(string log)
	{
	}

	private void authenticationSucceededEvent()
	{
		DebugLog("authenticationSucceededEvent");
		UpdateStatusText();
	}

	private void authenticationFailedEvent()
	{
		DebugLog("authenticationFailedEvent");
		UpdateStatusText();
	}

	private void licenseCheckFailedEvent()
	{
		DebugLog("licenseCheckFailedEvent");
	}

	private void userSignedOutEvent()
	{
		DebugLog("userSignedOutEvent");
		UpdateStatusText();
	}
}
