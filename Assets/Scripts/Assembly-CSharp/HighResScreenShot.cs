using System.Collections;
using System.IO;
using UnityEngine;

public class HighResScreenShot : MonoBehaviour
{
	public int resWidth = 1536;

	public int resHeight = 2048;

	public bool takeShot;

	public Camera cam;

	public Camera cam2;

	private Rect _captureRect;

	private string _lastSavedScreenshotFilename;

	public Rect CaptureRect => _captureRect;

	public string LastSavedScreenshotFilename => _lastSavedScreenshotFilename;

	private void Start()
	{
		if (Application.platform != 0)
		{
			resWidth = Screen.width;
			resHeight = Screen.height;
		}
		_captureRect = new Rect(0f, 0f, resWidth, resHeight);
	}

	private void LateUpdate()
	{
		if (takeShot)
		{
			StartCoroutine(DoCapture());
		}
	}

	public void TakeScreenShot()
	{
		takeShot = true;
		_captureRect = new Rect(0f, 0f, resWidth, resHeight);
	}

	public void TakeScreenShotWithRect(Rect cropRect)
	{
		takeShot = true;
		_captureRect = cropRect;
	}

	public IEnumerator DoCapture()
	{
		yield return new WaitForEndOfFrame();
		int width = (int)_captureRect.width;
		int height = (int)_captureRect.height;
		Texture2D texture2D = new Texture2D(width, height, TextureFormat.RGB24, mipChain: false);
		texture2D.ReadPixels(_captureRect, 0, 0);
		texture2D.Apply();
		byte[] bytes = texture2D.EncodeToPNG();
		_lastSavedScreenshotFilename = ScreenShotName(width, height);
		File.WriteAllBytes(_lastSavedScreenshotFilename, bytes);
		takeShot = false;
	}

	private IEnumerator take()
	{
		yield return new WaitForEndOfFrame();
		RenderTexture renderTexture = new RenderTexture(resWidth, resHeight, 24);
		Texture2D texture2D = new Texture2D(resWidth, resHeight, TextureFormat.RGB24, mipChain: false);
		cam.targetTexture = renderTexture;
		cam.Render();
		if ((bool)cam2)
		{
			cam2.targetTexture = renderTexture;
			cam2.Render();
		}
		RenderTexture.active = renderTexture;
		texture2D.ReadPixels(new Rect(0f, 0f, resWidth, resHeight), 0, 0);
		cam.targetTexture = null;
		if ((bool)cam2)
		{
			cam2.targetTexture = null;
		}
		RenderTexture.active = null;
		Object.Destroy(renderTexture);
		texture2D.EncodeToPNG();
		ScreenShotName(resWidth, resHeight);
		takeShot = false;
	}

	public static string ScreenShotName(int width, int height)
	{
		return ScreenShotNameForDevice();
	}

	public static string ScreenShotNameForDevice()
	{
		return $"{Application.persistentDataPath}/screenshot.png";
	}
}
