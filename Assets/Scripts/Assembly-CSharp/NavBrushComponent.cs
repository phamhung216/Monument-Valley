using System.Collections;
using UnityEngine;

public class NavBrushComponent : MonoBehaviour
{
	public enum Type
	{
		Flat = 0,
		Stairs = 1,
		Ladder = 2,
		Hole = 3,
		NoTotem = 4
	}

	public int zone;

	public Type type;

	public Vector3 normal = Vector3.up;

	public GameObject depthReferenceObject;

	public bool depthRefOnlyWhenConnected;

	public bool useRealDepth;

	public bool touchable = true;

	public bool touchableWhenFacingAway;

	public Transform navIndicatorPosition;

	public bool useNavIndicatorPositionForLocomotion;

	public int chunkIndex;

	public bool isStatic;

	public bool discarded;

	public bool debugMe;

	private Rect _panSpaceRect;

	private Vector3 _panSpacePos;

	private bool _isAccessibleByPlayer;

	private NavBoundaryComponent[] _boundaries;

	private static float _minRectWidth = 1.5f;

	private Vector3 _lastCalcPos;

	private Quaternion _lastRotation;

	public static Color[] colors = new Color[6]
	{
		Color.blue,
		Color.yellow,
		Color.cyan,
		Color.magenta,
		new Color(0.9f, 0.3f, 0.2f),
		new Color(0.3f, 0.2f, 0.4f)
	};

	public bool isAccessibleByPlayer
	{
		get
		{
			return _isAccessibleByPlayer;
		}
		set
		{
			_isAccessibleByPlayer = value;
		}
	}

	public NavBoundaryComponent[] boundaries => _boundaries;

	public Rect panSpaceBoundingRect => _panSpaceRect;

	public Vector3 panSpacePos => _panSpacePos;

	public bool hasPermanentConnections
	{
		get
		{
			if (_boundaries != null)
			{
				NavBoundaryComponent[] array = _boundaries;
				for (int i = 0; i < array.Length; i++)
				{
					if (array[i].permanentConnection != null)
					{
						return true;
					}
				}
			}
			return false;
		}
	}

	public void Init()
	{
		_boundaries = GetComponentsInChildren<NavBoundaryComponent>();
		for (int i = 0; i < _boundaries.Length; i++)
		{
			_boundaries[i].Init();
		}
		UpdateBoundaryZones();
		normal.Normalize();
		CalculateNavState();
	}

	private void Start()
	{
		if (_boundaries == null)
		{
			GameScene.navManager.Init();
		}
		DebugUtils.DebugAssert(_boundaries != null);
		if (!navIndicatorPosition)
		{
			navIndicatorPosition = base.transform;
		}
	}

	public Vector3 GetNavIndicatorPosition()
	{
		if (type == Type.Stairs)
		{
			return navIndicatorPosition.position + 0.1f * base.transform.TransformDirection(normal);
		}
		return navIndicatorPosition.position;
	}

	public Vector3 GetNavPosition()
	{
		if (!useNavIndicatorPositionForLocomotion)
		{
			return base.transform.position;
		}
		return navIndicatorPosition.position;
	}

	public void UpdateBoundaryZones()
	{
		if (zone == -1)
		{
			return;
		}
		NavBoundaryComponent[] array = _boundaries;
		foreach (NavBoundaryComponent navBoundaryComponent in array)
		{
			if (navBoundaryComponent.zone < 0)
			{
				navBoundaryComponent.zone = zone;
			}
		}
	}

	public bool IsConnectedToBrush(NavBrushComponent otherBrush)
	{
		DebugUtils.DebugAssert(otherBrush != null);
		NavBoundaryComponent[] array = _boundaries;
		foreach (NavBoundaryComponent navBoundaryComponent in array)
		{
			foreach (NavBrushLink link in navBoundaryComponent.links)
			{
				if (link.GetOtherBoundary(navBoundaryComponent).parentBrush == otherBrush)
				{
					return true;
				}
			}
		}
		return false;
	}

