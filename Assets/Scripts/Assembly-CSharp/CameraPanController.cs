using System.Collections;
using System.Collections.Generic;
using UnityCommon;
using UnityEngine;

public class CameraPanController : GameTouchable
{
	public enum Mode
	{
		Cinematic = 0,
		Pan = 1,
		Normal = 2,
		Fresco = 3
	}

	private class ZoomInfo
	{
		public float nearZoom;

		public float defaultZoom;

		public float farZoom;
	}

	private List<GameTouch> _touches = new List<GameTouch>();

	private Vector2 _dragBeginTouchPos;

	private Vector3 _dragBeginWorldPos;

	private Vector3 _dragBeginCamPos;

	private Vector2 _lastDragTouchPos;

	private float _dragBeginTouchSeparation;

	private float _dragBeginViewSize;

	private float _minViewSize;

	private float _refViewSize;

	private float _maxViewSize;

	public float mouseScrollWheelZoomSensitivity = 10f;

	public float rubberBandAllowance = 0.3f;

	public bool doubleTapToZoom;

	private bool allowZoom = true;

	public CameraPanBounds bounds;

	public AnimationCurveDefinition panCurve;

	private CameraLookAtPoint _lookAtPoint;

	private bool _doRubberBanding = true;

	private bool _allowFreeZoom;

	private bool _allowMouseWheelZooming;

	private bool _isMouseWheelZooming;

	private bool _hasMouseWheelFocus;

	private Matrix4x4 _initialXform;

	private Easer _zoomSnapEaser = new Easer();

	private Vector3 _zoomStartPos;

	private Vector3 _zoomEndPos;

	private float _lastMouseScrollTime;

	private float _lastZoomViewSize;

	public DragHistory _dragHistory = new DragHistory();

	private Mode _mode = Mode.Normal;

	private Dictionary<Mode, ZoomInfo> zoomModes = new Dictionary<Mode, ZoomInfo>();

	private Rect _screenRect;

	private bool _canActivateCameraMode;

	private float _minCameraModeViewSize;

	private Camera _camera;

	private TouchHandler _touchHandler;

	public CameraLookAtPoint lookAtPoint
	{
		get
		{
			return _lookAtPoint;
		}
		set
		{
			_lookAtPoint = value;
		}
	}

	public Mode mode
	{
		get
		{
			return _mode;
		}
		set
		{
			_mode = value;
			UpdateZoomConstraints();
		}
	}

	public bool allowFreeZoom
	{
		get
		{
			return _allowFreeZoom;
		}
		set
		{
			if (!value)
			{
				StartZoomSnap();
				_canActivateCameraMode = false;
			}
			_allowFreeZoom = value;
			allowZoom = _allowFreeZoom || OrientationOverrideManager.IsPortrait();
		}
	}

	public bool canActivateCameraMode => _canActivateCameraMode;

	public float visibleAspectRatio => Mathf.Min(_camera.aspect, 2.3333333f);

	public float zoom => _refViewSize / _camera.orthographicSize;

	public void SetHasMouseWheelFocus(bool value)
	{
		_hasMouseWheelFocus = value;
	}

	public CameraPanController()
	{
		claimOnTouchBegan = true;
		claimOnTouchNotTap = true;
		releaseOnTouchNotTap = false;
	}

	private void UpdateZoomConstraints()
	{
		_maxViewSize = _refViewSize / zoomModes[_mode].farZoom;
		_minViewSize = _refViewSize / zoomModes[_mode].nearZoom;
		_minCameraModeViewSize = _refViewSize / (zoomModes[_mode].farZoom * 3f);
	}

	public void OnScreenSizeChanged()
	{
		_refViewSize = CameraAspectController.CalculateReferenceOrthographicSize();
		_camera.orthographicSize = _refViewSize;
		_lastZoomViewSize = _refViewSize;
		UpdateZoomConstraints();
		_screenRect = new Rect(0f, 0f, Screen.width, Screen.height);
		bounds.OnScreenSizeChanged();
		UpdateIsMouseWheelAllowed();
	}

