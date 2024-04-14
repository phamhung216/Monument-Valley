using System.Collections;
using UnityEngine;

public class ChangePauseUITexture : MonoBehaviour
{
	public string whitePauseImageName;

	public string whiteCameraImageName;

	public string blackPauseImageName;

	public string blackCameraImageName;

	public UIImage pauseImage;

	public UIImage cameraImage;

	private void Start()
	{
		InitButton(pauseImage, blackPauseImageName);
		InitButton(cameraImage, blackCameraImageName);
	}

	private static void InitButton(UIImage image, string blackTextureName)
	{
		if ((bool)image)
		{
			UIButton componentInParent = image.GetComponentInParent<UIButton>();
			if ((bool)componentInParent)
			{
				componentInParent.dimmingMode = ((image.subTextureName == blackTextureName) ? UIButton.DimmingMode.OverDarken : UIButton.DimmingMode.Darken);
			}
		}
	}

	[TriggerableAction]
	public IEnumerator SetUIBlack()
	{
		UpdateImage(pauseImage, blackPauseImageName, UIButton.DimmingMode.OverDarken);
		UpdateImage(cameraImage, blackCameraImageName, UIButton.DimmingMode.OverDarken);
		return null;
	}

	[TriggerableAction]
	public IEnumerator SetUIWhite()
	{
		UpdateImage(pauseImage, whitePauseImageName, UIButton.DimmingMode.Darken);
		UpdateImage(cameraImage, whiteCameraImageName, UIButton.DimmingMode.Darken);
		return null;
	}

	private static void UpdateImage(UIImage image, string subTextureName, UIButton.DimmingMode buttonDimmingMode)
	{
		if ((bool)image)
		{
			image.subTextureName = subTextureName;
			image.subTexture = null;
			UIButton componentInParent = image.GetComponentInParent<UIButton>();
			if ((bool)componentInParent)
			{
				componentInParent.dimmingMode = buttonDimmingMode;
			}
		}
	}
}
