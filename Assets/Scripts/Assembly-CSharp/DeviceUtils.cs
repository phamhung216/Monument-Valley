using UnityEngine;

public class DeviceUtils
{
	private static string _deviceRegion = "";

	private static void CheckAndInitialiseDeviceClass()
	{
	}

	public static bool IsDeviceMusicPlaying()
	{
		return false;
	}

	public static void ShowQuitPrompt(string gameObjectName, string methodName)
	{
	}

	public static DeviceLanguage GetDeviceLanguage()
	{
		DeviceLanguage result = DeviceLanguage.English;
		switch (Application.systemLanguage)
		{
		case SystemLanguage.English:
			result = DeviceLanguage.English;
			break;
		case SystemLanguage.French:
			result = DeviceLanguage.French;
			break;
		case SystemLanguage.Italian:
			result = DeviceLanguage.Italian;
			break;
		case SystemLanguage.German:
			result = DeviceLanguage.German;
			break;
		case SystemLanguage.Spanish:
			result = DeviceLanguage.Spanish;
			break;
		case SystemLanguage.ChineseSimplified:
			result = DeviceLanguage.ChineseSimplified;
			break;
		case SystemLanguage.ChineseTraditional:
			result = DeviceLanguage.ChineseTraditional;
			break;
		case SystemLanguage.Japanese:
			result = DeviceLanguage.Japanese;
			break;
		case SystemLanguage.Korean:
			result = DeviceLanguage.Korean;
			break;
		case SystemLanguage.Russian:
			result = DeviceLanguage.Russian;
			break;
		case SystemLanguage.Portuguese:
			result = DeviceLanguage.Portuguese;
			break;
		case SystemLanguage.Swedish:
			result = DeviceLanguage.Swedish;
			break;
		case SystemLanguage.Turkish:
			result = DeviceLanguage.Turkish;
			break;
		case SystemLanguage.Dutch:
			result = DeviceLanguage.Dutch;
			break;
		case SystemLanguage.Thai:
			result = DeviceLanguage.Thai;
			break;
		}
		return result;
	}

	public static void StartActivityIndicator(int style)
	{
	}

	public static void StopActivityIndicator()
	{
	}

	public static bool IsGooglePlayServiceAvailable()
	{
		return false;
	}

	public static bool IsRunningART()
	{
		return false;
	}

	public static void ShowPopUp(string _title, string _message, string _confirmText, string _gameObjectName = null, string _methodName = null)
	{
	}

	public static bool HasNetworkConnection()
	{
		return Application.internetReachability != NetworkReachability.NotReachable;
	}

	public static string GetRegion()
	{
		return _deviceRegion;
	}
}