	private void UpdateIsMouseWheelAllowed()
	{
		allowZoom = _allowFreeZoom || OrientationOverrideManager.IsPortrait();
		_allowMouseWheelZooming = OrientationOverrideManager.IsLandscape();
	}

	private void Awake()
	{
		zoomModes[Mode.Normal] = new ZoomInfo();
		zoomModes[Mode.Normal].defaultZoom = 1f;
		zoomModes[Mode.Normal].nearZoom = 2f;
		zoomModes[Mode.Normal].farZoom = 0.952381f;
		zoomModes[Mode.Pan] = new ZoomInfo();
		zoomModes[Mode.Pan].defaultZoom = zoomModes[Mode.Normal].defaultZoom;
		zoomModes[Mode.Pan].nearZoom = zoomModes[Mode.Normal].defaultZoom;
		zoomModes[Mode.Pan].farZoom = zoomModes[Mode.Normal].defaultZoom;
		zoomModes[Mode.Cinematic] = new ZoomInfo();
		zoomModes[Mode.Cinematic].defaultZoom = zoomModes[Mode.Normal].defaultZoom;
		zoomModes[Mode.Cinematic].nearZoom = 2f;
		zoomModes[Mode.Cinematic].farZoom = 0.952381f;
		zoomModes[Mode.Fresco] = new ZoomInfo();
		zoomModes[Mode.Fresco].defaultZoom = zoomModes[Mode.Normal].defaultZoom;
		zoomModes[Mode.Fresco].nearZoom = zoomModes[Mode.Normal].defaultZoom;
		zoomModes[Mode.Fresco].farZoom = 0.8f;
		_zoomSnapEaser.curve = panCurve.curve;
		_camera = GetComponent<Camera>();
		_refViewSize = _camera.orthographicSize;
		_lastZoomViewSize = _refViewSize;
		_initialXform = base.transform.localToWorldMatrix;
		_screenRect = new Rect(0f, 0f, Screen.width, Screen.height);
		mode = Mode.Normal;
	}

	private void Start()
	{
		_touchHandler = GameScene.instance.GetComponentInChildren<TouchHandler>();
		UpdateIsMouseWheelAllowed();
	}

	private void Update()
	{
		if (isEnabled)
		{
			if (!IsPanning() && _mode == Mode.Pan)
			{
				mode = Mode.Normal;
				StartZoomSnap();
			}
			_doRubberBanding = !IsPanning() && _mode == Mode.Normal;
			UpdateZoom();
			UpdatePan();
		}
	}

	private void UpdatePan()
	{
		if (_touches.Count > 0 || _isMouseWheelZooming)
		{
			UpdateDrag();
		}
		else
		{
			UpdateSettle();
		}
	}

	private void UpdateDrag()
	{
		if (IsPanning() || _mode != Mode.Normal)
		{
			_touchHandler.CancelTouches();
			return;
		}
		Vector2 vector = GetDragPos();
		if (_isMouseWheelZooming)
		{
			vector = Input.mousePosition;
		}
		Vector3 vector2 = _camera.ScreenToWorldPoint(vector) - _dragBeginWorldPos;
		base.transform.position -= vector2;
		Vector2 vector3 = 2f * _camera.orthographicSize * new Vector2(visibleAspectRatio, 1f);
		DoRubberBanding(bounds.transform.localToWorldMatrix, (bounds.size.x > vector3.x) ? bounds.size : vector3);
		float num = zoomModes[Mode.Normal].defaultZoom / zoomModes[Mode.Normal].farZoom;
		Vector2 vector4 = bounds.size.y * new Vector2(2.3333333f, 1f) * num;
		Vector3 position = bounds.transform.InverseTransformPoint(base.transform.position);
		position.x = Mathf.Clamp(position.x, 0f - (0.5f * vector4.x - 0.5f * vector3.x), 0.5f * vector4.x - 0.5f * vector3.x);
		base.transform.position = bounds.transform.TransformPoint(position);
		_dragHistory.AddDatum(Time.time, base.transform.localPosition);
	}

