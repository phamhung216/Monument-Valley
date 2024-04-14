using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TouchLogger : MonoBehaviour
{
	private static List<LoggedTouch> _recordedStates;

	public bool record;

	public bool playback;

	private int _playbackIdx;

	private string _fileLocation = "Assets/Resources/";

	private string _logFolder = "TouchLogs/";

	private bool _hasLoadedSceneTouchXML;

	private static string _currentSceneName;

	private float _timeElapsed;

	private int _screenWidth;

	private int _screenHeight;

	public static bool HasUnsavedEvents
	{
		get
		{
			if (_recordedStates != null)
			{
				return _recordedStates.Count > 0;
			}
			return false;
		}
	}

	public static int UnsavedEventCount
	{
		get
		{
			if (_recordedStates == null)
			{
				return 0;
			}
			return _recordedStates.Count;
		}
	}

	public bool hasLoadedTouchXml => _hasLoadedSceneTouchXML;

	public int screenWidth => _screenWidth;

	public int screenHeight => _screenHeight;

	private void Start()
	{
		GameScene.instance.eventHandlers[SceneEvent.EndLevel].EventReceived += EndLevel;
		_hasLoadedSceneTouchXML = false;
		_currentSceneName = SceneManager.GetActiveScene().name;
		_timeElapsed = 0f;
		if (playback)
		{
			LoadEvents();
		}
		else if (record)
		{
			SetRecord();
		}
		_screenWidth = Screen.width;
		_screenHeight = Screen.height;
	}

	public void SetRecord()
	{
		_recordedStates = null;
		record = true;
		playback = false;
		if (record)
		{
			_recordedStates = new List<LoggedTouch>();
		}
	}

	private void Update()
	{
		float num = 0f;
		if (playback && _recordedStates != null)
		{
			while (_playbackIdx < _recordedStates.Count && Time.time > _recordedStates[_playbackIdx].timeStamp && (num == 0f || num == _recordedStates[_playbackIdx].timeStamp))
			{
				num = _recordedStates[_playbackIdx].timeStamp;
				GameTouch touch = _recordedStates[_playbackIdx].touch;
				GetComponent<TouchHandler>().SimulateEvent(touch);
				_playbackIdx++;
			}
		}
	}

	public void LogEvent(GameTouch touch)
	{
		if (record)
		{
			LoggedTouch loggedTouch = new LoggedTouch();
			loggedTouch.touch = touch.Clone();
			loggedTouch.timeStamp = Time.time;
			_recordedStates.Add(loggedTouch);
		}
	}

	public void SaveEvents()
	{
		if (record && _recordedStates.Count > 0)
		{
			TouchesXML touchesXML = new TouchesXML();
			touchesXML.touches = _recordedStates.ToArray();
			touchesXML.screenWidth = _screenWidth;
			touchesXML.screenHeight = _screenHeight;
			touchesXML.Save(GetLogScenePath());
		}
	}

	public void LoadEvents()
	{
		record = false;
		_playbackIdx = 0;
		string text = _logFolder + _currentSceneName;
		TextAsset textAsset = Resources.Load<TextAsset>(text);
		if (textAsset == null)
		{
			textAsset = Resources.Load<TextAsset>(text.Split(' ')[0]);
		}
		if (textAsset != null)
		{
			TouchesXML touchesXML = new TouchesXML();
			touchesXML = TouchesXML.LoadFromString(textAsset.text);
			_screenWidth = touchesXML.screenWidth;
			_screenHeight = touchesXML.screenHeight;
			_recordedStates = null;
			_recordedStates = new List<LoggedTouch>(touchesXML.touches);
			_hasLoadedSceneTouchXML = true;
		}
	}

	public string GetLogScenePath()
	{
		return _fileLocation + _logFolder + _currentSceneName + ".xml";
	}

	public void EndLevel()
	{
	}
}
