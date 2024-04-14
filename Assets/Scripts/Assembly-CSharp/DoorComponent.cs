using System.Collections;
using Fabric;
using UnityEngine;

public class DoorComponent : MonoBehaviour
{
	private enum CurrentActionState
	{
		None = 0,
		Entry = 1,
		Exit = 2,
		Locking = 3,
		Complete = 4
	}

	public ActionSequence entryActions = new ActionSequence();

	public ActionSequence exitActions = new ActionSequence();

	public DoorComponent connectedDoor;

	public NavBoundaryComponent entryBoundary;

	public NavBoundaryComponent teleportBoundary;

	public Animation doorAnimation;

	public bool startLocked;

	public bool lockOnEntry;

	public bool lockOnExit;

	public bool autoExit = true;

	public bool woodenDoors;

	public bool muteDoors;

	public float _moveInputCooldown = 0.5f;

	private static string _openAnimationName = "DoorwayAOpen";

	private static string _closeAnimationName = "DoorwayAClose";

	private static string _stoneDoorOpenEvent = "World/Doors/Open";

	private static string _stoneDoorCloseEvent = "World/Doors/Shut";

	private static string _woodenDoorOpenEvent = "World/Doors/WoodenOpen";

	private static string _woodenDoorCloseEvent = "World/Doors/WoodenClose";

	private bool _doorOpened;

	private bool _doorClosed;

	private CharacterLocomotion _playerLocomotion;

	private PlayerInput _playerInput;

	private bool _pendingExit;

	private bool _muteOnStartup = true;

	private float _muteAudioOnStartupTimer = 3f;

	private CurrentActionState _currentActionState;

	public bool isEntryWalkable
	{
		get
		{
			if (entryBoundary.parentBrush.gameObject.activeSelf)
			{
				return entryBoundary.links.Count > 0;
			}
			return false;
		}
	}

	public bool isOpen
	{
		get
		{
			if (isEntryWalkable && (bool)connectedDoor)
			{
				return connectedDoor.isEntryWalkable;
			}
			return false;
		}
	}

	[TriggerableAction]
	public IEnumerator CloseAndLockDoor()
	{
		GameScene.navManager.NotifyReconfigurationBegan(base.gameObject);
		entryBoundary.parentBrush.gameObject.SetActive(value: false);
		if ((bool)connectedDoor)
		{
			connectedDoor.entryBoundary.parentBrush.gameObject.SetActive(value: false);
		}
		GameScene.navManager.NotifyReconfigurationEnded();
		return null;
	}

	[TriggerableAction]
	public IEnumerator UnlockDoor()
	{
		GameScene.navManager.NotifyReconfigurationBegan(base.gameObject);
		entryBoundary.parentBrush.gameObject.SetActive(value: true);
		entryBoundary.parentBrush.UpdateBoundaryZones();
		connectedDoor.entryBoundary.parentBrush.gameObject.SetActive(value: true);
		connectedDoor.entryBoundary.parentBrush.UpdateBoundaryZones();
		GameScene.navManager.NotifyReconfigurationEnded();
		return null;
	}

	[TriggerableAction]
	public IEnumerator ExitFromDoor()
	{
		autoExit = true;
		return null;
	}

	[TriggerableAction]
	public IEnumerator DisableAutoExit()
	{
		autoExit = false;
		return null;
	}

	[TriggerableAction]
	public IEnumerator EnableLockOnExit()
	{
		lockOnExit = true;
		return null;
	}

	[TriggerableAction]
	public IEnumerator MuteDoors()
	{
		muteDoors = true;
		return null;
	}

	[TriggerableAction]
	public IEnumerator UnmuteDoors()
	{
		muteDoors = false;
		return null;
	}

	private void Awake()
	{
		if (connectedDoor != null)
		{
			DebugUtils.DebugAssert(teleportBoundary);
			DebugUtils.DebugAssert(connectedDoor.teleportBoundary);
			ConnectTo(connectedDoor);
		}
	}

	private void Start()
	{
		if (doorAnimation == null)
		{
			doorAnimation = GetComponentInChildren<Animation>();
		}
		NavBrushComponent parentBrush = entryBoundary.parentBrush;
		if ((bool)parentBrush)
		{
			GameObject gameObject = new GameObject("NavIndicator");
			gameObject.transform.position = Vector3.Lerp(entryBoundary.transform.position, parentBrush.transform.position, 0.75f) + 0.03f * parentBrush.transform.up;
			gameObject.transform.rotation = parentBrush.transform.rotation;
			gameObject.transform.parent = base.transform;
			parentBrush.navIndicatorPosition = gameObject.transform;
		}
		if (startLocked || (connectedDoor != null && connectedDoor.startLocked))
		{
			CloseAndLockDoor();
			CloseDoor(instant: true);
		}
		else
		{
			OpenDoor(instant: true);
		}
	}