	private Rect GetPanSpaceCamCentreBoundingRect()
	{
		Rect result = default(Rect);
		result.center = bounds.transform.localPosition;
		result.width = bounds.size.x;
		result.height = bounds.size.y;
		return result;
	}

	private void UpdateSettle()
	{
		_dragHistory.DecayMomentum(Time.deltaTime);
		base.transform.localPosition = base.transform.localPosition + _dragHistory.momentum * Time.deltaTime;
		if (!_doRubberBanding)
		{
			return;
		}
		_ = 2f * _camera.orthographicSize * new Vector2(visibleAspectRatio, 1f);
		Vector3 vector = base.transform.localPosition - bounds.transform.localPosition;
		Vector3 vector2 = -GetMoveBackLS(bounds.size, vector);
		float num = 30f;
		float num2 = rubberBandAllowance * _camera.orthographicSize;
		float num3 = vector2.magnitude / num2;
		if (num3 > 1f)
		{
			_dragHistory.ClearMomentum();
		}
		Vector3 vector3 = -1f * num * num3 * vector2.normalized * Time.deltaTime;
		for (int i = 0; i < 2; i++)
		{
			if (Mathf.Abs(vector3[i]) > Mathf.Abs(vector2[i]))
			{
				vector3[i] = 0f - vector2[i];
			}
		}
		vector += vector3;
		base.transform.localPosition = bounds.transform.localPosition + vector;
	}

	private void UpdateZoom()
	{
		float num = _camera.orthographicSize;
		if (_allowMouseWheelZooming)
		{
			if (allowZoom && _hasMouseWheelFocus)
			{
				if (_screenRect.Contains(Input.mousePosition))
				{
					float axis = Input.GetAxis("Mouse ScrollWheel");
					if (axis != 0f)
					{
						num = _camera.orthographicSize - mouseScrollWheelZoomSensitivity * axis;
						_lastMouseScrollTime = Time.time;
						_lastZoomViewSize = num;
						if (!_isMouseWheelZooming)
						{
							_dragBeginWorldPos = _camera.ScreenToWorldPoint(Input.mousePosition);
							_isMouseWheelZooming = true;
						}
					}
				}
			}
			else
			{
				_isMouseWheelZooming = false;
			}
		}
		if (_touches.Count == 2)
		{
			float magnitude = (_touches[0].pos - _touches[1].pos).magnitude;
			num = (_lastZoomViewSize = _dragBeginViewSize * (_dragBeginTouchSeparation / magnitude));
		}
		float num2 = Time.time - _lastMouseScrollTime;
		float num3 = (allowFreeZoom ? 0f : 0.2f);
		float num4 = num3 + 0.1f;
		if (num3 < num2 && num2 < num4 && !_zoomSnapEaser.isRunning)
		{
			_isMouseWheelZooming = false;
			if (!allowFreeZoom)
			{
				StartZoomSnap();
			}
		}
		if (_zoomSnapEaser.isRunning)
		{
			float value = _zoomSnapEaser.value;
			num = _refViewSize / value;
			float num5 = _zoomSnapEaser.timeParam;
			if (num5 >= 1f)
			{
				_zoomSnapEaser.End();
				num5 = 1f;
			}
			base.transform.localPosition = Vector3.Lerp(_zoomStartPos, _zoomEndPos, panCurve.curve.Evaluate(num5));
		}
		num = Mathf.Clamp(num, _minViewSize, _maxViewSize);
		if (allowFreeZoom)
		{
			num = Mathf.Clamp(_lastZoomViewSize, _minCameraModeViewSize, _maxViewSize);
		}
		else
		{
			_lastZoomViewSize = num;
			if (_mode == Mode.Normal && num < Mathf.Lerp(_maxViewSize, _minViewSize, 0.1f) && !_zoomSnapEaser.isRunning && (_lastMouseScrollTime != 0f || _touches.Count == 2))
			{
				_canActivateCameraMode = true;
			}
			else
			{
				_canActivateCameraMode = false;
			}
		}
		_camera.orthographicSize = num;
	}

	public override bool AcceptTouch(GameTouch touch)
	{
		if (_mode != Mode.Normal)
		{
			return false;
		}
		return base.AcceptTouch(touch);
	}

