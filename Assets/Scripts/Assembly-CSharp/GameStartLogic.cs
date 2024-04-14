using System.Collections;
using MVCommon;
using UnityCommon;
using UnityEngine;

public class GameStartLogic : MonoBehaviour
{
	private GoogleLVLChecker _googleLVL;

	private bool _hasValidLicense;

	public GameObject localisationLoader;

	private bool _loadedPrefab;

	private void Awake()
	{
		SetupQuality();
		if (true)
		{
			InstantitatePrefabs();
		}
		Screen.sleepTimeout = -1;
	}

	private void InstantitatePrefabs()
	{
		if (!_loadedPrefab)
		{
			_loadedPrefab = true;
			Object.Instantiate(Resources.Load<GameObject>("Prefabs/Audio")).name = "Audio";
		}
	}

	public static void SetupQuality()
	{
		if (!PerformanceSettings.Instance.isInitialised)
		{
			bool flag = false;
			bool flag2 = false;
			int maxFPS = UnityCommon.DeviceUtils.Instance.GetMaxFPS();
			flag = false;
			flag2 = true;
			QualitySettings.vSyncCount = 1;
			PerformanceSettings.Instance.Init(flag2 ? PerformanceSettings.AALevel._2 : PerformanceSettings.AALevel._0, flag ? 30 : maxFPS);
		}
	}

	public static IEnumerator StartLoadAfterAFrame()
	{
		yield return null;
		LevelManager instance = LevelManager.Instance;
		if (instance.IsLevelUnlocked(LevelName.LevelSelect))
		{
			LevelName lastCheckpointLevel = LevelProgress.GetLastCheckpointLevel();
			if (lastCheckpointLevel != 0)
			{
				LevelManager.Instance.LoadLevel(lastCheckpointLevel);
			}
			else if (LevelManager.LastPlayedLevel == LevelName.LevelSelectExpansion)
			{
				LevelManager.Instance.LoadLevel(LevelName.LevelSelectExpansion);
			}
			else
			{
				LevelManager.Instance.LoadLevel(LevelName.LevelSelect);
			}
		}
		else
		{
			instance.LoadLevel(LevelName.Prelude);
		}
	}

	private void OnDestroy()
	{
	}

	private void Start()
	{
		LaunchGame();
	}

	private void LaunchGame()
	{
		_ = LevelManager.Instance;
		UserDataController.Instance.Initialise();
		ScreenshotUtils.Init(letTheGameHandleScreenshots: false);
		Singleton<AchievementWrapper>.Instance.Init();
		StartCoroutine(StartLoadAfterAFrame());
	}

	public void CheckObbStatusOrRun()
	{
		LaunchGameForAndroid();
	}

	private void LaunchGameForAndroid()
	{
		InstantitatePrefabs();
		LaunchGame();
	}
}