	public NavBrushLink GetLinkToBrush(NavBrushComponent otherBrush)
	{
		DebugUtils.DebugAssert(otherBrush != null);
		NavBoundaryComponent[] array = _boundaries;
		foreach (NavBoundaryComponent navBoundaryComponent in array)
		{
			for (int j = 0; j < navBoundaryComponent.links.Count; j++)
			{
				NavBrushLink navBrushLink = navBoundaryComponent.links[j];
				if (navBrushLink.GetOtherBoundary(navBoundaryComponent).parentBrush == otherBrush)
				{
					return navBrushLink;
				}
			}
		}
		return null;
	}

	public NavBoundaryComponent GetConnectionBoundary(NavBrushComponent otherBrush)
	{
		DebugUtils.DebugAssert(otherBrush != null);
		NavBoundaryComponent[] array = _boundaries;
		foreach (NavBoundaryComponent navBoundaryComponent in array)
		{
			for (int j = 0; j < navBoundaryComponent.links.Count; j++)
			{
				if (navBoundaryComponent.links[j].GetOtherBoundary(navBoundaryComponent).parentBrush == otherBrush)
				{
					return navBoundaryComponent;
				}
			}
		}
		return null;
	}

	private Rect CalculatePanSpaceBoundingRect(Vector3 position)
	{
		Vector3 vector = GameScene.WorldToPanPoint(position);
		Rect result = new Rect(vector.x, vector.y, 0f, 0f);
		bool flag = false;
		if (_boundaries != null)
		{
			NavBoundaryComponent[] array = _boundaries;
			foreach (NavBoundaryComponent navBoundaryComponent in array)
			{
				navBoundaryComponent.panSpacePos = GameScene.WorldToPanPoint(navBoundaryComponent.transform.position);
				navBoundaryComponent.flattenedPanSpaceDir = (Vector2)navBoundaryComponent.panSpacePos - (Vector2)vector;
				navBoundaryComponent.flattenedPanSpaceDir.Normalize();
				switch (navBoundaryComponent.frontFaceEligibility)
				{
				case NavBoundaryComponent.FrontFaceMode.WhenFront:
					navBoundaryComponent.hasFrontFace = type != Type.Stairs && vector.z - navBoundaryComponent.panSpacePos.z > 0.1f;
					break;
				case NavBoundaryComponent.FrontFaceMode.Always:
					navBoundaryComponent.hasFrontFace = true;
					break;
				}
				Vector2 vector2 = navBoundaryComponent.panSpacePos;
				if (!flag)
				{
					result.x = vector2.x;
					result.y = vector2.y;
					flag = true;
					continue;
				}
				if (vector2.x < result.xMin)
				{
					result.xMin = vector2.x;
				}
				else if (vector2.x > result.xMax)
				{
					result.xMax = vector2.x;
				}
				if (vector2.y < result.yMin)
				{
					result.yMin = vector2.y;
				}
				else if (vector2.y > result.yMax)
				{
					result.yMax = vector2.y;
				}
			}
		}
		float num = 0.2f;
		result.xMin -= num;
		result.yMin -= num;
		result.xMax += num;
		result.yMax += num;
		if (result.width < _minRectWidth)
		{
			result.xMin = vector.x - 0.5f * _minRectWidth;
			result.xMax = vector.x + 0.5f * _minRectWidth;
		}
		return result;
	}

	public void CalculateNavState()
	{
		Vector3 position = base.transform.position;
		Quaternion rotation = base.transform.rotation;
		if (_lastCalcPos != position || _lastRotation != rotation)
		{
			_panSpaceRect = CalculatePanSpaceBoundingRect(position);
			_panSpacePos = GameScene.WorldToPanPoint(position);
			_lastCalcPos = position;
			_lastRotation = rotation;
		}
	}

	public DoorComponent GetOwningDoor()
	{
		if (!base.transform.parent)
		{
			return null;
		}
		return base.transform.parent.GetComponent<DoorComponent>();
	}

	[TriggerableAction]
	public IEnumerator DisableTotemAccess()
	{
		type = Type.NoTotem;
		return null;
	}

