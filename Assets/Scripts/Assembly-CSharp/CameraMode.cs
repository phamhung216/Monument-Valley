using System.Collections;
using MVCommon;
using UnityCommon;
using UnityEngine;

public class CameraMode : MonoBehaviour
{
	private enum EnterCameraModeFrom
	{
		PausePage = 0,
		LevelPage = 1,
		LevelComplete = 2
	}

	public enum AspectMode
	{
		Portrait = 0,
		Square = 1
	}

	public float aspectInterpolationParam = 1f;

	public LayerMask layersToDisable;

	private bool _isEnabled = true;

	private bool _isCameraModeActive;

	private CameraPanController _panController;

	public UILayout viewport;

	public UILayout whiteQuad;

	private int _borderHeight;

	private EnterCameraModeFrom _enterCameraModeFrom;

	public TriggerableActionSequence exitCameraModeToPause;

	public TriggerableActionSequence exitCameraModeToLevelComplete;

	public TriggerableActionSequence exitCameraModeToGame;

	public GameObject gameCameraModeButton;

	public TriggerableActionSequence screenShotPreviewFade;

	private HighResScreenShot _highResScreenShot;

	private AspectMode _aspectMode;

	private float _cameraButtonTime;

	public float buttonFadeTime = 1f;

	public UICamera uiCamera;

	private Animation _animation;

	private UILayout cameraButtonLayout;

	public bool IsEnabled => _isEnabled;

	public bool isCameraModeActive => _isCameraModeActive;

	public AspectMode aspectMode => _aspectMode;

	private void Start()
	{
		_panController = Camera.main.GetComponent<CameraPanController>();
		_animation = GetComponent<Animation>();
		_borderHeight = Mathf.FloorToInt(uiCamera.layoutHeight - uiCamera.layoutWidth) / 2;
		viewport.layoutWidthMode = UILayout.SizeMode.Fixed;
		viewport.layoutHeightMode = UILayout.SizeMode.Fixed;
		viewport.layoutWidth = viewport.rootLayout.layoutWidth;
		viewport.layoutHeight = viewport.rootLayout.layoutHeight;
		_highResScreenShot = GameObject.Find("Scene").GetComponent<HighResScreenShot>();
		cameraButtonLayout = gameCameraModeButton.GetComponent<UILayout>();
	}

	private void LateUpdate()
	{
		if (!_isEnabled)
		{
			return;
		}
		if (_animation.isPlaying)
		{
			float a = Mathf.Min(viewport.rootLayout.layoutWidth, viewport.rootLayout.layoutHeight);
			viewport.layoutWidth = Mathf.Lerp(a, viewport.rootLayout.layoutWidth, aspectInterpolationParam);
			viewport.layoutHeight = Mathf.Lerp(a, viewport.rootLayout.layoutHeight, aspectInterpolationParam);
			viewport.Unfinalise();
			viewport.Layout();
		}
		if (_panController.mode != CameraPanController.Mode.Normal && _isCameraModeActive)
		{
			ExitCameraMode();
		}
		if ((double)cameraButtonLayout.opacity < 0.03 && _panController.canActivateCameraMode)
		{
			if (!cameraButtonLayout.animationComponent.isPlaying)
			{
				cameraButtonLayout.animationComponent.Play("UILayout_FadeInVeryFast");
				_cameraButtonTime = buttonFadeTime;
			}
		}
		else if (cameraButtonLayout.opacity == 1f)
		{
			_cameraButtonTime -= Time.deltaTime;
			if (_cameraButtonTime <= 0f && !_panController.canActivateCameraMode)
			{
				cameraButtonLayout.animationComponent.Play("UILayout_FadeOut");
			}
		}
	}

	[TriggerableAction]
	public IEnumerator BeginCameraMode()
	{
		_isEnabled = true;
		if (!_isCameraModeActive)
		{
			_isCameraModeActive = true;
		}
		return null;
	}

	[TriggerableAction]
	public IEnumerator EndCameraMode()
	{
		if (_isCameraModeActive)
		{
			_isCameraModeActive = false;
			GameScene.player.GetComponent<PlayerInput>().Enable();
			_panController.allowFreeZoom = false;
			_panController.StartZoomSnapToCentre();
		}
		return null;
	}