	public override bool ReleaseOnTapEnded(GameTouch touch)
	{
		if (base.ReleaseOnTapEnded(touch))
		{
			OnTouchEnded(touch);
			return true;
		}
		return false;
	}

	public override void OnTouchBegan(GameTouch touch)
	{
		if ((allowZoom || _touches.Count < 1) && _touches.Count < 2)
		{
			_touches.Add(touch);
			StartDrag();
			if (doubleTapToZoom)
			{
				_ = touch.tapCount;
				_ = 2;
			}
		}
	}

	public override void OnTouchEnded(GameTouch touch)
	{
		OnTouchCancelled(touch);
		if (_touches.Count == 0 && touch.tapCount == 2)
		{
			OnDoubleTap();
		}
	}

	public override void OnTouchCancelled(GameTouch touch)
	{
		_touches.Remove(touch);
		if (_touches.Count >= 1)
		{
			StartDrag();
		}
		if (_touches.Count == 1)
		{
			_dragHistory.ClearMomentum();
			if (allowFreeZoom)
			{
				StartZoomSnap();
			}
			else
			{
				StartZoomSnapToCentre();
			}
		}
	}

	private void OnDoubleTap()
	{
		if (doubleTapToZoom && allowZoom)
		{
			if (zoom >= zoomModes[_mode].defaultZoom)
			{
				StartZoomIn(0.2f);
			}
			else
			{
				StartZoomOut(0.2f);
			}
		}
	}

	private void StartDrag()
	{
		_dragBeginTouchPos = GetDragPos();
		if (_touches.Count == 2)
		{
			_dragBeginTouchSeparation = (_touches[0].pos - _touches[1].pos).magnitude;
		}
		else
		{
			_dragBeginTouchSeparation = 0f;
		}
		_dragBeginViewSize = _camera.orthographicSize;
		_dragBeginWorldPos = _camera.ScreenToWorldPoint(new Vector3(_dragBeginTouchPos.x, _dragBeginTouchPos.y, 0f));
		_dragBeginCamPos = base.transform.position;
	}

	private Vector2 GetDragPos()
	{
		Vector2 result = new Vector2(0f, 0f);
		foreach (GameTouch touch in _touches)
		{
			result += touch.pos;
		}
		result /= (float)_touches.Count;
		if (!allowFreeZoom && (!allowZoom || _touches.Count != 2))
		{
			result.x = Screen.width / 2;
			result.y = Screen.height / 2;
		}
		return result;
	}

	private void DoRubberBanding(Matrix4x4 boundsXform, Vector2 boundsSize)
	{
		Vector3 position = base.transform.position;
		Vector3 posLS = boundsXform.inverse.MultiplyPoint(position);
		Vector3 moveBackLS = GetMoveBackLS(boundsSize, posLS);
		float magnitude = moveBackLS.magnitude;
		float num = rubberBandAllowance * _camera.orthographicSize;
		float num2 = (((double)magnitude <= 0.02) ? 0f : (1f / (magnitude / num + 1f)));
		if (_touches.Count > 1)
		{
			num2 *= 0.2f;
		}
		moveBackLS *= 1f - num2;
		Vector3 vector = boundsXform.MultiplyVector(moveBackLS);
		if (vector.magnitude < 0.001f)
		{
			vector = Vector3.zero;
		}
		base.transform.position += vector;
	}

	private Vector3 GetMoveBackLS(Vector2 boundsSize, Vector3 posLS)
	{
		float x = boundsSize.x;
		Rect rect = new Rect(-0.5f * x, -0.5f * boundsSize.y, x, boundsSize.y);
		Vector3 vector = _camera.orthographicSize * new Vector3(visibleAspectRatio, 1f, 0f);
		Vector3 vector2 = posLS + vector;
		Vector3 vector3 = posLS - vector;
		float num = Mathf.Max(0f, rect.xMin - vector3.x);
		float num2 = Mathf.Max(0f, vector2.x - rect.xMax);
		float num3 = Mathf.Max(0f, vector2.y - rect.yMax);
		float num4 = Mathf.Max(0f, rect.yMin - vector3.y);
		return new Vector3(num - num2, num4 - num3, 0f);
	}

