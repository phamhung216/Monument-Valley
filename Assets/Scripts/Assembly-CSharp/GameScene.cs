using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameScene : MonoBehaviour
{
	public NavManager sceneNavManager;

	public Transform scenePanSpaceTransform;

	public NavIndicator sceneNavIndicator;

	public bool logSceneActionSequences;

	public TextAsset localisedStrings;

	public List<LocalisedFonts> localisedFonts;

	public SocialParams socialParams;

	public string socialString1;

	public string socialString2;

	public Camera invertedCamera;

	private List<AIController> _crows;

	private Dictionary<string, SceneSingleton> _singletonObjects;

	private Dictionary<SceneEvent, SceneEventHandler> _eventHandlers = new Dictionary<SceneEvent, SceneEventHandler>();

	private static GameScene s_instance;

	private GameObject _player;

	private DebugPanel _debugPanel;

	public bool isRunningTest;

	private static bool _seenLowMemoryMessageOnce;

	private LevelName _currenLevel;

	public Dictionary<SceneEvent, SceneEventHandler> eventHandlers => _eventHandlers;

	public static GameScene instance => s_instance;

	public static NavIndicator navIndicator => s_instance.sceneNavIndicator;

	public static NavManager navManager
	{
		get
		{
			if (!s_instance)
			{
				return null;
			}
			return s_instance.sceneNavManager;
		}
	}

	public static GameObject player => s_instance._player;

	public static Transform panSpaceTransform => s_instance.scenePanSpaceTransform;

	public static bool logActionSequences => s_instance.logSceneActionSequences;

	private void Awake()
	{
		s_instance = this;
		GameStartLogic.SetupQuality();
		_ = LevelManager.Instance;
		UserDataController.Instance.Initialise();
		LocalisationManager.Instance.LoadStringTable(localisedStrings, localisedFonts);
		foreach (SceneEvent value in Enum.GetValues(typeof(SceneEvent)))
		{
			_eventHandlers.Add(value, new SceneEventHandler());
		}
		_eventHandlers[SceneEvent.QuitLevel].EventReceived += OnQuitLevel;
		_eventHandlers[SceneEvent.QuitGame].EventReceived += OnQuitGame;
		_eventHandlers[SceneEvent.EndLevel].EventReceived += OnEndLevel;
		if (Debug.isDebugBuild)
		{
			_debugPanel = base.gameObject.AddComponent<DebugPanel>();
		}
	}

	private void Start()
	{
		_crows = new List<AIController>();
		AIController[] array = UnityEngine.Object.FindObjectsOfType<AIController>();
		for (int i = 0; i < array.Length; i++)
		{
			_crows.Add(array[i]);
		}
		_currenLevel = LevelName.None;
	}

	private void ResumeGameFromPopUp(string _param)
	{
		Time.timeScale = 1f;
	}

	private void OnDestroy()
	{
	}

	public void EnsurePlayer()
	{
		if (!_player)
		{
			SetPlayer(GameObject.Find("Player"));
		}
	}

	public void SetPlayer(GameObject player)
	{
		_player = player;
	}

	public static Vector3 WorldToPanPoint(Vector3 position)
	{
		return panSpaceTransform.InverseTransformPoint(position);
	}

	public static float ScreenToWorldLength(float _length)
	{
		return (Camera.main.ScreenToWorldPoint(new Vector3(0f, 0f, 0f)) - Camera.main.ScreenToWorldPoint(new Vector3(_length, 0f, 0f))).magnitude;
	}

	public static Vector3 ScreenToPanPoint(Vector3 position, Camera camera)
	{
		return WorldToPanPoint(camera.ScreenToWorldPoint(position));
	}

	public static Vector3 PanToWorldPoint(Vector3 position)
	{
		return panSpaceTransform.TransformPoint(position);
	}

	public static Ray PanToWorldRay(Vector3 position)
	{
		position.z = 0f;
		return new Ray(PanToWorldPoint(position), panSpaceTransform.forward);
	}

	public static Vector3 PanToScreenPoint(Vector3 position, Camera camera)
	{
		return camera.WorldToScreenPoint(PanToWorldPoint(position));
	}

	public static bool IsWorldPointOnScreen(Vector3 position, Camera cam, float toleranceMultiplier)
	{
		Vector3 vector = WorldToPanPoint(position);
		Vector3 vector2 = WorldToPanPoint(cam.transform.position);
		Vector3 vector3 = vector - vector2;
		vector3.z = 0f;
		CameraPanController component = Camera.main.GetComponent<CameraPanController>();
		if (Mathf.Abs(vector3.x) < toleranceMultiplier * 0.5f * component.bounds.size.x && Mathf.Abs(vector3.y) < toleranceMultiplier * 0.5f * component.bounds.size.y)
		{
			return true;
		}
		return false;
	}

	private void OnQuitGame()
	{
		Application.Quit();
	}

	public void OnQuitLevel()
	{
		LevelManager.Instance.showLevelComplete = false;
		LevelName currentLevel = LevelManager.CurrentLevel;
		if (currentLevel == LevelName.Windmill || currentLevel == LevelName.Windmill_blue)
		{
			LevelUnlockLogic.doNormalToWorldSelectSequence = true;
			LevelManager.Instance.LoadLevel(LevelManager.lastLevelSelect);
		}
		else if (LevelManager.Instance.levelData[currentLevel].expansion)
		{
			LevelManager.Instance.LoadLevel(LevelName.LevelSelectExpansion);
		}
		else
		{
			LevelManager.Instance.LoadLevel(LevelName.LevelSelect);
		}
	}

	public void OnEndLevel()
	{
		LevelName currentLevel = LevelManager.CurrentLevel;
		LevelManager.Instance.SetLevelCompleted(currentLevel);
		LevelProgress.ClearProgress();
		if (currentLevel == LevelName.Windmill || currentLevel == LevelName.Windmill_blue)
		{
			LevelUnlockLogic.doNormalToWorldSelectSequence = true;
			LevelManager.Instance.LoadLevel(LevelManager.lastLevelSelect);
		}
		else if (LevelManager.Instance.levelData[currentLevel].expansion)
		{
			if (currentLevel == LevelName.Rebuild)
			{
				LevelUnlockLogic.doNormalToWorldSelectSequence = true;
			}
			LevelManager.Instance.LoadLevel(LevelName.LevelSelectExpansion);
		}
		else
		{
			if (currentLevel == LevelName.Zen)
			{
				LevelUnlockLogic.doNormalToWorldSelectSequence = true;
			}
			LevelManager.Instance.LoadLevel(LevelName.LevelSelect);
		}
	}

	public GameObject GetSingleton(string name)
	{
		if (_singletonObjects == null)
		{
			_singletonObjects = new Dictionary<string, SceneSingleton>();
			SceneSingleton[] array = UnityEngine.Object.FindObjectsOfType(typeof(SceneSingleton)) as SceneSingleton[];
			foreach (SceneSingleton sceneSingleton in array)
			{
				_singletonObjects.Add(sceneSingleton.name, sceneSingleton);
			}
		}
		if (_singletonObjects.TryGetValue(name, out var value))
		{
			return value.gameObject;
		}
		return null;
	}

	private void Update()
	{
		if (_currenLevel == LevelName.None)
		{
			_currenLevel = LevelManager.CurrentLevel;
		}
		if (_currenLevel == LevelName.LevelSelect || _crows == null || _crows.Count <= 0)
		{
			return;
		}
		for (int i = 0; i < _crows.Count; i++)
		{
			AIController aIController = _crows[i];
			if (aIController != null)
			{
				float toleranceMultiplier = 3f;
				bool flag = IsWorldPointOnScreen(aIController.transform.position, Camera.main, toleranceMultiplier);
				if (!flag && invertedCamera != null)
				{
					flag = IsWorldPointOnScreen(aIController.transform.position, invertedCamera, toleranceMultiplier);
				}
				aIController.isOnScreen = flag;
			}
		}
	}

	[TriggerableAction(true)]
	public IEnumerator StartActivityIndicator()
	{
		yield return null;
	}
}
