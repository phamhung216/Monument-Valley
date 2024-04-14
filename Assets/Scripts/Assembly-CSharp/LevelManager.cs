using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager
{
	public bool showEnding;

	public bool showLevelComplete;

	public TimeKeeper timeKeeper = new TimeKeeper();

	private Dictionary<LevelName, LevelData> _levelData = new Dictionary<LevelName, LevelData>();

	private LevelName _lastPlayedLevel;

	private static LevelManager s_instance;

	private UserDataController _userDataController;

	private Dictionary<LevelName, string> _localisedLevelName = new Dictionary<LevelName, string>();

	private static LevelName _lastLevelSelect;

	public static LevelName lastLevelSelect
	{
		get
		{
			if (_lastLevelSelect != LevelName.LevelSelect && _lastLevelSelect != LevelName.LevelSelectExpansion)
			{
				return LevelName.LevelSelect;
			}
			return _lastLevelSelect;
		}
	}

	public static LevelManager Instance
	{
		get
		{
			if (s_instance == null)
			{
				s_instance = new LevelManager();
			}
			return s_instance;
		}
	}

	public static LevelName CurrentLevel => GetLevelNameForScene(SceneManager.GetActiveScene().name);

	public static LevelName LastPlayedLevel
	{
		get
		{
			return Instance._lastPlayedLevel;
		}
		set
		{
			Instance._lastPlayedLevel = value;
		}
	}

	public Dictionary<LevelName, LevelData> levelData => _levelData;

	private LevelManager()
	{
		LevelName[] array = (LevelName[])Enum.GetValues(typeof(LevelName));
		_userDataController = UserDataController.Instance;
		int num = array.Length;
		int num2 = 1;
		for (int i = 0; i < num; i++)
		{
			LevelName levelName = array[i];
			if (levelName != 0)
			{
				LevelData levelData = new LevelData();
				levelData.filename = array[i].ToString("G");
				levelData.sceneFilename = levelData.filename;
				if (levelName == LevelName.Volcano)
				{
					num2 = 1;
				}
				if (levelName != LevelName.LevelSelect && levelName != 0 && levelName != LevelName.PreGameLogic && levelName != LevelName.LevelSelectExpansion)
				{
					levelData.chapterNumber = num2;
					num2++;
				}
				if (i > 1)
				{
					levelData.precursors = new LevelName[1] { array[i - 1] };
				}
				_levelData[levelName] = levelData;
				string.Format("{0}: {1} (depends on {2})", levelName, levelData.filename, (levelData.precursors == null) ? "None" : levelData.precursors[0].ToString());
			}
		}
		_levelData[LevelName.Prelude].sceneFilename += " Export";
		_levelData[LevelName.Intro].sceneFilename += " Export";
		_levelData[LevelName.Intro].precursors[0] = LevelName.Prelude;
		_levelData[LevelName.Draggers].sceneFilename += " Export";
		_levelData[LevelName.GripRotate].sceneFilename += " Export";
		_levelData[LevelName.Spire].sceneFilename += " Export";
		_levelData[LevelName.Labyrinth].sceneFilename += " Export";
		_levelData[LevelName.Keep].sceneFilename += " Export";
		_levelData[LevelName.InsideBox].sceneFilename += " Export";
		_levelData[LevelName.Descent].sceneFilename += " Export";
		_levelData[LevelName.Zen].sceneFilename += " Export";
		_levelData[LevelName.PreGameLogic].completed = 1;
		_levelData[LevelName.Prelude].shownUnlock = 1;
		InitSecrets(LevelName.Intro, 1);
		InitSecrets(LevelName.Draggers, 1);
		InitSecrets(LevelName.GripRotate, 1);
		_levelData[LevelName.LevelSelectExpansion].expansion = true;
		_levelData[LevelName.Reintro].expansion = true;
		_levelData[LevelName.Volcano].expansion = true;
		_levelData[LevelName.Twister].expansion = true;
		_levelData[LevelName.Waterways].expansion = true;
		_levelData[LevelName.Thief].expansion = true;
		_levelData[LevelName.Crush].expansion = true;
		_levelData[LevelName.NSided].expansion = true;
		_levelData[LevelName.Rebuild].expansion = true;
		_levelData[LevelName.Crush].sceneFilename += " Export";
		_levelData[LevelName.Volcano].sceneFilename += " Export";
		_levelData[LevelName.Reintro].sceneFilename += " Export";
		_levelData[LevelName.Twister].sceneFilename += " Export";
		_levelData[LevelName.Waterways].sceneFilename += " Export";
		_levelData[LevelName.Thief].sceneFilename += " Export";
		_levelData[LevelName.Rebuild].sceneFilename += " Export";
		_levelData[LevelName.NSided].sceneFilename += " Export";
		_levelData[LevelName.Windmill].sceneFilename += " Export";
		_levelData[LevelName.Windmill].redSuits = true;
		_levelData[LevelName.Windmill].chapterNumber = 0;
		_levelData[LevelName.Windmill_blue].sceneFilename += " Export";
		_levelData[LevelName.Windmill_blue].chapterNumber = 0;
		_localisedLevelName[LevelName.Prelude] = "$(level_prelude_name)";
		_localisedLevelName[LevelName.Intro] = "$(level_intro_name)";
		_localisedLevelName[LevelName.Draggers] = "$(level_draggers_name)";
		_localisedLevelName[LevelName.GripRotate] = "$(level_griprotate_name)";
		_localisedLevelName[LevelName.Spire] = "$(level_spire_name)";
		_localisedLevelName[LevelName.Labyrinth] = "$(level_labyrinth_name)";
		_localisedLevelName[LevelName.Keep] = "$(level_keep_name)";
		_localisedLevelName[LevelName.InsideBox] = "$(level_insidebox_name)";
		_localisedLevelName[LevelName.Descent] = "$(level_descent_name)";
		_localisedLevelName[LevelName.Zen] = "$(level_zen_name)";
		_localisedLevelName[LevelName.Volcano] = "$(level_volcano_name)";
		_localisedLevelName[LevelName.Twister] = "$(level_twister_name)";
		_localisedLevelName[LevelName.Thief] = "$(level_thief_name)";
		_localisedLevelName[LevelName.Waterways] = "$(level_waterways_name)";
		_localisedLevelName[LevelName.Crush] = "$(level_crush_name)";
		_localisedLevelName[LevelName.Reintro] = "$(level_reintro_name)";
		_localisedLevelName[LevelName.NSided] = "$(level_nsided_name)";
		_localisedLevelName[LevelName.Rebuild] = "$(level_rebuild_name)";
		timeKeeper.Load();
	}

	private void InitSecrets(LevelName level, int numSecrets)
	{
		_levelData[level].secretStates = new int[numSecrets];
		for (int i = 0; i < numSecrets; i++)
		{
			_levelData[level].secretStates[i] = 0;
		}
	}

	public static LevelName GetLevelNameForScene(string sceneName)
	{
		LevelName result = LevelName.None;
		try
		{
			result = (LevelName)Enum.Parse(typeof(LevelName), sceneName);
		}
		catch (ArgumentException)
		{
			if (sceneName.EndsWith(" Export"))
			{
				string value = sceneName.Replace(" Export", "");
				try
				{
					result = (LevelName)Enum.Parse(typeof(LevelName), value);
				}
				catch (ArgumentException)
				{
				}
			}
		}
		return result;
	}

	public string GetSceneName(LevelName level)
	{
		return _levelData[level].sceneFilename;
	}

	public string GetLevelName(LevelName level)
	{
		return level.ToString("G");
	}

	public LevelName GetLevelIDFromName(string name)
	{
		return GetLevelNameForScene(name);
	}

	public void LoadLevel(LevelName level)
	{
		if (!GameScene.instance || !GameScene.instance.isRunningTest)
		{
			if ((level == LevelName.Windmill || level == LevelName.Windmill_blue) && (CurrentLevel == LevelName.LevelSelect || CurrentLevel == LevelName.LevelSelectExpansion))
			{
				_lastLevelSelect = CurrentLevel;
			}
			if ((CurrentLevel == LevelName.LevelSelect && level == LevelName.LevelSelect) || (level != 0 && level != LevelName.LevelSelect))
			{
				LastPlayedLevel = level;
				_userDataController.Save();
			}
			Analytics.LogEvent("PlayingLevel_" + level);
			timeKeeper.DidStartLevel();
			SceneLoader.nextLevelName = _levelData[level].sceneFilename;
			SceneManager.LoadScene("LoadingScene");
		}
	}

	public void SetLevelCompleted(LevelName level)
	{
		if (!GameScene.instance.isRunningTest)
		{
			showLevelComplete = true;
			if (level == LevelName.Zen)
			{
				showEnding = true;
			}
			LevelData levelData = _levelData[level];
			DebugUtils.DebugAssert(levelData != null);
			bool flag = levelData.completed != 1;
			timeKeeper.DidCompleteLevel(flag);
			if (flag)
			{
				levelData.completed = 1;
				_userDataController.Save();
			}
			AchievementManager.Instance.SyncAchievements();
		}
	}

	public TimeSpan RoundToNearestMinute(TimeSpan timeSpan)
	{
		return TimeSpan.FromMinutes(Mathf.CeilToInt((float)timeSpan.TotalSeconds / 60f));
	}

	public bool IsLevelCompleted(LevelName level)
	{
		if (!_levelData.ContainsKey(level))
		{
			return false;
		}
		LevelData obj = _levelData[level];
		DebugUtils.DebugAssert(obj != null);
		return obj.completed == 1;
	}

	public bool HasShownLevelUnlock(LevelName level)
	{
		if (level == LevelName.None)
		{
			return false;
		}
		LevelData obj = _levelData[level];
		DebugUtils.DebugAssert(obj != null);
		return obj.shownUnlock == 1;
	}

	public void SetHasShownLevelUnlock(LevelName level)
	{
		LevelData levelData = _levelData[level];
		DebugUtils.DebugAssert(levelData != null);
		if (levelData.shownUnlock != 1)
		{
			levelData.shownUnlock = 1;
			_userDataController.Save();
		}
	}

	public void SetAllLevelsComplete()
	{
		foreach (LevelData value in _levelData.Values)
		{
			value.completed = 1;
			value.shownUnlock = 1;
		}
		_userDataController.Save();
	}

	public bool IsLevelUnlocked(LevelName level)
	{
		if (level == LevelName.None)
		{
			return false;
		}
		LevelData obj = _levelData[level];
		DebugUtils.DebugAssert(obj != null);
		LevelName[] precursors = obj.precursors;
		foreach (LevelName key in precursors)
		{
			if (_levelData[key].completed == 0)
			{
				return false;
			}
		}
		return true;
	}

	public LevelName GetNextLevel(LevelName level)
	{
		bool flag = false;
		foreach (LevelName value in Enum.GetValues(typeof(LevelName)))
		{
			if (value == level)
			{
				flag = true;
			}
			else if (flag)
			{
				return value;
			}
		}
		return LevelName.None;
	}

	public LevelName[] GetLevelPrecursors(LevelName level)
	{
		if (level == LevelName.None)
		{
			return null;
		}
		LevelData obj = _levelData[level];
		DebugUtils.DebugAssert(obj != null);
		return obj.precursors;
	}

	public int GetLevelChapterNumber(LevelName level)
	{
		if (level == LevelName.None)
		{
			return -1;
		}
		LevelData obj = _levelData[level];
		DebugUtils.DebugAssert(obj != null);
		return obj.chapterNumber;
	}

	public int GetLevelSecretsFound(LevelName level)
	{
		if (level == LevelName.None)
		{
			return -1;
		}
		LevelData levelData = _levelData[level];
		DebugUtils.DebugAssert(levelData != null);
		int num = 0;
		if (levelData.secretStates != null)
		{
			int[] secretStates = levelData.secretStates;
			for (int i = 0; i < secretStates.Length; i++)
			{
				if (secretStates[i] > 0)
				{
					num++;
				}
			}
		}
		return num;
	}

	public int GetLevelTotalSecrets(LevelName level)
	{
		if (level == LevelName.None)
		{
			return -1;
		}
		LevelData levelData = _levelData[level];
		DebugUtils.DebugAssert(levelData != null);
		if (levelData.secretStates != null)
		{
			return levelData.secretStates.Length;
		}
		return 0;
	}

	public void SetLevelSecretCollected(LevelName level, int secretIdx)
	{
		if (level != 0)
		{
			LevelData obj = _levelData[level];
			DebugUtils.DebugAssert(obj != null);
			obj.secretStates[secretIdx] = 1;
			_userDataController.Save();
		}
	}

	public bool IsLevelSecretCollected(LevelName level, int secretIdx)
	{
		if (level == LevelName.None)
		{
			return false;
		}
		LevelData obj = _levelData[level];
		DebugUtils.DebugAssert(obj != null);
		return obj.secretStates[secretIdx] == 1;
	}

	public LevelName GetHighestUnlockedLevel()
	{
		LevelName[] array = (LevelName[])Enum.GetValues(typeof(LevelName));
		for (int num = array.Length - 1; num >= 0; num--)
		{
			LevelName levelName = array[num];
			if (IsLevelCompleted(levelName))
			{
				return levelName;
			}
		}
		return LevelName.None;
	}

	public bool IsExpansionLevel(LevelName level)
	{
		if (level == LevelName.None)
		{
			return false;
		}
		return _levelData[level].expansion;
	}

	public bool IsMainGameComplete()
	{
		return IsLevelCompleted(LevelName.Zen);
	}

	public void DoTests()
	{
		DebugUtils.DebugAssert(LevelName.Prelude == GetLevelNameForScene("Prelude"));
		DebugUtils.DebugAssert(LevelName.Prelude == GetLevelNameForScene("Prelude Export"));
		DebugUtils.DebugAssert(GetLevelNameForScene("PreludeExport") == LevelName.None);
	}

	public string GetLocalisedLevelName(LevelName level)
	{
		string result = "";
		if (_localisedLevelName.ContainsKey(level))
		{
			result = LocalisationManager.Instance.LocaliseString(_localisedLevelName[level]);
		}
		return result;
	}
}
