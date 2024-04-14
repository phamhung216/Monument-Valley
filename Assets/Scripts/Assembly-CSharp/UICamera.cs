using System.Collections.Generic;
using UnityCommon;
using UnityEngine;

public class UICamera : UILayout
{
	private class ScreenArchetype
	{
		public int width;

		public int height;

		public string name;

		public int targetWidth;

		public int targetHeight;

		public float aspectRatio => (float)width / (float)height;

		public ScreenArchetype(string name, int screenWidth, int screenHeight, int targetWidth, int targetHeight)
		{
			this.name = name;
			width = screenWidth;
			height = screenHeight;
			this.targetWidth = targetWidth;
			this.targetHeight = targetHeight;
		}
	}

	private ScreenArchetype _archetype;

	private List<ScreenArchetype> _archetypes = new List<ScreenArchetype>();

	private Vector2Int _lastScreenSize;

	private int _scale = 1;

	private CameraPanController _cameraPanController;

	private Vector2 _uiToPixelsScale;

	private Vector3 _worldToUIOffset;

	private Camera _uiCamera;

	private static int Height => Screen.height;

	private static int Width => Screen.width;

	public int scale => _scale;

	public void EnsureScreenArchetypeSelected()
	{
		if (_archetype == null)
		{
			CalculateSize();
		}
	}

	private void CalculateSize()
	{
		CalculateSize(new Vector2(Width, Height));
	}

	private void CalculateSize(Vector2 screenSize)
	{
		_archetypes.Clear();
		_archetypes.Add(new ScreenArchetype("iPhone", 640, 960, 640, 960));
		_archetypes.Add(new ScreenArchetype("iPad", 768, 1024, 768, 1024));
		_archetypes.Add(new ScreenArchetype("iPad Pro", 1668, 2388, 768, 1024));
		_archetypes.Add(new ScreenArchetype("PC", 1920, 1080, 1920, 1080));
		int num = Mathf.RoundToInt(screenSize.x);
		int num2 = Mathf.RoundToInt(screenSize.y);
		_lastScreenSize = new Vector2Int(num, num2);
		float num3 = (float)num / (float)num2;
		bool flag = num2 > num;
		ScreenArchetype screenArchetype = null;
		foreach (ScreenArchetype archetype in _archetypes)
		{
			if (archetype.height > archetype.width == flag && (screenArchetype == null || Mathf.Abs(screenArchetype.aspectRatio - num3) > Mathf.Abs(archetype.aspectRatio - num3)))
			{
				screenArchetype = archetype;
			}
		}
		_archetype = screenArchetype;
		float num4 = screenArchetype.targetWidth;
		float num5 = screenArchetype.targetHeight;
		if (flag)
		{
			num4 = screenArchetype.targetWidth;
			num5 = num4 / num3;
		}
		else
		{
			num5 = screenArchetype.targetHeight;
			num4 = num3 * num5;
		}
		_scale = Mathf.FloorToInt(Mathf.Max(((float)num + 0.5f) / (float)screenArchetype.targetWidth, ((float)num2 + 0.5f) / (float)screenArchetype.targetHeight));
		_scale = Mathf.FloorToInt(Mathf.Max(_scale, 1));
		layoutWidth = num4;
		layoutHeight = num5;
		if (num3 > 2.3333333f)
		{
			layoutWidth = layoutHeight * 2.3333333f;
		}
		GetCamera().orthographicSize = num5 / 2f;
		int width = Screen.width;
		int height = Screen.height;
		_uiToPixelsScale = new Vector2((float)width / num4, (float)height / num5);
		_worldToUIOffset = -base.transform.position;
		Vector4 one = Vector4.one;
		float num6 = 0.75f;
		float num7 = (float)Screen.width / (float)Screen.height;
		if (num7 > 1f)
		{
			one.x = num6 / num7;
		}
		else
		{
			one.y = num7 / num6;
		}
		Shader.SetGlobalVector("_screenAspectCompensation", one);
	}

	public Camera GetCamera()
	{
		if (!_uiCamera)
		{
			_uiCamera = GetComponent<Camera>();
			Transform transform = base.transform.Find("LandscapeCamera");
			if ((bool)transform)
			{
				transform.GetComponent<Camera>().enabled = false;
			}
			_uiCamera.enabled = true;
		}
		return _uiCamera;
	}

