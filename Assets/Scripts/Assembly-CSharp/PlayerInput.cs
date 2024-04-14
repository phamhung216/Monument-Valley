using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : GameTouchable
{
	public enum PathState
	{
		Waiting = 0,
		EnRoute = 1
	}

	public enum ControlSource
	{
		Game = 0,
		User = 1
	}

	private NavManager navManager;

	public NavRequest navRequest;

	public int currentTarget;

	public Camera currentCamera;

	private NavIndicator navIndicator;

	private PathState _playerPathState;

	private bool _newLocationSelected;

	private NavBrushComponent _touchedBrush;

	private ControlSource _controlSource = ControlSource.User;

	private NavBrushComponent _lastInputDestination;

	private float _startPlanTime;

	private bool _requestNav;

	private float _inputCooldownStartTime;

	private float _cooldownDuration = 0.5f;

	public NavBrushComponent lastInputDestination => _lastInputDestination;

	public PathState playerPathState => _playerPathState;

	public ControlSource controlSource => _controlSource;

	public override bool showTouchIndicator => false;

	public float timeSinceMovePlanStarted => Time.time - _startPlanTime;

	private void Awake()
	{
		claimOnTouchBegan = true;
		claimOnTouchNotTap = false;
		releaseOnTouchNotTap = true;
		_requestNav = false;
	}

	private void Start()
	{
		GameScene.instance.SetPlayer(base.gameObject);
		currentCamera = Camera.main;
		navManager = GameScene.navManager;
		navIndicator = GameScene.navIndicator;
		navRequest = new NavRequest();
		Camera.main.GetComponent<TouchHandler>().RegisterNonPhysicalTouchable(this);
		if (Camera.main.GetComponent<CameraPanController>().doubleTapToZoom)
		{
			ignoreMultiTaps = true;
		}
		else
		{
			ignoreMultiTaps = false;
		}
	}

	private void Update()
	{
		_playerPathState = PathState.Waiting;
		if (navRequest.status == NavRequest.RequestStatus.Complete && _newLocationSelected && navRequest.route.Count > 0)
		{
			_playerPathState = PathState.EnRoute;
			_newLocationSelected = false;
			NavBrushComponent targetBrush = navRequest.route[navRequest.route.Count - 1];
			if (IsBrushTeleporter(targetBrush))
			{
				InsertTeleporterExitPoint(targetBrush, navRequest.route);
			}
			GetComponent<CharacterLocomotion>().AddMoveRequest(navRequest);
		}
		if (_requestNav && navManager.ReconfigurationCount() == 0 && (navRequest.route == null || navRequest.route.Count == 0) && navRequest.status == NavRequest.RequestStatus.Complete)
		{
			if ((double)timeSinceMovePlanStarted < 1.0)
			{
				navRequest.status = NavRequest.RequestStatus.Pending;
				navManager.AddRequest(navRequest);
			}
			_requestNav = false;
		}
	}

	public void PlayDestinationEffect(NavBrushComponent brush)
	{
		if (navIndicator != null)
		{
			navIndicator.PlayForBrush(brush);
		}
	}

	public override float GetHitDistance(GameTouch touch, Ray worldRay)
	{
		return Vector3.Dot(rhs: (_touchedBrush = navManager.FindNavBrushBelowPanPoint(GameScene.ScreenToPanPoint(touch.pos, currentCamera), touchableOnly: true)).transform.position, lhs: worldRay.direction) - Vector3.Dot(worldRay.direction, worldRay.origin) - 0.4f;
	}

	public bool IsInCooldownPeriod()
	{
		return Time.time - _inputCooldownStartTime < _cooldownDuration;
	}

	public bool CanAcceptTouches()
	{
		if (IsInCooldownPeriod())
		{
			return false;
		}
		if (GetComponent<CharacterLocomotion>().lastValidBrush.hasPermanentConnections || GetComponent<CharacterLocomotion>().targetBrush.hasPermanentConnections)
		{
			return false;
		}
		if (!IsInCooldownPeriod() && _controlSource == ControlSource.User && base.enabled)
		{
			return isEnabled;
		}
		return false;
	}

	public override bool AcceptTouch(GameTouch touch)
	{
		if (!CanAcceptTouches())
		{
			return false;
		}
		if (!base.AcceptTouch(touch))
		{
			return false;
		}
		if (touch.isTap && (!ignoreMultiTaps || touch.tapCount <= 1))
		{
			if (navManager != null)
			{
				return null != (_touchedBrush = navManager.FindNavBrushBelowPanPoint(GameScene.ScreenToPanPoint(touch.pos, currentCamera), touchableOnly: true));
			}
			D.Error("Player missing NavManager so cannot plan path");
		}
		return false;
	}

	private bool IsBrushTeleporter(NavBrushComponent targetBrush)
	{
		return targetBrush.hasPermanentConnections;
	}

	private void InsertTeleporterExitPoint(NavBrushComponent targetBrush, List<NavBrushComponent> route)
	{
		NavBoundaryComponent[] boundaries = targetBrush.boundaries;
		foreach (NavBoundaryComponent navBoundaryComponent in boundaries)
		{
			if (navBoundaryComponent != null && navBoundaryComponent.permanentConnection != null)
			{
				NavBrushComponent component = navBoundaryComponent.permanentConnection.transform.parent.GetComponent<NavBrushComponent>();
				route.Add(component);
				break;
			}
		}
	}

	public override void OnTouchBegan(GameTouch touch)
	{
		base.OnTouchBegan(touch);
	}

	public override void OnTouchEnded(GameTouch touch)
	{
		if (CanAcceptTouches() && (!ignoreMultiTaps || touch.tapCount <= 1) && _touchedBrush != null)
		{
			PlayDestinationEffect(_touchedBrush);
			if (!ignoreMultiTaps)
			{
				MoveTo(_touchedBrush);
			}
		}
	}

	public override void OnTouchCancelled(GameTouch touch)
	{
	}

	public override void OnTouchIsSingleTap(GameTouch touch)
	{
		if (CanAcceptTouches() && ignoreMultiTaps && _touchedBrush != null)
		{
			MoveTo(_touchedBrush);
		}
	}

	public void MoveTo(NavBrushComponent destination)
	{
		_startPlanTime = Time.time;
		if (!(destination != null))
		{
			return;
		}
		_lastInputDestination = destination;
		if (navRequest == null || !(navRequest.startBrush != null) || navRequest.status == NavRequest.RequestStatus.Complete)
		{
			navManager.RemoveRequest(navRequest);
			navRequest.Init(GetComponent<CharacterLocomotion>().getTargetBrush(), destination, GetComponent<CharacterLocomotion>().navAccess);
			navManager.AddRequest(navRequest);
			if (navManager.ReconfigurationCount() != 0)
			{
				_requestNav = true;
			}
			GetComponent<CharacterLocomotion>().StopAtTargetBrush();
			_newLocationSelected = true;
		}
	}

	public void CancelPendingRoutePlan()
	{
		GameScene.navManager.RemoveRequest(navRequest);
		_newLocationSelected = false;
		navIndicator.Clear();
		if (navRequest != null)
		{
			navRequest.startBrush = null;
			navRequest.status = NavRequest.RequestStatus.Complete;
		}
	}

	public void CancelMove()
	{
		GetComponent<CharacterLocomotion>().StopMove();
		CancelPendingRoutePlan();
	}

	public void StartMoveInputCooldown(float coolDownDuration)
	{
		_inputCooldownStartTime = Time.time;
		_cooldownDuration = coolDownDuration;
	}

	public void SetControlSource(ControlSource source)
	{
		_controlSource = source;
	}
}
