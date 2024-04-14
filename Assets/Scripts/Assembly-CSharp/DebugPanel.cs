using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DebugPanel : GameTouchable
{
	public const string folder = "Resources";

	public const string levelListFile = "extra-levels";

	private float _guiRectWidth = 400f;

	private Rect _guiRect;

	private bool _panelVisible;

	private bool _teleporting;

	private bool _levelListActive;

	private int _frames;

	private float _fps;

	private float _time;

	private string[] _extraLevels;

	private Vector2 _levelListScrollPosition;

	private float _scrollPosition;

	private static float hotspotDistance => (float)Mathf.Min(Screen.width, Screen.height) / 12f;

	private static Vector2 hotspot => new Vector2(0f, 0.8f * (float)Screen.height);

	private void Start()
	{
		_guiRect = new Rect(((float)Screen.width - _guiRectWidth) * 0.5f, 0f, _guiRectWidth, Screen.height - 50);
		Camera.main.GetComponent<TouchHandler>().RegisterNonPhysicalTouchable(this);
		TextAsset textAsset = Resources.Load("extra-levels", typeof(TextAsset)) as TextAsset;
		if (textAsset != null && textAsset.text != null && !string.IsNullOrEmpty(textAsset.text))
		{
			_extraLevels = textAsset.text.Split(new string[1] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
		}
	}

	public static bool HitTestTouch(GameTouch touch)
	{
		return Vector2.Distance(hotspot, touch.pos) <= hotspotDistance;
	}

	public override bool AcceptTouch(GameTouch touch)
	{
		if (_teleporting || HitTestTouch(touch))
		{
			return true;
		}
		return false;
	}

	public override float GetHitDistance(GameTouch touch, Ray worldRay)
	{
		return -1f;
	}

	public override void OnTouchBegan(GameTouch touch)
	{
		if (_teleporting)
		{
			GameScene.player.GetComponent<CharacterLocomotion>().TeleportPlayerToScreenPosition(Input.mousePosition);
			_teleporting = false;
		}
		else if (!_panelVisible)
		{
			_panelVisible = true;
		}
	}

	private void OnGUI()
	{
		float height = 60f;
		if (_teleporting)
		{
			GUILayout.BeginArea(_guiRect);
			GUILayout.Box("Teleporting ...");
			GUILayout.EndArea();
		}
		else
		{
			if (!_panelVisible)
			{
				return;
			}
			if (GUI.Button(new Rect(hotspot.x - hotspotDistance, (float)Screen.height - hotspot.y - hotspotDistance, 2f * hotspotDistance, 2f * hotspotDistance), "Close"))
			{
				_panelVisible = false;
				_teleporting = false;
			}
			GUILayout.BeginArea(_guiRect);
			GUILayout.BeginHorizontal();
			GUILayout.Box("Version: " + ApplicationInfo.bundleVersion);
			GUILayout.Box(_fps + " FPS");
			GUILayout.EndHorizontal();
			GUILayout.Box(ApplicationInfo.buildTime);
			if (GUILayout.Button("Teleport", GUILayout.Height(height)))
			{
				_panelVisible = false;
				_teleporting = true;
			}
			if (GUILayout.Button("Expansion Level Select", GUILayout.Height(height)))
			{
				SceneManager.LoadScene("LevelSelectExpansion");
			}
			if (_extraLevels != null && _extraLevels.Length != 0)
			{
				if (GUILayout.Button("Show/Hide levels", GUILayout.Height(height)))
				{
					_levelListActive = !_levelListActive;
				}
				if (_levelListActive)
				{
					GUIStyle gUIStyle = new GUIStyle(GUI.skin.verticalScrollbar);
					gUIStyle.fixedWidth = 70f;
					_levelListScrollPosition = GUILayout.BeginScrollView(_levelListScrollPosition, alwaysShowHorizontal: false, alwaysShowVertical: true, GUIStyle.none, gUIStyle, GUIStyle.none, (GUILayoutOption[])null);
					GUILayout.BeginVertical();
					if (GUILayout.Button("Level Select", GUILayout.Height(height)))
					{
						SceneManager.LoadScene("LevelSelect");
					}
					string[] extraLevels = _extraLevels;
					foreach (string text in extraLevels)
					{
						if (GUILayout.Button(text, GUILayout.Height(height)))
						{
							_panelVisible = false;
							SceneManager.LoadScene(text);
						}
					}
					GUILayout.EndVertical();
					GUILayout.EndScrollView();
				}
			}
			GUILayout.EndArea();
		}
	}

	private void Update()
	{
		if (_panelVisible)
		{
			_time += Time.deltaTime;
			_frames++;
			if (_time >= 1f)
			{
				_fps = _frames;
				_time = 0f;
				_frames = 0;
			}
		}
	}
}