	public static bool IsTablet()
	{
		return (float)Width / Screen.dpi > 3f;
	}

	private new void Awake()
	{
		base.Awake();
		CalculateSize();
		FindChildren();
	}

	private void Start()
	{
		_cameraPanController = Object.FindObjectOfType<CameraPanController>();
		FindChildren();
		InitViewControllerContent(this);
		OnScreenSizeChanged();
	}

	private void InitViewControllerContent(UILayout layout)
	{
		UIViewController component = layout.GetComponent<UIViewController>();
		if ((bool)component)
		{
			component.InitContent(layout);
		}
		for (int i = 0; i < layout.childLayoutCount; i++)
		{
			InitViewControllerContent(layout.GetChild(i));
		}
	}

	private void UpdateViewControllerLayout(UILayout layout)
	{
		UIViewController component = layout.GetComponent<UIViewController>();
		if ((bool)component)
		{
			component.SetupLayout(layout);
		}
		for (int i = 0; i < layout.childLayoutCount; i++)
		{
			UpdateViewControllerLayout(layout.GetChild(i));
		}
	}

	private void UpdateLayout()
	{
		if ((bool)GameScene.navManager)
		{
			if (OrientationOverrideManager.IsLandscape())
			{
				GameScene.navManager.extraNavTouchArea = GameScene.ScreenToWorldLength(0f);
			}
			else if (IsTablet())
			{
				GameScene.navManager.extraNavTouchArea = GameScene.ScreenToWorldLength(4f);
			}
			else
			{
				GameScene.navManager.extraNavTouchArea = GameScene.ScreenToWorldLength(16f);
			}
		}
		UpdateViewControllerLayout(this);
		Unfinalise();
		Layout();
	}

	public void OnScreenSizeChanged()
	{
		CalculateSize();
		UpdateLayout();
		if ((bool)_cameraPanController)
		{
			_cameraPanController.OnScreenSizeChanged();
		}
	}

	public void EditorLayout(Vector2 screenSize)
	{
		Unfinalise();
		CalculateSize(screenSize);
		Awake();
		UpdateLayout();
	}

	public Vector2 ScreenToDPPoint(Vector3 screenPos)
	{
		return GetCamera().ScreenToWorldPoint(screenPos);
	}

	private void Update()
	{
		Vector2Int vector2Int = new Vector2Int(Screen.width, Screen.height);
		if (vector2Int != _lastScreenSize)
		{
			_lastScreenSize = vector2Int;
			OnScreenSizeChanged();
		}
		UpdateIsFinalised();
		Layout();
		if (!(_cameraPanController != null))
		{
			return;
		}
		if (_cameraPanController.zoom < 1.5f)
		{
			if (OrientationOverrideManager.IsLandscape())
			{
				GameScene.navManager.extraNavTouchArea = GameScene.ScreenToWorldLength(0f);
			}
			else if (IsTablet())
			{
				GameScene.navManager.extraNavTouchArea = GameScene.ScreenToWorldLength(16f);
			}
			else
			{
				GameScene.navManager.extraNavTouchArea = GameScene.ScreenToWorldLength(32f);
			}
		}
		else if (OrientationOverrideManager.IsLandscape())
		{
			GameScene.navManager.extraNavTouchArea = GameScene.ScreenToWorldLength(0f);
		}
		else if (IsTablet())
		{
			GameScene.navManager.extraNavTouchArea = GameScene.ScreenToWorldLength(0f);
		}
		else
		{
			GameScene.navManager.extraNavTouchArea = GameScene.ScreenToWorldLength(0f);
		}
	}

	public Vector2 LayoutToScreenSpace(Vector2 layoutVector)
	{
		return Vector2.Scale(layoutVector, _uiToPixelsScale);
	}

	public Vector3 ClampWorldSpacePointToScreenPixels(Vector3 worldPos)
	{
		Vector3 vector = worldPos + _worldToUIOffset;
		vector.x *= _uiToPixelsScale.x;
		vector.y *= _uiToPixelsScale.y;
		vector.x = Mathf.RoundToInt(vector.x);
		vector.y = Mathf.RoundToInt(vector.y);
		vector.x /= _uiToPixelsScale.x;
		vector.y /= _uiToPixelsScale.y;
		return vector - _worldToUIOffset;
	}
}
