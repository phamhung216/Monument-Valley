using System;
using UnityEngine;

public class TimeKeeper
{
	private float _levelStartTime;

	private float _levelDuration;

	private float _totalPlayDuration;

	private string _totalPlayDurationKey = "TotalPlayTime";

	public float levelDuration => _levelDuration;

	public float playTimeSinceLevelStarted => Time.time - _levelStartTime;

	public void Load()
	{
		_totalPlayDuration = UserDataController.GetUserLocalPrefsFloat(_totalPlayDurationKey);
	}

	public void DidStartLevel()
	{
		_levelStartTime = Time.time;
		_levelDuration = 0f;
	}

	public void DidFastForwardLevel(Checkpoint startCheckpoint, float savedTime)
	{
		_levelStartTime = Time.time - savedTime;
		_levelDuration = savedTime;
		startCheckpoint.triggerTime = _levelStartTime;
	}

	public void DidCompleteLevel(bool firstCompletion)
	{
		_levelDuration = playTimeSinceLevelStarted;
		_totalPlayDuration += _levelDuration;
		UserDataController.SetUserLocalPrefsFloat(_totalPlayDurationKey, _totalPlayDuration);
		UserDataController.SaveLocalPrefs();
		if (firstCompletion)
		{
			LogFirstComplete(LevelManager.CurrentLevel, _levelDuration);
		}
	}

	public void DidTriggerCheckpoint(Checkpoint checkpoint, Checkpoint lastCheckpointInStrand)
	{
		if ((bool)checkpoint)
		{
			checkpoint.triggerTime = playTimeSinceLevelStarted;
			float checkpointDuration = 0f;
			if ((bool)lastCheckpointInStrand)
			{
				checkpointDuration = checkpoint.triggerTime - lastCheckpointInStrand.triggerTime;
			}
			LogCheckpointDuration(checkpoint, checkpointDuration);
		}
	}

	private void LogCheckpointDuration(Checkpoint checkpoint, float checkpointDuration)
	{
		LogTimeEvent(LevelManager.CurrentLevel, "CP", checkpoint.name, checkpointDuration);
	}

	private void LogTimeEvent(LevelName level, string eventKey, string eventValue, float seconds)
	{
		string levelName = LevelManager.Instance.GetLevelName(level);
		TimeSpan timeSpan = TimeSpan.FromSeconds(seconds);
		timeSpan = LevelManager.Instance.RoundToNearestMinute(timeSpan);
		string paramValue = $"{timeSpan.Hours:D2}:{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}";
		Analytics.LogPivotEventWithVars("Level_" + levelName + "_" + eventKey, eventKey, eventValue, null);
		Analytics.LogPivotEventWithVars("Level_" + levelName + "_" + eventKey + "_" + eventValue, "Time", paramValue, null);
		string[] attributeNames = new string[1] { eventKey };
		string[] attributeValues = new string[1] { eventValue };
		string[] metricNames = new string[1] { "Time" };
		string[] metricValues = new string[1] { timeSpan.TotalMinutes.ToString() };
		Analytics.LogEventWithAttributesAndMetrics("Level_" + levelName, attributeNames, attributeValues, metricNames, metricValues);
	}

	private void LogFirstComplete(LevelName level, float duration)
	{
		string levelName = LevelManager.Instance.GetLevelName(level);
		Analytics.LogPivotEventWithVars("LevelFirstComplete", "Level", levelName, null);
		Analytics.LogEventWithAttributesAndMetrics("LevelFirstComplete", new string[1] { "Level" }, new string[1] { levelName }, null, null);
		TimeSpan timeSpan = TimeSpan.FromSeconds(duration);
		timeSpan = LevelManager.Instance.RoundToNearestMinute(timeSpan);
		string paramValue = $"{timeSpan.Hours:D2}:{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}";
		Analytics.LogPivotEventWithVars("LevelFirstComplete_" + levelName, "CompleteTime", paramValue, null);
		string[] array = new string[2] { "CompleteTime", null };
		string[] array2 = new string[2]
		{
			timeSpan.TotalMinutes.ToString(),
			null
		};
		timeSpan = TimeSpan.FromSeconds(_totalPlayDuration);
		timeSpan = LevelManager.Instance.RoundToNearestMinute(timeSpan);
		paramValue = $"{timeSpan.Hours:D2}:{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}";
		Analytics.LogPivotEventWithVars("LevelFirstComplete_" + levelName + "_TotalTime", "TotalPlayTime", paramValue, null);
		array[1] = "TotalPlayTime";
		array2[1] = timeSpan.TotalMinutes.ToString();
		Analytics.LogEventWithAttributesAndMetrics("LevelFirstComplete_" + levelName, null, null, array, array2);
	}
}
