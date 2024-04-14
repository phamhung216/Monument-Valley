using System.Collections;
using UnityEngine;

public class LevelUnlockLogic : MonoBehaviour
{
	public Rotatable rotator;

	public TriggerableActionSequence normalSequence;

	public TriggerableActionSequence finaleSequence;

	public TriggerableActionSequence normalToWorldSelectSequence;

	public AutoRotator finaleAutoRotator;

	public GameObject finaleGround;

	public bool forceShowEnding;

	public bool forceShowLevelComplete;

	public bool forceShowAfterEnding;

	public static bool doNormalToWorldSelectSequence;

	public static bool debugMode;

	private UnlockableLevel[] unlockableLevels;

	[TriggerableAction]
	public IEnumerator ResetSave()
	{
		UserDataController.Instance.Reset();
		UpdateRotator(resetting: true);
		StartPrelude();
		return null;
	}

	[TriggerableAction]
	public IEnumerator ActivateDebugMode()
	{
		debugMode = true;
		UpdateRotator(resetting: true);
		return null;
	}

	[TriggerableAction]
	public IEnumerator DeActivateDebugMode()
	{
		debugMode = false;
		UpdateRotator(resetting: true);
		return null;
	}

	public void DisableScreenDimming()
	{
		Screen.sleepTimeout = -1;
	}

	public void EnableScreenDimming()
	{
		Screen.sleepTimeout = -2;
	}

	private void Awake()
	{
		forceShowEnding = LevelManager.Instance.showEnding || forceShowEnding;
		if (forceShowAfterEnding)
		{
			doNormalToWorldSelectSequence = true;
		}
	}

	private void Start()
	{
		GameScene.instance.eventHandlers[SceneEvent.DisableScreenDimming].EventReceived += DisableScreenDimming;
		GameScene.instance.eventHandlers[SceneEvent.EnableScreenDimming].EventReceived += EnableScreenDimming;
		unlockableLevels = Object.FindObjectsOfType(typeof(UnlockableLevel)) as UnlockableLevel[];
		UpdateRotator(resetting: false);
	}

	private LevelName GetNextPlayableLevel(LevelName lastLevel)
	{
		LevelName levelName = LevelName.None;
		if (lastLevel < LevelName.Zen && lastLevel != 0)
		{
			levelName = lastLevel + 1;
			if (levelName == LevelName.LevelSelect)
			{
				levelName = LevelName.Intro;
			}
		}
		return levelName;
	}

	private void OnApplicationPause(bool pauseStatus)
	{
		if (!pauseStatus)
		{
			UserDataController.Instance.Synchronise();
		}
	}

	private void UpdateRotator(bool resetting)
	{
		int num = 0;
		for (int i = 0; i < unlockableLevels.Length; i++)
		{
			unlockableLevels[i].RefreshState();
			if (LevelManager.Instance.IsLevelUnlocked(unlockableLevels[i].levelName) || debugMode)
			{
				num++;
			}
		}
		if ((bool)rotator)
		{
			int num2 = 9;
			rotator.maxRotation = rotator.snapAngle * (float)num2 + 15f;
			int num3 = (int)LevelManager.LastPlayedLevel;
			if (LevelManager.LastPlayedLevel >= LevelName.LevelSelectExpansion)
			{
				LevelManager.LastPlayedLevel = LevelName.LevelSelect;
				num3 = 3 + (num - 1);
			}
			if (forceShowEnding)
			{
				num3 = 12;
			}
			num3 = ((num3 > 2) ? (num3 - 3) : 0);
			rotator.currentAngle = rotator.snapAngle * (float)num3;
		}
		LevelName lastPlayedLevel = LevelManager.LastPlayedLevel;
		if (forceShowEnding)
		{
			LevelManager.Instance.showEnding = false;
			finaleGround.SetActive(value: true);
			rotator.minRotation = finaleAutoRotator.endAngle;
			rotator.GetComponent<MoverAudio>().audioEvent = "World/Movers/Global/Rotating/Auto/Normal";
			rotator.GetComponent<MoverAudio>().motionType = MoverAudio.MotionType.Rotate;
			StartCoroutine(finaleSequence.RunSequence());
			return;
		}
		if (!resetting)
		{
			if (doNormalToWorldSelectSequence)
			{
				doNormalToWorldSelectSequence = false;
				StartCoroutine(normalToWorldSelectSequence.RunSequence());
			}
			else
			{
				StartCoroutine(normalSequence.RunSequence());
			}
		}
		finaleGround.SetActive(value: false);
		AIController[] array = (AIController[])Object.FindObjectsOfType(typeof(AIController));
		for (int j = 0; j < array.Length; j++)
		{
			array[j].gameObject.SetActive(value: false);
		}
		if (!LevelManager.Instance.IsLevelCompleted(lastPlayedLevel) && !forceShowLevelComplete)
		{
			return;
		}
		UnlockableLevel button = GetButton(lastPlayedLevel);
		UnlockableLevel unlockableLevel = null;
		LevelName nextPlayableLevel = GetNextPlayableLevel(lastPlayedLevel);
		if (nextPlayableLevel != 0)
		{
			unlockableLevel = GetButton(nextPlayableLevel);
		}
		if (nextPlayableLevel == LevelName.None)
		{
			return;
		}
		bool flag = !LevelManager.Instance.HasShownLevelUnlock(nextPlayableLevel) || forceShowLevelComplete;
		if (flag && (bool)unlockableLevel)
		{
			unlockableLevel.ShowClosed();
		}
		if ((LevelManager.Instance.showLevelComplete || forceShowLevelComplete) && (bool)button)
		{
			if (flag)
			{
				StartCoroutine(button.firstCompleteSequence.RunSequence());
			}
			else
			{
				StartCoroutine(button.completeSequence.RunSequence());
			}
		}
	}

	private UnlockableLevel GetButton(LevelName level)
	{
		UnlockableLevel[] array = unlockableLevels;
		foreach (UnlockableLevel unlockableLevel in array)
		{
			if (unlockableLevel.levelName == level)
			{
				return unlockableLevel;
			}
		}
		return null;
	}

	private void StartPrelude()
	{
		for (int i = 0; i < unlockableLevels.Length; i++)
		{
			if (unlockableLevels[i].levelName == LevelName.Prelude)
			{
				unlockableLevels[i].button.Trigger();
				break;
			}
		}
	}

	[TriggerableAction]
	public IEnumerator StartNextLevelUnlockSequence()
	{
		LevelName lastPlayedLevel = LevelManager.LastPlayedLevel;
		LevelName nextPlayableLevel = GetNextPlayableLevel(lastPlayedLevel);
		UnlockableLevel button = GetButton(nextPlayableLevel);
		if (!LevelManager.Instance.HasShownLevelUnlock(nextPlayableLevel) || forceShowLevelComplete)
		{
			LevelManager.Instance.SetHasShownLevelUnlock(nextPlayableLevel);
			if ((bool)button)
			{
				button.animationTriggeredBySequence = true;
				StartCoroutine(button.unlockSequence.RunSequence());
			}
		}
		return null;
	}

	public void RefreshLevelsState()
	{
		for (int i = 0; i < unlockableLevels.Length; i++)
		{
			if (!unlockableLevels[i].animationTriggeredBySequence)
			{
				unlockableLevels[i].RefreshState();
			}
		}
	}

	[TriggerableAction]
	public IEnumerator SetNormalToWorldSelectSequence()
	{
		doNormalToWorldSelectSequence = true;
		return null;
	}
}
