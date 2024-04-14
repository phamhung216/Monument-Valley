using System;
using Fabric;
using UnityCommon;
using UnityEngine;

public class SocialNetworkManager : Service<SocialNetworkManager>
{
	private const string _message = "$(share_message)";

	private const string _emailSubject = "$(email_subject)";

	private const string appLink = "https://bit.ly/MV1site";

	private const string _shareAudioEvent = "User/Send";

	private bool _requestingAndroidWritePermission;

	private bool _saveToGallery;

	private void Start()
	{
		NativeSharing instance = NativeSharing.Instance;
		instance.OnShareCallback = (NativeSharingPlatform.OnShareCallback)Delegate.Combine(instance.OnShareCallback, new NativeSharingPlatform.OnShareCallback(OnShareCallback));
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		NativeSharing instance = NativeSharing.Instance;
		instance.OnShareCallback = (NativeSharingPlatform.OnShareCallback)Delegate.Remove(instance.OnShareCallback, new NativeSharingPlatform.OnShareCallback(OnShareCallback));
	}

	public void ShowShareOptions(string imagePath)
	{
		if (!string.IsNullOrEmpty(imagePath))
		{
			NativeSharing.Instance.Post(imagePath, LocalisationManager.Instance.LocaliseString("$(share_message)") + "\nhttps://bit.ly/MV1site", LocalisationManager.Instance.LocaliseString("$(email_subject)"));
		}
	}

	public void SaveToGallery(string imagePath)
	{
		if (!string.IsNullOrEmpty(imagePath))
		{
			_saveToGallery = true;
			NativeSharing.Instance.SaveImageToGallery(imagePath, "MV1", "MV1", "MV1");
		}
	}

	public void OnShareSuccessAndExitPreview(string param)
	{
		OnShareCallback(success: true, param);
		ExitPreviewPage();
	}

	public void ShareOptionsDidLoad(string message)
	{
		Time.timeScale = 0f;
	}

	private void ExitPreviewPage()
	{
		TriggerableActionSequence component = GameObject.Find("ScreenShotPreviewPageExit").GetComponent<TriggerableActionSequence>();
		if (component != null)
		{
			StartCoroutine(component.RunSequence());
		}
	}

	private void OnShareCallback(bool success, string extraInformation)
	{
		if (_saveToGallery && !Application.isEditor)
		{
			string message = (success ? LocalisationManager.Instance.LocaliseString("$(save_to_gallery_success)") : LocalisationManager.Instance.LocaliseString("$(save_to_gallery_failed)"));
			UnityCommon.DeviceUtils.Instance.ShowAlert(message, LocalisationManager.Instance.LocaliseString("$(save_to_gallery_ok)"));
		}
		if (success && (bool)EventManager.Instance)
		{
			EventManager.Instance.PostEvent("User/Send");
			Analytics.LogEvent("SocialShared_" + extraInformation);
		}
	}
}
