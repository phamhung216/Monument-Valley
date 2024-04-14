using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using MVCommon;
using UnityCommon;
using UnityEngine;

public class TouchHandler : MonoBehaviour
{
	private class TouchableHit
	{
		public GameTouchable hitObject;

		public float hitDistance;

		public TouchableHit(GameTouchable inHitObject, float inHitDistance)
		{
			hitObject = inHitObject;
			hitDistance = inHitDistance;
		}
	}

	public CameraPanController gameCamera;

	private static int MOUSE_TOUCH_ID = 100;

	private static int MAX_RAYCAST_HITS = 100;

	private float touchRadius = 32f;

	private const float _maxTapTime = 0.25f;

	private float _maxTapRelSpeed = 4f;

	private Dictionary<int, GameTouch> _touches = new Dictionary<int, GameTouch>();

	private GameTouch _lastEndedTouch;

	private List<GameTouchable> _nonPhysicalTouchables = new List<GameTouchable>();

	private int _touchableLayer;

	private UITouchHandler _uiHandler;

	public Transform _interactionIndicator;

	public Animation _interactionIndicatorAnimation;

	public AnimationClip _interactionIndicatorAppearClip;

	public AnimationClip _interactionIndicatorDisappearClip;

	private ICursorListener _cursorListener;

	private int mask;

	private Camera _camera;

	private RaycastHit[] _hoverHits = new RaycastHit[MAX_RAYCAST_HITS];

	private RaycastHit[] _touchHits = new RaycastHit[MAX_RAYCAST_HITS];

	private bool _touchDisabled;

	private Vector3 _touchPosWorldSpace;

	private bool _touchActive;

	private IHoverable _lastHover;

	public bool touchActive => _touchActive;

	public Vector3 touchPosWorldSpace => _touchPosWorldSpace;

	public bool touchDisabled => _touchDisabled;

	public void EnableTouchLogging(bool value)
	{
		if (value)
		{
			D.Error("Attempt to enable touch logging when ENABLE_TOUCH_LOGGING is not defined");
		}
	}

	private int GetMouseTouchID()
	{
		return MOUSE_TOUCH_ID;
	}

	private void Start()
	{
		if (OrientationOverrideManager.IsLandscape())
		{
			_cursorListener = Service<MVCursor>.Instance;
		}
		_touchableLayer = LayerMask.NameToLayer("Touchable");
		mask = 1 << _touchableLayer;
		if (!gameCamera)
		{
			gameCamera = GetComponent<CameraPanController>();
		}
		_camera = GetComponent<Camera>();
		if (!_uiHandler)
		{
			GameObject gameObject = GameObject.Find("UICamera");
			if ((bool)gameObject)
			{
				_uiHandler = gameObject.GetComponent<UITouchHandler>();
			}
			float num = GameScene.ScreenToWorldLength(touchRadius);
			float num2 = 1f;
			touchRadius *= num2 / num;
			touchRadius *= 0.25f;
		}
		GameScene.instance.eventHandlers[SceneEvent.DisableInput].EventReceived += OnDisableInputEvent;
		GameScene.instance.eventHandlers[SceneEvent.DisableInputAndHidePauseButton].EventReceived += OnDisableInputEvent;
		GameScene.instance.eventHandlers[SceneEvent.EnableInput].EventReceived += OnEnableInputEvent;
	}

	public void OnDisableInputEvent()
	{
		DisableInput();
	}

	public void OnEnableInputEvent()
	{
		EnableInput();
	}

	[TriggerableAction]
	public IEnumerator DisableInput()
	{
		_touchDisabled = true;
		if (_cursorListener != null)
		{
			_cursorListener.DisableGameCursor();
		}
		CancelTouches();
		return null;
	}

	[TriggerableAction]
	public IEnumerator EnableInput()
	{
		_touchDisabled = false;
		if (_cursorListener != null)
		{
			_cursorListener.EnableGameCursor();
		}
		return null;
	}

