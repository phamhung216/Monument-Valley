using System.Collections;
using UnityEngine;

public class Weblink : MonoBehaviour
{
	public string url = "http://www.monumentvalleygame.com/";

	[Tooltip("Override the above Url property with an iOS-specific URL")]
	public string iosUrl;

	[Tooltip("Override the above Url property with an Android-specific URL")]
	public string androidUrl;

	[Tooltip("Override the above Url property with a China iOS-specific URL")]
	public string iosChinaUrl;

	[Tooltip("Override the above Url property with a Steam-specific URL")]
	public string steamUrl;

	public void Start()
	{
		if (!string.IsNullOrEmpty(steamUrl))
		{
			url = steamUrl;
		}
	}

	[TriggerableAction]
	public IEnumerator OpenURL()
	{
		Application.OpenURL(url);
		return null;
	}
}