	private void StartZoomSnap()
	{
		float num = zoom;
		float defaultZoom = zoomModes[_mode].defaultZoom;
		if (defaultZoom != num)
		{
			_zoomSnapEaser.StartFromTo(num, defaultZoom, 0.2f);
			_zoomStartPos = base.transform.localPosition;
			_zoomEndPos = _zoomStartPos;
		}
	}

	public void StartZoomSnapToCentre()
	{
		float num = zoom;
		float defaultZoom = zoomModes[_mode].defaultZoom;
		if (defaultZoom != num)
		{
			_zoomSnapEaser.StartFromTo(num, defaultZoom, 0.2f);
			_zoomStartPos = base.transform.localPosition;
			_zoomEndPos = base.transform.parent.InverseTransformPoint(GetBoundedPosWithViewSize(bounds.transform.position, _refViewSize / defaultZoom));
		}
	}

	[TriggerableAction]
	public IEnumerator ZoomOut()
	{
		StartZoomOut(1f);
		return null;
	}

	public void StartPanWithZoomIn(Vector3 pos, float duration)
	{
		float num = zoom;
		float nearZoom = zoomModes[_mode].nearZoom;
		_zoomStartPos = base.transform.localPosition;
		_zoomEndPos = base.transform.parent.InverseTransformPoint(pos);
		if (_zoomStartPos != _zoomEndPos || num != nearZoom)
		{
			_zoomSnapEaser.StartFromTo(num, nearZoom, duration);
		}
		_doRubberBanding = false;
	}

	public void StartPanWithZoomOut(Vector3 pos, float duration)
	{
		float num = zoom;
		float farZoom = zoomModes[_mode].farZoom;
		_zoomStartPos = base.transform.localPosition;
		_zoomEndPos = base.transform.parent.InverseTransformPoint(pos);
		if (_zoomStartPos != _zoomEndPos || num != farZoom)
		{
			_zoomSnapEaser.StartFromTo(num, farZoom, duration);
		}
		_doRubberBanding = false;
	}

	public void StartPanWithZoomToDefault(Vector3 pos, float duration)
	{
		float num = zoom;
		float defaultZoom = zoomModes[_mode].defaultZoom;
		_zoomStartPos = base.transform.localPosition;
		_zoomEndPos = base.transform.parent.InverseTransformPoint(pos);
		if (_zoomStartPos != _zoomEndPos || num != defaultZoom)
		{
			_zoomSnapEaser.StartFromTo(num, defaultZoom, duration);
		}
		_doRubberBanding = false;
	}

	public bool IsPanning()
	{
		return _zoomSnapEaser.isRunning;
	}

	public void StartZoomOut(float duration)
	{
		float num = zoom;
		float farZoom = zoomModes[_mode].farZoom;
		if (num != farZoom)
		{
			_zoomSnapEaser.StartFromTo(num, farZoom, duration);
			_zoomStartPos = base.transform.localPosition;
			_zoomEndPos = base.transform.parent.InverseTransformPoint(GetBoundedPosWithViewSize(base.transform.position, _refViewSize / farZoom));
		}
	}

	public void StartZoomIn(float duration)
	{
		float num = zoom;
		float nearZoom = zoomModes[_mode].nearZoom;
		if (num != nearZoom)
		{
			_zoomSnapEaser.StartFromTo(num, nearZoom, duration);
			_zoomStartPos = base.transform.localPosition;
			_zoomEndPos = _zoomStartPos;
		}
	}

	public Vector3 GetBoundedPosWithViewSize(Vector3 viewCentre, float viewSize)
	{
		Vector3 vector = bounds.transform.InverseTransformPoint(viewCentre);
		vector.z = 0f;
		Vector3 moveBackLS = GetMoveBackLS(new Vector2(viewSize * visibleAspectRatio, viewSize), vector);
		return bounds.transform.TransformPoint(vector + moveBackLS);
	}

	public Camera GetCameraComponent()
	{
		return _camera;
	}
}