	public void CancelTouches()
	{
		while (_touches.Count > 0)
		{
			GameTouch touch = null;
			using (Dictionary<int, GameTouch>.ValueCollection.Enumerator enumerator = _touches.Values.GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					touch = enumerator.Current;
				}
			}
			OnTouchCancelled(touch);
		}
	}

	private void Update()
	{
		if (!_touchDisabled)
		{
			AdvanceInput();
		}
		else if (OrientationOverrideManager.IsLandscape())
		{
			if (Input.GetMouseButtonUp(0))
			{
				_cursorListener.ShowDefaultCursor();
			}
			if (Input.GetMouseButtonDown(0))
			{
				_cursorListener.ShowClickedCursor();
			}
		}
		if (OrientationOverrideManager.IsLandscape() && _cursorListener != null)
		{
			_cursorListener.OnUpdate(Input.mousePosition);
		}
	}

	private void LateUpdate()
	{
		UpdateInteractionIndicator();
	}

	private void UpdateInteractionIndicator()
	{
		if (_interactionIndicator != null)
		{
			bool flag = false;
			if (_touches.Count == 1)
			{
				using Dictionary<int, GameTouch>.ValueCollection.Enumerator enumerator = _touches.Values.GetEnumerator();
				if (enumerator.MoveNext())
				{
					GameTouch current = enumerator.Current;
					if (current.owner != gameCamera.gameObject && current.owner != null && current.owner.gameObject.GetComponent<GameTouchable>().showTouchIndicator)
					{
						_touchPosWorldSpace = current.owner.gameObject.GetComponent<GameTouchable>().GetTouchIndicatorPosition(current);
						Vector3 pos = gameCamera.GetCameraComponent().WorldToScreenPoint(_touchPosWorldSpace);
						Ray ray = gameCamera.GetCameraComponent().ScreenPointToRay(pos);
						_interactionIndicator.position = ray.GetPoint(1f);
						flag = true;
					}
				}
			}
			if (_touchActive != flag)
			{
				_touchActive = flag;
				if (flag)
				{
					_interactionIndicator.gameObject.SetActive(value: true);
					_interactionIndicatorAnimation.clip = _interactionIndicatorAppearClip;
					_interactionIndicatorAnimation.Play();
				}
				else
				{
					_interactionIndicatorAnimation.clip = _interactionIndicatorDisappearClip;
					_interactionIndicatorAnimation.Play();
				}
			}
		}
		foreach (GameTouch value in _touches.Values)
		{
			_ = value;
		}
	}

	[Conditional("ENABLE_TOUCH_LOGGING")]
	private void ShowTouchPos(GameTouch touch, float radius, float timeout, Color color)
	{
	}

	[Conditional("ENABLE_TOUCH_LOGGING")]
	private void ShowTouchPos(GameTouch touch, float radius, float timeout)
	{
		_ = Color.white;
		if (touch.isTap)
		{
			_ = Color.yellow;
		}
	}

	private GameTouch GetTouch(int id)
	{
		GameTouch value = null;
		if (_touches.TryGetValue(id, out value))
		{
			return value;
		}
		return null;
	}

	private GameTouch AddTouch(int id, Vector2 pos)
	{
		if (GetTouch(id) != null)
		{
			D.Error("Duplicate touch id " + id);
			return null;
		}
		int tapCount = 1;
		if (_lastEndedTouch != null && Vector2.Distance(_lastEndedTouch.pos, pos) < touchRadius && Time.time - _lastEndedTouch.endTime < 0.25f)
		{
			tapCount = _lastEndedTouch.tapCount + 1;
		}
		if (!_touches.ContainsKey(id))
		{
			GameTouch gameTouch = new GameTouch(id);
			gameTouch.tapCount = tapCount;
			_touches.Add(id, gameTouch);
			return GetTouch(id);
		}
		throw new Exception("Duplicate touch ID");
	}

	public void SimulateEvent(GameTouch touch)
	{
		switch (touch.phase)
		{
		case TouchPhase.Began:
			if (!_touches.ContainsKey(touch.id))
			{
				AddTouch(touch.id, touch.pos);
				GameTouch touch3 = GetTouch(touch.id);
				touch3.Copy(touch);
				OnTouchBegan(touch3);
				break;
			}
			throw new Exception("Duplicate touch ID");
		case TouchPhase.Moved:
		{
			GameTouch touch4 = GetTouch(touch.id);
			if (touch4 != null)
			{
				GameObject owner2 = touch4.owner;
				touch4.Copy(touch);
				touch4.owner = owner2;
				OnTouchMoved(touch4);
			}
			break;
		}
		case TouchPhase.Ended:
		{
			GameTouch touch2 = GetTouch(touch.id);
			if (touch2 != null)
			{
				GameObject owner = touch2.owner;
				touch2.Copy(touch);
				touch2.owner = owner;
				OnTouchEnded(touch2);
			}
			break;
		}
		case TouchPhase.Stationary:
			break;
		}
	}

	private void AdvanceInput()
	{
		Touch[] touches = Input.touches;
		for (int i = 0; i < touches.Length; i++)
		{
			Touch touch = touches[i];
			if (touch.phase == TouchPhase.Began)
			{
				GameTouch gameTouch = AddTouch(touch.fingerId, touch.position);
				if (gameTouch != null)
				{
					gameTouch.phase = touch.phase;
					gameTouch.pos = touch.position;
					gameTouch.lastPos = gameTouch.pos;
					gameTouch.initialPos = gameTouch.pos;
					OnTouchBegan(gameTouch);
				}
			}
			else if (touch.phase == TouchPhase.Canceled || touch.phase == TouchPhase.Ended)
			{
				GameTouch touch2 = GetTouch(touch.fingerId);
				if (touch2 != null)
				{
					touch2.phase = touch.phase;
					touch2.lastPos = touch2.pos;
					touch2.pos = touch.position;
					OnTouchEnded(touch2);
				}
			}
			else if (touch.phase == TouchPhase.Moved)
			{
				GameTouch touch3 = GetTouch(touch.fingerId);
				if (touch3 != null)
				{
					touch3.phase = touch.phase;
					touch3.lastPos = touch3.pos;
					touch3.pos = touch.position;
					OnTouchMoved(touch3);
				}
			}
		}
		if (Input.touchCount == 0)
		{
			if (Input.GetMouseButtonDown(0))
			{
				if (OrientationOverrideManager.IsLandscape())
				{
					_cursorListener.ShowClickedCursor();
				}
				if (0f <= Input.mousePosition.x && Input.mousePosition.x <= (float)Screen.width && 0f <= Input.mousePosition.y && Input.mousePosition.y <= (float)Screen.height)
				{
					GameTouch touch4 = GetTouch(MOUSE_TOUCH_ID);
					if (touch4 != null)
					{
						touch4.phase = TouchPhase.Ended;
						touch4.lastPos = touch4.pos;
						touch4.pos = Input.mousePosition;
						OnTouchEnded(touch4);
					}
					touch4 = AddTouch(GetMouseTouchID(), Input.mousePosition);
					if (touch4 != null)
					{
						touch4.phase = TouchPhase.Began;
						touch4.pos = Input.mousePosition;
						touch4.lastPos = touch4.pos;
						touch4.initialPos = touch4.pos;
						OnTouchBegan(touch4);
					}
				}
			}
			else if (Input.GetMouseButtonUp(0))
			{
				if (OrientationOverrideManager.IsLandscape())
				{
					_cursorListener.ShowDefaultCursor();
				}
				GameTouch touch5 = GetTouch(MOUSE_TOUCH_ID);
				if (touch5 != null)
				{
					touch5.phase = TouchPhase.Ended;
					touch5.lastPos = touch5.pos;
					touch5.pos = Input.mousePosition;
					OnTouchEnded(touch5);
				}
			}
			else if (Input.GetMouseButton(0))
			{
				GameTouch touch6 = GetTouch(MOUSE_TOUCH_ID);
				if (touch6 != null)
				{
					if (touch6.pos != new Vector2(Input.mousePosition.x, Input.mousePosition.y))
					{
						touch6.phase = TouchPhase.Moved;
						touch6.lastPos = touch6.pos;
						touch6.pos = Input.mousePosition;
						OnTouchMoved(touch6);
					}
					else
					{
						touch6.phase = TouchPhase.Stationary;
					}
				}
			}
			if (OrientationOverrideManager.IsLandscape())
			{
				HoverCheck(Input.mousePosition);
			}
		}
		foreach (GameTouch value in _touches.Values)
		{
			if (value.phase == TouchPhase.Moved || value.phase == TouchPhase.Stationary)
			{
				float num = Time.time - value.startTime;
				_ = 0.25f;
				float magnitude = (value.pos - value.initialPos).magnitude;
				if (magnitude > touchRadius)
				{
					_ = value.isTap;
					OnTouchTapEnded(value);
				}
				float num2 = magnitude / num;
				float num3 = _maxTapRelSpeed * touchRadius;
				if (num > 0.125f && num2 > num3)
				{
					_ = value.isTap;
					OnTouchTapEnded(value);
				}
			}
		}
		if (_lastEndedTouch != null && _lastEndedTouch.tapCount == 1 && GetTouch(_lastEndedTouch.id) == null && Time.time - _lastEndedTouch.endTime > 0.25f)
		{
			if ((bool)_lastEndedTouch.owner && (bool)_lastEndedTouch.owner.GetComponent<GameTouchable>())
			{
				_lastEndedTouch.owner.GetComponent<GameTouchable>().OnTouchIsSingleTap(_lastEndedTouch);
			}
			_lastEndedTouch = null;
		}
	}

	private void OnTouchTapEnded(GameTouch touch)
	{
		if (!touch.isTap)
		{
			return;
		}
		touch.tapCount = 0;
		if (touch.owner != null && (bool)touch.owner.GetComponent<GameTouchable>() && touch.owner.GetComponent<GameTouchable>().ReleaseOnTapEnded(touch))
		{
			if (_touches.Count == 1)
			{
				touch.owner = HitTestGameTouchable(touch, touch.initialPos);
				_ = touch.owner != null;
			}
			if (touch.owner == null && gameCamera.isEnabled && gameCamera.AcceptTouch(touch))
			{
				touch.owner = gameCamera.gameObject;
			}
			if (touch.owner != null && (bool)touch.owner.GetComponent<GameTouchable>())
			{
				touch.owner.GetComponent<GameTouchable>().OnTouchBegan(touch);
			}
		}
	}

	private GameTouch GetFirstTouchBefore(GameTouch beforeTouch)
	{
		float num = float.MaxValue;
		GameTouch gameTouch = null;
		foreach (KeyValuePair<int, GameTouch> touch in _touches)
		{
			if (touch.Value.startTime < num && touch.Value != beforeTouch)
			{
				gameTouch = touch.Value;
				num = gameTouch.startTime;
			}
		}
		return gameTouch;
	}

	private void OnTouchBegan(GameTouch touch)
	{
		LogEvent(touch);
		if (_touches.Count == 2)
		{
			GameTouch firstTouchBefore = GetFirstTouchBefore(touch);
			if (firstTouchBefore.owner != gameCamera)
			{
				if (firstTouchBefore.owner != null)
				{
					firstTouchBefore.owner.GetComponent<GameTouchable>().OnTouchCancelled(firstTouchBefore);
				}
				firstTouchBefore.owner = gameCamera.gameObject;
				firstTouchBefore.owner.GetComponent<GameTouchable>().OnTouchBegan(firstTouchBefore);
			}
		}
		GameObject gameObject = null;
		if (UnityEngine.Debug.isDebugBuild && DebugPanel.HitTestTouch(touch))
		{
			gameObject = UnityEngine.Object.FindObjectOfType<DebugPanel>().gameObject;
			if (gameObject != null)
			{
				touch.owner = gameObject;
			}
		}
		if (gameObject == null && _uiHandler != null)
		{
			gameObject = _uiHandler.HitTest(touch);
			if (gameObject != null)
			{
				touch.owner = gameObject;
			}
		}
		if (gameObject == null)
		{
			if (_touches.Count == 1)
			{
				touch.owner = HitTestGameTouchable(touch, touch.initialPos);
				_ = touch.owner != null;
			}
			if (touch.owner == null && gameCamera.isEnabled && gameCamera.AcceptTouch(touch))
			{
				touch.owner = gameCamera.gameObject;
			}
		}
		if (touch.owner != null && (bool)touch.owner.GetComponent<GameTouchable>())
		{
			touch.owner.GetComponent<GameTouchable>().OnTouchBegan(touch);
		}
	}

	private void OnTouchMoved(GameTouch touch)
	{
		LogEvent(touch);
		if (touch.owner != null && (bool)touch.owner.GetComponent<GameTouchable>())
		{
			touch.owner.GetComponent<GameTouchable>().OnTouchMoved(touch);
		}
	}

	private void OnTouchEnded(GameTouch touch)
	{
		LogEvent(touch);
		touch.endTime = Time.time;
		if (touch.owner != null && (bool)touch.owner.GetComponent<GameTouchable>())
		{
			touch.owner.GetComponent<GameTouchable>().OnTouchEnded(touch);
		}
		_lastEndedTouch = touch;
		_touches.Remove(touch.id);
	}

	private void OnTouchCancelled(GameTouch touch)
	{
		LogEvent(touch);
		touch.endTime = Time.time;
		if (touch.owner != null && (bool)touch.owner.GetComponent<GameTouchable>())
		{
			touch.owner.GetComponent<GameTouchable>().OnTouchCancelled(touch);
		}
		_lastEndedTouch = touch;
		_touches.Remove(touch.id);
	}

	private void LogEvent(GameTouch touch)
	{
		if ((bool)GetComponent<TouchLogger>())
		{
			GetComponent<TouchLogger>().LogEvent(touch);
		}
	}

	private GameObject DoHitTest(Camera camera, Vector2 pos, LayerMask mask)
	{
		if (camera == null)
		{
			return null;
		}
		Ray ray = camera.ScreenPointToRay(new Vector3(pos.x, pos.y, -100f));
		GameObject.Find("Image Button");
		if (Physics.Raycast(ray, out var hitInfo, float.PositiveInfinity, 1 << (int)mask))
		{
			return hitInfo.collider.gameObject;
		}
		return null;
	}

	private bool CheckForHoverOverUI(Vector2 touchPos)
	{
		GameObject gameObject = _uiHandler.HitTestHover(touchPos);
		if (gameObject != null)
		{
			if (gameObject.GetComponent<UITouchable>() is IHoverable hoverable)
			{
				if (_lastHover != null && _lastHover != hoverable)
				{
					_lastHover.OnHoverEnd();
					_lastHover = null;
				}
				hoverable.OnHover();
				_lastHover = hoverable;
			}
			return true;
		}
		if (_lastHover != null)
		{
			_lastHover.OnHoverEnd();
			_lastHover = null;
		}
		return false;
	}

	private void HoverCheck(Vector2 testTouchPos)
	{
		List<MonoBehaviour> listOfBehavioursForInterface = Service<InterfaceCollectorService>.Instance.GetListOfBehavioursForInterface<IHoverable>();
		if (CheckForHoverOverUI(testTouchPos))
		{
			gameCamera.SetHasMouseWheelFocus(value: false);
			return;
		}
		gameCamera.SetHasMouseWheelFocus(value: true);
		if (gameCamera.allowFreeZoom)
		{
			return;
		}
		new List<TouchableHit>();
		MonoBehaviour monoBehaviour = null;
		Ray ray = _camera.ScreenPointToRay(new Vector3(testTouchPos.x, testTouchPos.y, -100f));
		float num = float.MaxValue;
		int num2 = Physics.RaycastNonAlloc(ray, _hoverHits, float.PositiveInfinity, mask);
		_ = MAX_RAYCAST_HITS;
		for (int i = 0; i < num2; i++)
		{
			RaycastHit raycastHit = _hoverHits[i];
			if (!(raycastHit.distance < num))
			{
				continue;
			}
			for (int j = 0; j < listOfBehavioursForInterface.Count; j++)
			{
				if (listOfBehavioursForInterface[j].gameObject == raycastHit.collider.gameObject)
				{
					GameTouchable gameTouchable = listOfBehavioursForInterface[j] as GameTouchable;
					if (gameTouchable == null)
					{
						D.Error("No GameTouchable found on hoverable");
						return;
					}
					TotemPoleInput obj = listOfBehavioursForInterface[j] as TotemPoleInput;
					bool flag = false;
					if (obj != null)
					{
						flag = true;
					}
					else
					{
						Dragger dragger = listOfBehavioursForInterface[j] as Dragger;
						if (dragger != null)
						{
							flag = dragger.targetDraggable.AllowDrag();
						}
						else
						{
							TouchableProxy touchableProxy = listOfBehavioursForInterface[j] as TouchableProxy;
							if (touchableProxy != null)
							{
								Dragger dragger2 = touchableProxy.touchable as Dragger;
								flag = !(dragger2 != null) || !(dragger2.targetDraggable != null) || dragger2.targetDraggable.AllowDrag();
							}
							else if (listOfBehavioursForInterface[j] as TotemCubeProxy != null)
							{
								flag = true;
							}
						}
					}
					if (gameTouchable.isEnabled && flag)
					{
						num = raycastHit.distance;
						monoBehaviour = listOfBehavioursForInterface[j];
					}
				}
				else if (raycastHit.collider.GetComponent<GameTouchable>() == null)
				{
					num = raycastHit.distance;
					monoBehaviour = null;
				}
			}
		}
		if (monoBehaviour != null)
		{
			(monoBehaviour as IHoverable).OnHover();
		}
	}

	private GameObject HitTestGameTouchable(GameTouch touch, Vector2 testTouchPos)
	{
		if (gameCamera.allowFreeZoom)
		{
			return null;
		}
		List<TouchableHit> list = new List<TouchableHit>();
		Ray ray = _camera.ScreenPointToRay(new Vector3(testTouchPos.x, testTouchPos.y, -100f));
		foreach (GameTouchable nonPhysicalTouchable in _nonPhysicalTouchables)
		{
			if (nonPhysicalTouchable.AcceptTouch(touch))
			{
				list.Add(new TouchableHit(nonPhysicalTouchable, nonPhysicalTouchable.GetHitDistance(touch, ray)));
			}
		}
		int count = list.Count;
		float num = float.MaxValue;
		int num2 = Physics.RaycastNonAlloc(ray, _touchHits, float.PositiveInfinity, mask);
		_ = MAX_RAYCAST_HITS;
		for (int i = 0; i < num2; i++)
		{
			RaycastHit raycastHit = _touchHits[i];
			if (!(raycastHit.distance < num))
			{
				continue;
			}
			GameTouchable component = raycastHit.collider.GetComponent<GameTouchable>();
			if (component != null)
			{
				if (component.AcceptTouch(touch))
				{
					RenderDebug.DrawLine(ray.origin, ray.GetPoint(raycastHit.distance), Color.green, 1f);
					if (component.CanOverRideNonPhysicals() || count == 0)
					{
						list.Add(new TouchableHit(component, raycastHit.distance));
					}
				}
				else
				{
					RenderDebug.DrawLine(ray.origin, ray.GetPoint(raycastHit.distance), Color.gray, 1f);
				}
				continue;
			}
			RenderDebug.DrawLine(ray.origin, ray.GetPoint(raycastHit.distance), Color.red, 1f);
			num = raycastHit.distance;
			for (int j = 0; j < count; j++)
			{
			}
			int num3 = count;
			while (num3 < list.Count)
			{
				if (list[num3].hitDistance > num)
				{
					list.RemoveAt(num3);
				}
				else
				{
					num3++;
				}
			}
		}
		TouchableHit touchableHit = null;
		for (int k = 0; k < list.Count; k++)
		{
			if (touchableHit == null || list[k].hitDistance < touchableHit.hitDistance)
			{
				touchableHit = list[k];
			}
		}
		return touchableHit?.hitObject.gameObject;
	}

	public void RegisterNonPhysicalTouchable(GameTouchable touchable)
	{
		_nonPhysicalTouchables.Add(touchable);
	}

	[Conditional("ENABLE_TOUCH_LOGGING")]
	public static void Log(string text)
	{
	}
}
