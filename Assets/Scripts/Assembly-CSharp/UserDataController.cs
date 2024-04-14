using System;
using Steamworks;
using UnityEngine;

public class UserDataController
{
	private LevelManager _levelManager;

	public static string LastPlayedLevelKey = "lastPlayedLevel";

	private static UserDataController s_instance = null;

	private bool _isInitialised;

	public bool isInitialised => _isInitialised;

	public static UserDataController Instance
	{
		get
		{
			if (s_instance == null)
			{
				s_instance = new UserDataController();
			}
			return s_instance;
		}
	}

	protected UserDataController()
	{
	}

	public void Initialise()
	{
		if (!_isInitialised)
		{
			Log("Initialise");
			_levelManager = LevelManager.Instance;
			SharedPlayerPrefs.IntConflictDetected += OnIntConflictDetected;
			SharedPlayerPrefs.StringConflictDetected += OnStringConflictDetected;
			SharedPlayerPrefs.Sync();
			Load();
			_isInitialised = true;
		}
	}

	private void OnIntConflictDetected(string name, int mine, int theirs, out int resolved)
	{
		Log("OnIntConflictDetected");
		int num = Mathf.Max(mine, theirs);
		resolved = num;
	}

	private void OnStringConflictDetected(string name, string mine, string theirs, out string resolved)
	{
		Log("StringConflictDetected name " + name + " mine " + mine + " theirs " + theirs);
		resolved = mine;
	}

	private void Log(string _log)
	{
	}

	public void Load()
	{
		Log("Load");
		SyncState(saving: false);
	}

	public void Save()
	{
		Log("Save");
		SharedPlayerPrefs.Sync();
		SyncState(saving: true);
	}

	public void Reset()
	{
		Log("Reset");
		bool flag = false;
		InAppPurchaseLogic inAppPurchaseLogic = UnityEngine.Object.FindObjectOfType<InAppPurchaseLogic>();
		if ((bool)inAppPurchaseLogic)
		{
			flag = inAppPurchaseLogic.HasPurchasedContentPack1();
		}
		LevelProgress.ClearProgress();
		SharedPlayerPrefs.Reset();
		LevelName[] array = (LevelName[])Enum.GetValues(typeof(LevelName));
		foreach (LevelName levelName in array)
		{
			if (levelName != 0)
			{
				_levelManager.levelData[levelName].completed = 0;
				_levelManager.levelData[levelName].shownUnlock = 0;
			}
		}
		_levelManager.levelData[LevelName.PreGameLogic].completed = 1;
		if (flag && (bool)inAppPurchaseLogic)
		{
			InAppPurchaseLogic.SetContentPack1Purchased();
		}
		LevelManager.LastPlayedLevel = LevelName.None;
		AudioSettings.musicMuted = false;
		AudioSettings.soundMuted = false;
	}

	private void SyncState(bool saving)
	{
		LevelName[] array = (LevelName[])Enum.GetValues(typeof(LevelName));
		foreach (LevelName levelName in array)
		{
			if (levelName != 0)
			{
				string text = levelName.ToString("G");
				SyncKey(text + "_complete", ref _levelManager.levelData[levelName].completed, saving);
				SyncKey(text + "_shownUnlock", ref _levelManager.levelData[levelName].shownUnlock, saving);
			}
		}
		string stringValue = LevelManager.LastPlayedLevel.ToString("G");
		SyncKey(LastPlayedLevelKey, ref stringValue, saving);
		LevelManager.LastPlayedLevel = LevelManager.GetLevelNameForScene(stringValue);
		if (saving)
		{
			SharedPlayerPrefs.Save();
		}
	}

	private void SyncKey(string key, ref int intValue, bool saving)
	{
		if (saving)
		{
			SharedPlayerPrefs.SetInt(key, intValue);
		}
		else if (SharedPlayerPrefs.HasKey(key))
		{
			intValue = SharedPlayerPrefs.GetInt(key);
		}
	}

	private void SyncKey(string key, ref string stringValue, bool saving)
	{
		if (saving)
		{
			SharedPlayerPrefs.SetString(key, stringValue);
		}
		else if (SharedPlayerPrefs.HasKey(key))
		{
			stringValue = SharedPlayerPrefs.GetString(key);
		}
	}

	public void Synchronise()
	{
		SyncState(saving: true);
	}

	public static float GetCommonLocalPrefsFloat(string key)
	{
		return PlayerPrefs.GetFloat(key);
	}

	public static bool HasLocalUserPrefsKey(string key)
	{
		return PlayerPrefs.HasKey(GetUserSpecificKey(key));
	}

	public static int GetUserLocalPrefsInt(string key)
	{
		return PlayerPrefs.GetInt(GetUserSpecificKey(key));
	}

	public static int GetUserLocalPrefsInt(string key, int defaultValue)
	{
		return PlayerPrefs.GetInt(GetUserSpecificKey(key), defaultValue);
	}

	public static void SetUserLocalPrefsInt(string key, int value)
	{
		PlayerPrefs.SetInt(GetUserSpecificKey(key), value);
	}

	public static string GetUserLocalPrefsString(string key)
	{
		return PlayerPrefs.GetString(GetUserSpecificKey(key));
	}

	public static void SetUserLocalPrefsString(string key, string value)
	{
		PlayerPrefs.SetString(GetUserSpecificKey(key), value);
	}

	public static float GetUserLocalPrefsFloat(string key)
	{
		return PlayerPrefs.GetFloat(GetUserSpecificKey(key));
	}

	public static void SetUserLocalPrefsFloat(string key, float value)
	{
		PlayerPrefs.SetFloat(GetUserSpecificKey(key), value);
	}

	private static string GetUserSpecificKey(string baseKeyName)
	{
		return SteamUser.GetSteamID().ToString() + "_" + baseKeyName;
	}

	public static void SaveLocalPrefs()
	{
		PlayerPrefs.Save();
	}
}