	private void OnDrawGizmos()
	{
		Color color = Color.grey;
		if (!discarded && zone >= 0 && zone < colors.Length)
		{
			color = colors[zone];
		}
		float a = ((!discarded && base.gameObject.activeSelf) ? 1f : 0.3f);
		color.a = a;
		Gizmos.color = color;
		float radius = (isStatic ? 0.1f : 0.2f);
		Gizmos.DrawSphere(base.transform.position, radius);
		if (_isAccessibleByPlayer)
		{
			Gizmos.color = Color.green;
			Gizmos.DrawSphere(base.transform.position, 0.1f);
			Gizmos.color = color;
		}
		NavBoundaryComponent[] componentsInChildren = GetComponentsInChildren<NavBoundaryComponent>();
		foreach (NavBoundaryComponent navBoundaryComponent in componentsInChildren)
		{
			Gizmos.DrawLine(base.transform.position, navBoundaryComponent.transform.position);
		}
		if (depthReferenceObject != null)
		{
			Gizmos.color = Color.grey;
			Gizmos.DrawLine(base.transform.position, depthReferenceObject.transform.position);
			Vector3 vector = CalculateFromDepthReference(base.transform.position, depthReferenceObject.transform.position);
			color = Color.yellow;
			color.a = 0.3f;
			Gizmos.color = color;
			Gizmos.DrawLine(base.transform.position, vector);
			Gizmos.DrawSphere(vector - new Vector3(0f, -0.5f, 0f), 0.5f);
		}
	}

	private void OnDrawGizmosSelected()
	{
		if ((bool)GameScene.instance)
		{
			float extraNavTouchArea = GameScene.navManager.extraNavTouchArea;
			if (extraNavTouchArea != 0f)
			{
				Gizmos.color = Color.yellow;
				Rect rect = panSpaceBoundingRect;
				rect.width += 2f * extraNavTouchArea;
				rect.height += 2f * extraNavTouchArea;
				rect.center = panSpaceBoundingRect.center;
				Vector3 vector = GameScene.PanToWorldPoint(new Vector3(rect.xMin, rect.yMin, 0f));
				Vector3 vector2 = GameScene.PanToWorldPoint(new Vector3(rect.xMax, rect.yMin, 0f));
				Vector3 vector3 = GameScene.PanToWorldPoint(new Vector3(rect.xMax, rect.yMax, 0f));
				Vector3 vector4 = GameScene.PanToWorldPoint(new Vector3(rect.xMin, rect.yMax, 0f));
				Gizmos.DrawLine(vector, vector2);
				Gizmos.DrawLine(vector2, vector3);
				Gizmos.DrawLine(vector3, vector4);
				Gizmos.DrawLine(vector4, vector);
			}
			Gizmos.color = Color.white;
			Rect rect2 = panSpaceBoundingRect;
			Vector3 vector5 = GameScene.PanToWorldPoint(new Vector3(rect2.xMin, rect2.yMin, 0f));
			Vector3 vector6 = GameScene.PanToWorldPoint(new Vector3(rect2.xMax, rect2.yMin, 0f));
			Vector3 vector7 = GameScene.PanToWorldPoint(new Vector3(rect2.xMax, rect2.yMax, 0f));
			Vector3 vector8 = GameScene.PanToWorldPoint(new Vector3(rect2.xMin, rect2.yMax, 0f));
			Gizmos.DrawLine(vector5, vector6);
			Gizmos.DrawLine(vector6, vector7);
			Gizmos.DrawLine(vector7, vector8);
			Gizmos.DrawLine(vector8, vector5);
		}
		Vector3 up = base.transform.up;
		Gizmos.color = Color.green;
		Gizmos.DrawRay(base.transform.position, up);
		Vector3 vector9 = base.transform.TransformDirection(normal);
		if (base.transform.up != vector9)
		{
			Gizmos.color = Color.grey;
			Gizmos.DrawRay(base.transform.position, vector9);
		}
	}

	public void SetDone()
	{
	}

	private Vector3 CalculateFromDepthReference(Vector3 currentPos, Vector3 refPos)
	{
		Vector3 vector = new Vector3(currentPos.x, currentPos.y, currentPos.z);
		Vector3 rhs = new Vector3(refPos.x, refPos.y - 0.5f, refPos.z);
		Vector3 forward = Camera.main.transform.forward;
		float num = Vector3.Dot(forward, currentPos);
		float num2 = Vector3.Dot(forward, rhs);
		return vector + forward * (num2 - num);
	}
}