	public void ConnectTo(DoorComponent door)
	{
		if (connectedDoor != door)
		{
			if ((bool)connectedDoor)
			{
				connectedDoor.connectedDoor = null;
				connectedDoor.teleportBoundary.ConnectTo(null, permanent: true);
			}
			if ((bool)door && (bool)door.connectedDoor)
			{
				door.connectedDoor.connectedDoor = null;
				door.connectedDoor.teleportBoundary.ConnectTo(null, permanent: true);
			}
			connectedDoor = door;
		}
		if ((bool)connectedDoor)
		{
			connectedDoor.connectedDoor = this;
			teleportBoundary.ConnectTo(connectedDoor.teleportBoundary, permanent: true);
		}
		else
		{
			teleportBoundary.ConnectTo(null, permanent: true);
		}
	}

	public void NotifyDelayedExit()
	{
		if (_pendingExit)
		{
			_currentActionState = CurrentActionState.Exit;
			_pendingExit = false;
		}
	}

	private void Update()
	{
		if (_muteOnStartup)
		{
			_muteAudioOnStartupTimer -= Time.deltaTime;
			if (_muteAudioOnStartupTimer <= 0f)
			{
				_muteOnStartup = false;
			}
		}
		if (!_playerInput)
		{
			_playerLocomotion = GameScene.player.GetComponent<CharacterLocomotion>();
			_playerInput = GameScene.player.GetComponent<PlayerInput>();
		}
		entryBoundary.parentBrush.touchable = isOpen;
		if (isOpen)
		{
			if (!_doorOpened)
			{
				OpenDoor();
			}
		}
		else if (!_doorClosed)
		{
			CloseDoor();
		}
		bool num = _playerLocomotion.getTargetBrush() != _playerLocomotion.lastValidBrush;
		bool flag = _playerLocomotion.lastValidBrush == entryBoundary.parentBrush;
		if (!_pendingExit)
		{
			_pendingExit = _playerLocomotion.targetBrush == teleportBoundary.parentBrush && _playerLocomotion.entryBoundary == teleportBoundary;
		}
		if (num && flag && _currentActionState == CurrentActionState.None)
		{
			bool flag2 = _pendingExit && autoExit && _playerLocomotion.exitBoundary == entryBoundary;
			if (_playerLocomotion.exitBoundary == teleportBoundary)
			{
				_currentActionState = CurrentActionState.Entry;
			}
			else if (flag2)
			{
				_currentActionState = CurrentActionState.Exit;
				_pendingExit = false;
			}
		}
		if (_currentActionState == CurrentActionState.Entry)
		{
			PlayerInput component = _playerLocomotion.GetComponent<PlayerInput>();
			if ((bool)component)
			{
				component.CancelPendingRoutePlan();
				component.StartMoveInputCooldown(_moveInputCooldown);
			}
			StartCoroutine(entryActions.DoSequence());
			_currentActionState = CurrentActionState.Complete;
			_currentActionState = (lockOnEntry ? CurrentActionState.Locking : CurrentActionState.Complete);
		}
		else if (_currentActionState == CurrentActionState.Exit)
		{
			AnalyticsTrigger component2 = GetComponent<AnalyticsTrigger>();
			if (null != component2)
			{
				component2.SendAnalyticsEvent();
			}
			StartCoroutine(exitActions.DoSequence());
			_currentActionState = (lockOnExit ? CurrentActionState.Locking : CurrentActionState.Complete);
		}
		if (_currentActionState == CurrentActionState.Locking && !flag)
		{
			CloseAndLockDoor();
			_currentActionState = CurrentActionState.Complete;
		}
		if (!num)
		{
			_currentActionState = CurrentActionState.None;
		}
	}

	private void OpenDoor(bool instant = false)
	{
		_doorOpened = true;
		_doorClosed = false;
		if ((bool)doorAnimation)
		{
			doorAnimation[_openAnimationName].time = (instant ? doorAnimation[_openAnimationName].length : 0f);
			doorAnimation.Play(_openAnimationName);
		}
		if (!_muteOnStartup && !muteDoors && !instant && !TriggerAction.FastForward && (bool)EventManager.Instance)
		{
			EventManager.Instance.PostEvent(woodenDoors ? _woodenDoorOpenEvent : _stoneDoorOpenEvent, EventAction.PlaySound);
		}
	}

	private void CloseDoor(bool instant = false)
	{
		_doorOpened = false;
		_doorClosed = true;
		if ((bool)doorAnimation)
		{
			doorAnimation[_closeAnimationName].time = (instant ? doorAnimation[_closeAnimationName].length : 0f);
			doorAnimation.Play(_closeAnimationName);
		}
		if (!_muteOnStartup && !muteDoors && (bool)doorAnimation && !instant && !TriggerAction.FastForward && (bool)EventManager.Instance)
		{
			EventManager.Instance.PostEvent(woodenDoors ? _woodenDoorCloseEvent : _stoneDoorCloseEvent, EventAction.PlaySound);
		}
	}

	private void OnDrawGizmos()
	{
		if ((bool)connectedDoor)
		{
			Gizmos.color = Color.yellow;
			Gizmos.DrawLine(base.transform.position, connectedDoor.transform.position);
		}
		else
		{
			Gizmos.color = Color.red;
			Gizmos.DrawLine(base.transform.position, base.transform.position + new Vector3(0f, 10000f, 0f));
		}
	}
}
