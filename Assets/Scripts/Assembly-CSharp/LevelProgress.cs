using System.Collections.Generic;
using UnityEngine;

public class LevelProgress : MonoBehaviour
{
	private class SaveGameIO
	{
		private static readonly string lastCheckpointKey = "LastCheckpoint";

		private static readonly string playTimeKey = "PlayTime";

		public static void Save(string checkpointProgressString, float playTime)
		{
			UserDataController.SetUserLocalPrefsString(lastCheckpointKey, checkpointProgressString);
			UserDataController.SetUserLocalPrefsFloat(playTimeKey, playTime);
			UserDataController.SaveLocalPrefs();
		}

		public static void Load(out string checkpointProgressString, out float playTime)
		{
			checkpointProgressString = UserDataController.GetUserLocalPrefsString(lastCheckpointKey);
			playTime = UserDataController.GetUserLocalPrefsFloat(playTimeKey);
		}
	}

	public Checkpoint defaultCheckpoint;

	public TriggerableActionSequence levelIntroSequence;

	public static LevelName GetLastCheckpointLevel()
	{
		SaveGameIO.Load(out var checkpointProgressString, out var _);
		string[] array = checkpointProgressString.Split(':');
		if (array.Length < 3)
		{
			return LevelName.None;
		}
		string text = array[0];
		return LevelManager.Instance.GetLevelIDFromName(text);
	}

	private static Checkpoint StartWithDefaultCheckpoint()
	{
		LevelProgress levelProgress = Object.FindObjectOfType<LevelProgress>();
		if ((bool)levelProgress && (bool)levelProgress.defaultCheckpoint)
		{
			StartWithCheckpoint(levelProgress.defaultCheckpoint);
			SaveCheckpoint(levelProgress.defaultCheckpoint);
			return levelProgress.defaultCheckpoint;
		}
		return null;
	}

	private static void StartWithCheckpoint(Checkpoint checkpoint)
	{
		LevelProgress levelProgress = Object.FindObjectOfType<LevelProgress>();
		if ((bool)checkpoint)
		{
			checkpoint.FastForwardToHere();
			GameScene.navManager.ScanAllConnections();
			checkpoint.startupLookAtPoint.LookAtWithZoomOutImmediate();
			if ((bool)levelProgress && (bool)levelProgress.levelIntroSequence && checkpoint.showLevelIntroSequence)
			{
				levelProgress.levelIntroSequence.TriggerActions();
			}
		}
	}

	public static Checkpoint FastForwardLevel()
	{
		SaveGameIO.Load(out var checkpointProgressString, out var playTime);
		string[] array = checkpointProgressString.Split(':');
		if (array.Length < 3)
		{
			return StartWithDefaultCheckpoint();
		}
		string text = array[0];
		if (LevelManager.Instance.GetLevelName(LevelManager.CurrentLevel) != text)
		{
			return StartWithDefaultCheckpoint();
		}
		Checkpoint checkpoint = FindCheckpointWithName(array[1]);
		LevelProgressStrand levelProgressStrand = null;
		for (int i = 2; i < array.Length; i++)
		{
			string text2 = array[i].Split('=')[0];
			string checkpointName = array[i].Split('=')[1];
			LevelProgressStrand[] array2 = (LevelProgressStrand[])Object.FindObjectsOfType(typeof(LevelProgressStrand));
			foreach (LevelProgressStrand levelProgressStrand2 in array2)
			{
				if (levelProgressStrand2.name == text2)
				{
					if (levelProgressStrand2.checkpoints.Contains(checkpoint))
					{
						levelProgressStrand = levelProgressStrand2;
					}
					else
					{
						levelProgressStrand2.FastForwardToCheckpoint(FindCheckpointWithName(checkpointName));
					}
					break;
				}
			}
		}
		if (levelProgressStrand == null || checkpoint == null)
		{
			return StartWithDefaultCheckpoint();
		}
		levelProgressStrand.FastForwardToCheckpoint(checkpoint);
		LevelManager.Instance.timeKeeper.DidFastForwardLevel(checkpoint, playTime);
		StartWithCheckpoint(checkpoint);
		return checkpoint;
	}

	public static void ClearProgress()
	{
		SaveCheckpoint(null);
	}

	public static void SaveCheckpoint(Checkpoint checkpoint)
	{
		if (TriggerAction.FastForward)
		{
			return;
		}
		List<Checkpoint> list = new List<Checkpoint>();
		LevelProgressStrand[] obj = (LevelProgressStrand[])Object.FindObjectsOfType(typeof(LevelProgressStrand));
		LevelProgressStrand levelProgressStrand = null;
		LevelProgressStrand[] array = obj;
		foreach (LevelProgressStrand levelProgressStrand2 in array)
		{
			Checkpoint currentCheckpoint = levelProgressStrand2.GetCurrentCheckpoint();
			list.Add(currentCheckpoint);
			if (currentCheckpoint == checkpoint)
			{
				levelProgressStrand = levelProgressStrand2;
			}
		}
		SaveProgress(LevelManager.CurrentLevel, checkpoint, list);
		LevelManager.Instance.timeKeeper.DidTriggerCheckpoint(checkpoint, levelProgressStrand ? levelProgressStrand.GetLastCheckpoint() : null);
	}

	private static Checkpoint FindCheckpointWithName(string checkpointName)
	{
		Checkpoint[] array = (Checkpoint[])Object.FindObjectsOfType(typeof(Checkpoint));
		foreach (Checkpoint checkpoint in array)
		{
			if (checkpoint.name == checkpointName)
			{
				return checkpoint;
			}
		}
		return null;
	}

	public static void SaveProgress(LevelName levelName, Checkpoint currentCheckpoint, List<Checkpoint> checkpoints)
	{
		DebugUtils.DebugAssert(currentCheckpoint == null || checkpoints.Count == 0 || checkpoints.Contains(currentCheckpoint));
		string levelName2;
		if ((bool)currentCheckpoint)
		{
			levelName2 = LevelManager.Instance.GetLevelName(levelName);
			levelName2 += ":";
			levelName2 += currentCheckpoint.name;
			LevelProgressStrand[] array = (LevelProgressStrand[])Object.FindObjectsOfType(typeof(LevelProgressStrand));
			foreach (LevelProgressStrand levelProgressStrand in array)
			{
				foreach (Checkpoint checkpoint in checkpoints)
				{
					if (levelProgressStrand.checkpoints.Contains(checkpoint))
					{
						levelName2 += ":";
						levelName2 += levelProgressStrand.name;
						levelName2 += "=";
						levelName2 += checkpoint.name;
						break;
					}
				}
			}
		}
		else
		{
			levelName2 = "NONE";
		}
		float playTime = 0f;
		if ((bool)currentCheckpoint)
		{
			playTime = LevelManager.Instance.timeKeeper.playTimeSinceLevelStarted;
		}
		SaveGameIO.Save(levelName2, playTime);
	}
}