	[TriggerableAction]
	public IEnumerator DisableInputsForScreenShotPreview()
	{
		GameScene.player.GetComponent<PlayerInput>().Disable();
		_panController.isEnabled = false;
		return null;
	}

	[TriggerableAction]
	public IEnumerator EnableInputsForCameraMode()
	{
		GameScene.player.GetComponent<PlayerInput>().Disable();
		_panController.allowFreeZoom = true;
		_panController.isEnabled = true;
		Time.timeScale = 1f;
		return null;
	}

	[TriggerableAction]
	public IEnumerator EnterCameraModeFromPause()
	{
		EnterCameraModeFromMode(EnterCameraModeFrom.PausePage);
		return null;
	}

	[TriggerableAction]
	public IEnumerator EnterCameraModeFromGame()
	{
		EnterCameraModeFromMode(EnterCameraModeFrom.LevelPage);
		return null;
	}

	[TriggerableAction]
	public IEnumerator EnterCameraModeFromLevelComplete()
	{
		EnterCameraModeFromMode(EnterCameraModeFrom.LevelComplete);
		return null;
	}

	private void EnterCameraModeFromMode(EnterCameraModeFrom from)
	{
		_enterCameraModeFrom = from;
		if (from == EnterCameraModeFrom.LevelComplete)
		{
			_panController.mode = CameraPanController.Mode.Normal;
		}
		Camera.main.cullingMask &= ~(int)layersToDisable;
	}

	[TriggerableAction]
	public IEnumerator ExitCameraMode()
	{
		Camera.main.cullingMask |= layersToDisable;
		if (exitCameraModeToPause != null)
		{
			switch (_enterCameraModeFrom)
			{
			case EnterCameraModeFrom.PausePage:
				StartCoroutine(exitCameraModeToPause.RunSequence());
				EndCameraMode();
				break;
			case EnterCameraModeFrom.LevelComplete:
				StartCoroutine(exitCameraModeToLevelComplete.RunSequence());
				_isCameraModeActive = false;
				_panController.allowFreeZoom = false;
				break;
			case EnterCameraModeFrom.LevelPage:
				StartCoroutine(exitCameraModeToGame.RunSequence());
				EndCameraMode();
				break;
			}
		}
		return null;
	}

	[TriggerableAction]
	public IEnumerator PauseGame()
	{
		Time.timeScale = 0f;
		return null;
	}

	[TriggerableAction]
	public IEnumerator DoCapture()
	{
		if (OrientationOverrideManager.IsLandscape())
		{
			Service<MVCursor>.Instance.HideCursor();
		}
		Time.timeScale = 0f;
		Vector2 vector = viewport.rootLayout.LayoutToScreenSpace(viewport.layoutSize);
		Rect cropRect = new Rect(0.5f * new Vector2(Screen.width, Screen.height) - 0.5f * vector, vector);
		_highResScreenShot.TakeScreenShotWithRect(cropRect);
		return null;
	}

	[TriggerableAction]
	public IEnumerator ShowShareOptions()
	{
		Service<SocialNetworkManager>.Instance.ShowShareOptions(HighResScreenShot.ScreenShotNameForDevice());
		return null;
	}

	[TriggerableAction]
	public IEnumerator SaveToGallery()
	{
		ScreenshotUtils.AddImageToScreenshotLibrary(_highResScreenShot.LastSavedScreenshotFilename, (int)_highResScreenShot.CaptureRect.width, (int)_highResScreenShot.CaptureRect.height);
		return null;
	}

	[TriggerableAction]
	public IEnumerator ToggleCameraModeAspect()
	{
		if (!_animation.isPlaying)
		{
			if (_aspectMode == AspectMode.Portrait)
			{
				_aspectMode = AspectMode.Square;
				_animation.Play("CameraMode_AspectToSquare");
			}
			else
			{
				_aspectMode = AspectMode.Portrait;
				_animation.Play("CameraMode_AspectToPortrait");
			}
		}
		return null;
	}

	[TriggerableAction]
	public IEnumerator HandleScreenshotPreviewFade()
	{
		if (screenShotPreviewFade != null)
		{
			StartCoroutine(screenShotPreviewFade.RunSequence());
		}
		if (OrientationOverrideManager.IsLandscape())
		{
			Service<MVCursor>.Instance.ShowCursor();
		}
		return null;
	}
}
