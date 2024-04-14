using System;
using System.Collections;
using UnityEngine;

public class BaseLocomotion : MonoBehaviour
{
	protected enum UpDownState
	{
		Down = 0,
		Up = 1
	}

	public enum LocomotionState
	{
		LocoEmpty = 0,
		LocoIdle = 1,
		LocoIdleLadder = 2,
		LocoWalk = 3,
		LocoStartLadder = 4,
		LocoLadder = 5,
		LocoEndLadder = 6,
		LocoStartLadderDown = 7,
		LocoLadderDown = 8,
		LocoEndLadderDown = 9,
		LocoStartStairsUp = 10,
		LocoStairsUp = 11,
		LocoEndStairsUp = 12,
		LocoStartStairsDown = 13,
		LocoStairsDown = 14,
		LocoEndStairsDown = 15,
		LocoWalkUpCorner = 16,
		LocoWalkDownCorner = 17,
		LocoStairsIdle = 18,
		LocoBlocked = 19,
		LocoCustomAnim = 20
	}

	public struct CustomAnimRequest
	{
		public bool isPending;

		public string animName;

		public bool ignoreNav;

		public bool blend;

		public bool detachHat;

		public Transform dropShadowPosition;

		public float startNormalizeTime;
	}

	protected float interpT;

	private NavBrushComponent _lastValidBrush;

	private NavBoundaryComponent _exitBoundary;

	private NavBoundaryComponent _entryBoundary;

	private NavBrushComponent _targetBrush;

	public float characterVel = 5f;

	protected Vector3 currentDir;

	protected float _walkAnimAuthoredSpeed = 3f;

	public float characterRotVel = 20f;

	private DropShadow _shadow;

	protected UpDownState prevUpDownDir;

	private LocomotionState _locoState;

	protected LocomotionState prevState;

	public Animation animSystem;

	public float ladderSpeed = 1f;

	public float stairSpeed = 1f;

	public bool lookAtObject = true;

	public Transform lookAtObjectTarget;

	public bool alwaysUseDefaultConnectionTolerance;

	protected string animSuffix = "";

	protected CustomAnimRequest _customAnimRequest;

	protected bool _walkRoundCorners;

	[BitMaskField(typeof(NavAccessFlags))]
	public NavAccessFlags navAccess = NavAccessFlags.Player | NavAccessFlags.NotBlocked;

	protected float _magicPosCameraOffset = 0.1f;

	private int _lastTravelBrushConnectionCheckFrameIdx;

	private bool _areTravelBrushesConnected;

	public virtual NavBrushComponent lastValidBrush => _lastValidBrush;

	public virtual NavBoundaryComponent exitBoundary => _exitBoundary;

	public virtual NavBoundaryComponent entryBoundary => _entryBoundary;

	public virtual NavBrushComponent targetBrush => _targetBrush;

	public NavBrushComponent currentBrush
	{
		get
		{
			if (!(interpT < 0.5f))
			{
				return _targetBrush;
			}
			return _lastValidBrush;
		}
	}

	public Vector3 characterUp => base.transform.up;

	public LocomotionState locoState
	{
		get
		{
			return _locoState;
		}
		set
		{
			_locoState = value;
		}
	}

	public DropShadow shadow
	{
		get
		{
			return _shadow;
		}
		set
		{
			_shadow = value;
		}
	}

	public bool IsOnStairs
	{
		get
		{
			if (locoState != LocomotionState.LocoStartStairsUp && locoState != LocomotionState.LocoStartStairsDown && locoState != LocomotionState.LocoStairsDown && locoState != LocomotionState.LocoStairsIdle && locoState != LocomotionState.LocoStairsUp && locoState != LocomotionState.LocoEndStairsDown)
			{
				return locoState == LocomotionState.LocoEndStairsUp;
			}
			return true;
		}
	}

	public bool IsOnLadder
	{
		get
		{
			if (locoState != LocomotionState.LocoIdleLadder && locoState != LocomotionState.LocoStartLadder && locoState != LocomotionState.LocoLadder && locoState != LocomotionState.LocoEndLadder && locoState != LocomotionState.LocoStartLadderDown && locoState != LocomotionState.LocoLadderDown)
			{
				return locoState == LocomotionState.LocoEndLadderDown;
			}
			return true;
		}
	}

	public bool IsWalkingAroundCorner
	{
		get
		{
			if (locoState != LocomotionState.LocoWalkDownCorner)
			{
				return locoState == LocomotionState.LocoWalkUpCorner;
			}
			return true;
		}
	}

	public void RequestCustomAnim(string name, bool ignoreNav = false, bool blend = false, bool detachHat = true, Transform dropShadowPosition = null, float startNormalizeTime = 0f)
	{
		_customAnimRequest.isPending = true;
		_customAnimRequest.animName = name;
		_customAnimRequest.ignoreNav = ignoreNav;
		_customAnimRequest.blend = blend;
		_customAnimRequest.dropShadowPosition = dropShadowPosition;
		_customAnimRequest.detachHat = detachHat;
		_customAnimRequest.startNormalizeTime = startNormalizeTime;
	}

	public virtual void InitNav()
	{
	}

	protected void SetLocomotionNodes(NavBrushComponent brush, NavBoundaryComponent brushEntryBoundary)
	{
		DebugUtils.DebugAssert(brushEntryBoundary);
		DebugUtils.DebugAssert(!brushEntryBoundary || brushEntryBoundary.parentBrush == brush);
		_exitBoundary = null;
		_entryBoundary = brushEntryBoundary;
		_lastValidBrush = brush;
		_targetBrush = brush;
		UpdateAreTravelBrushesConnected();
	}

	protected virtual void SetLocomotionNodes(NavBrushComponent brush, Vector3 direction)
	{
		NavBoundaryComponent navBoundaryComponent = null;
		float num = -2f;
		NavBoundaryComponent[] array = brush.boundaries;
		if (array == null)
		{
			array = brush.gameObject.GetComponentsInChildren<NavBoundaryComponent>();
		}
		NavBoundaryComponent[] array2 = array;
		foreach (NavBoundaryComponent navBoundaryComponent2 in array2)
		{
			Vector3 lhs = navBoundaryComponent2.transform.position - brush.transform.position;
			lhs.Normalize();
			float num2 = Vector3.Dot(lhs, -direction);
			if (num2 > num)
			{
				num = num2;
				navBoundaryComponent = navBoundaryComponent2;
			}
		}
		if ((bool)navBoundaryComponent)
		{
			SetLocomotionNodes(brush, navBoundaryComponent);
			return;
		}
		_exitBoundary = null;
		_entryBoundary = null;
		_lastValidBrush = brush;
		_targetBrush = brush;
	}

	protected virtual void SetLocomotionNodes(NavBrushComponent from, NavBrushComponent to)
	{
		if (from == to)
		{
			if (from == _lastValidBrush)
			{
				_targetBrush = _lastValidBrush;
				_exitBoundary = null;
				return;
			}
			throw new Exception("Not allowed to set from and to brushes to same object.");
		}
		NavBoundaryComponent connectionBoundary = from.GetConnectionBoundary(to);
		NavBoundaryComponent connectionBoundary2 = to.GetConnectionBoundary(from);
		if (!connectionBoundary || !connectionBoundary2)
		{
			throw new Exception("Brushes not connected.");
		}
		_exitBoundary = connectionBoundary;
		_lastValidBrush = from;
		_entryBoundary = connectionBoundary2;
		_targetBrush = to;
		UpdateAreTravelBrushesConnected();
	}

	protected virtual void IncrementNodes()
	{
	}

	protected void UpdateShadow()
	{
		shadow.PosParentUpdate();
	}

	protected void InitBase()
	{
		navAccess |= NavAccessFlags.NotBlocked;
	}

	public virtual Vector3 getShadowUp()
	{
		return characterUp;
	}

	public virtual float GetInterpT()
	{
		return interpT;
	}

	public virtual NavBrushComponent getTargetBrush()
	{
		return null;
	}

	public virtual void Stop()
	{
	}

	public void Awake()
	{
	}

	private void Start()
	{
	}

	private void Update()
	{
	}

	public virtual void FaceDirection(Vector3 direction)
	{
		SetLocomotionNodes(lastValidBrush, direction);
	}

	protected Vector3 CalculateMagicPlayerProjection(Vector3 currentPos)
	{
		return currentPos;
	}

	protected Vector3 CalculateMagicPlayerProjection(Vector3 currentPos, Vector3 fromPos, Vector3 targetPos, Vector3 worldUp)
	{
		Vector3 vector = currentPos;
		Vector3 forward = Camera.main.transform.forward;
		if (Vector3.Dot(forward, worldUp) <= -0.02f)
		{
			Ray ray = new Ray(vector, -forward);
			Plane plane = new Plane(worldUp, fromPos);
			Plane plane2 = new Plane(worldUp, targetPos);
			float enter = 0f;
			float enter2 = 0f;
			plane.Raycast(ray, out enter);
			plane2.Raycast(ray, out enter2);
			float num = enter2;
			if (enter > enter2)
			{
				num = enter;
			}
			vector = currentPos - forward * num;
		}
		return CalculateMagicPlayerProjection(vector);
	}

	protected Vector3 GetFaceBrushDirection(NavBrushComponent brushToFace, NavBrushComponent ownBrush)
	{
		NavBrushLink linkToBrush = ownBrush.GetLinkToBrush(brushToFace);
		if (linkToBrush == null)
		{
			if (brushToFace == ownBrush)
			{
				return base.transform.forward;
			}
			return ProjectDirectionIntoBrushPlane(brushToFace.transform.position - ownBrush.transform.position, ownBrush);
		}
		Vector3 dir = linkToBrush.GetOtherBoundary(brushToFace).transform.position - ownBrush.transform.position;
		return ProjectDirectionIntoBrushPlane(dir, ownBrush);
	}

	protected Quaternion GetFaceBrushRotation(NavBrushComponent brushToFace, NavBrushComponent ownBrush)
	{
		return Quaternion.LookRotation(GetFaceBrushDirection(brushToFace, ownBrush), ownBrush.transform.up);
	}

	protected Quaternion GetNavDirectionOnBrushTowardsPoint(Vector3 targetPoint, NavBrushComponent brush)
	{
		Vector3 forward = ProjectNavPointIntoBrushPlane(targetPoint, brush) - brush.transform.position;
		forward.Normalize();
		return Quaternion.LookRotation(forward, brush.transform.up);
	}

	protected Vector3 CalculateFromDepthReference(Vector3 currentPos, Vector3 refPos)
	{
		Vector3 vector = new Vector3(currentPos.x, currentPos.y, currentPos.z);
		Vector3 rhs = new Vector3(refPos.x, refPos.y - 0.5f, refPos.z);
		Vector3 forward = Camera.main.transform.forward;
		float num = Vector3.Dot(forward, currentPos);
		float num2 = Vector3.Dot(forward, rhs);
		return vector + forward * (num2 - num);
	}

	public virtual Vector3 getShadowRootPosition()
	{
		if (locoState == LocomotionState.LocoCustomAnim && (bool)_customAnimRequest.dropShadowPosition)
		{
			return _customAnimRequest.dropShadowPosition.position;
		}
		return base.transform.position;
	}

	public virtual float getShadowIntensity()
	{
		if (locoState == LocomotionState.LocoCustomAnim && (bool)_customAnimRequest.dropShadowPosition)
		{
			float num = 3f;
			float num2 = Vector3.Dot(base.transform.up, base.transform.position - _customAnimRequest.dropShadowPosition.position);
			return Mathf.Clamp(1f - num2 / num, 0f, 1f);
		}
		return 1f;
	}

	protected virtual void UpdateStates()
	{
		switch (locoState)
		{
		case LocomotionState.LocoBlocked:
			UpdateBlockedState();
			break;
		case LocomotionState.LocoIdle:
			UpdateIdleState();
			break;
		case LocomotionState.LocoIdleLadder:
			UpdateIdleLadderState();
			break;
		case LocomotionState.LocoStairsIdle:
			UpdateStairsIdle();
			break;
		case LocomotionState.LocoWalk:
			UpdateWalkState();
			break;
		case LocomotionState.LocoLadder:
			UpdateClimbState();
			break;
		case LocomotionState.LocoStartLadder:
			UpdateStartClimbState();
			break;
		case LocomotionState.LocoEndLadder:
			UpdateEndClimbState();
			break;
		case LocomotionState.LocoLadderDown:
			UpdateClimbStateDown();
			break;
		case LocomotionState.LocoStartLadderDown:
			UpdateStartClimbStateDown();
			break;
		case LocomotionState.LocoEndLadderDown:
			UpdateEndClimbStateDown();
			break;
		case LocomotionState.LocoStartStairsUp:
			UpdateStartStairsUpState();
			break;
		case LocomotionState.LocoStairsUp:
			UpdateStairsUpState();
			break;
		case LocomotionState.LocoEndStairsUp:
			UpdateEndStairsUpState();
			break;
		case LocomotionState.LocoStartStairsDown:
			UpdateStartStairsDownState();
			break;
		case LocomotionState.LocoStairsDown:
			UpdateStairsDownState();
			break;
		case LocomotionState.LocoEndStairsDown:
			UpdateEndStairsDownState();
			break;
		case LocomotionState.LocoWalkUpCorner:
			UpdateWalkUpCornerState();
			break;
		case LocomotionState.LocoWalkDownCorner:
			UpdateWalkDownCornerState();
			break;
		case LocomotionState.LocoCustomAnim:
			UpdateCustomAnimState();
			break;
		}
		prevState = locoState;
	}

	protected LocomotionState getCorrectIdleState(NavBrushComponent brush)
	{
		if (brush != null && brush.type == NavBrushComponent.Type.Ladder)
		{
			return LocomotionState.LocoIdleLadder;
		}
		if (brush != null && brush.type == NavBrushComponent.Type.Stairs)
		{
			return LocomotionState.LocoStairsIdle;
		}
		return LocomotionState.LocoIdle;
	}

	protected virtual bool IsBlocked()
	{
		return false;
	}

	protected virtual void StartBlockedState()
	{
	}

	protected virtual void EndBlockedState()
	{
	}

	protected LocomotionState SelectLocoState(NavBrushComponent fromBrush, NavBrushComponent toBrush)
	{
		if (locoState == LocomotionState.LocoBlocked)
		{
			if (IsBlocked())
			{
				return LocomotionState.LocoBlocked;
			}
			EndBlockedState();
		}
		else if (IsBlocked())
		{
			return LocomotionState.LocoBlocked;
		}
		if (fromBrush == toBrush)
		{
			return getCorrectIdleState(fromBrush);
		}
		if (fromBrush.type != NavBrushComponent.Type.Stairs && toBrush.type == NavBrushComponent.Type.Stairs)
		{
			NavBoundaryComponent connectionBoundary = toBrush.GetConnectionBoundary(fromBrush);
			if ((bool)connectionBoundary)
			{
				if (Vector3.Dot(toBrush.transform.position - connectionBoundary.transform.position, characterUp) > 0f)
				{
					return LocomotionState.LocoStartStairsUp;
				}
				return LocomotionState.LocoStartStairsDown;
			}
			Stop();
			return LocomotionState.LocoIdle;
		}
		if (fromBrush.type == NavBrushComponent.Type.Stairs && toBrush.type == NavBrushComponent.Type.Stairs)
		{
			NavBoundaryComponent connectionBoundary2 = toBrush.GetConnectionBoundary(fromBrush);
			if (Vector3.Dot(toBrush.transform.position - connectionBoundary2.transform.position, characterUp) > 0f)
			{
				return LocomotionState.LocoStairsUp;
			}
			return LocomotionState.LocoStairsDown;
		}
		if (fromBrush.type == NavBrushComponent.Type.Stairs && toBrush.type != NavBrushComponent.Type.Stairs)
		{
			NavBoundaryComponent connectionBoundary3 = fromBrush.GetConnectionBoundary(toBrush);
			if ((bool)connectionBoundary3)
			{
				if (Vector3.Dot(fromBrush.transform.position - connectionBoundary3.transform.position, characterUp) < 0f)
				{
					return LocomotionState.LocoEndStairsUp;
				}
				return LocomotionState.LocoEndStairsDown;
			}
			return LocomotionState.LocoStairsIdle;
		}
		if (fromBrush.type != NavBrushComponent.Type.Ladder && toBrush.type == NavBrushComponent.Type.Ladder)
		{
			NavBoundaryComponent connectionBoundary4 = toBrush.GetConnectionBoundary(fromBrush);
			if (!connectionBoundary4)
			{
				return LocomotionState.LocoIdleLadder;
			}
			if (Vector3.Dot(toBrush.transform.position - connectionBoundary4.transform.position, characterUp) > 0f)
			{
				return LocomotionState.LocoStartLadder;
			}
			return LocomotionState.LocoStartLadderDown;
		}
		if (fromBrush.type == NavBrushComponent.Type.Ladder && toBrush.type == NavBrushComponent.Type.Ladder)
		{
			NavBoundaryComponent connectionBoundary5 = toBrush.GetConnectionBoundary(fromBrush);
			if (!connectionBoundary5)
			{
				return LocomotionState.LocoIdleLadder;
			}
			if (Vector3.Dot(toBrush.transform.position - connectionBoundary5.transform.position, characterUp) > 0f)
			{
				return LocomotionState.LocoLadder;
			}
			return LocomotionState.LocoLadderDown;
		}
		if (fromBrush.type == NavBrushComponent.Type.Ladder && toBrush.type != NavBrushComponent.Type.Ladder)
		{
			NavBoundaryComponent connectionBoundary6 = fromBrush.GetConnectionBoundary(toBrush);
			if (!connectionBoundary6)
			{
				return LocomotionState.LocoIdleLadder;
			}
			if (Vector3.Dot(fromBrush.transform.position - connectionBoundary6.transform.position, characterUp) < 0f)
			{
				return LocomotionState.LocoEndLadder;
			}
			return LocomotionState.LocoEndLadderDown;
		}
		if (fromBrush.type == NavBrushComponent.Type.Flat && toBrush.type == NavBrushComponent.Type.Flat && (!fromBrush.hasPermanentConnections || !toBrush.hasPermanentConnections) && AreTravelBrushesConnected() && Vector3.Dot(fromBrush.transform.TransformDirection(fromBrush.normal), toBrush.transform.TransformDirection(toBrush.normal)) < 0.1f)
		{
			NavBoundaryComponent connectionBoundary7 = toBrush.GetConnectionBoundary(fromBrush);
			if (connectionBoundary7 != null && _walkRoundCorners)
			{
				if (Vector3.Dot(toBrush.transform.position - connectionBoundary7.transform.position, characterUp) > 0f)
				{
					return LocomotionState.LocoWalkUpCorner;
				}
				return LocomotionState.LocoWalkDownCorner;
			}
			return LocomotionState.LocoIdle;
		}
		return LocomotionState.LocoWalk;
	}

	protected bool AreTravelBrushesConnected()
	{
		if (_lastTravelBrushConnectionCheckFrameIdx != Time.frameCount)
		{
			UpdateAreTravelBrushesConnected();
		}
		return _areTravelBrushesConnected;
	}

	private void UpdateAreTravelBrushesConnected()
	{
		if ((bool)GameScene.navManager)
		{
			_lastTravelBrushConnectionCheckFrameIdx = Time.frameCount;
			float connectionTolerance = NavManager.DefaultConnectionTolerance;
			if (!alwaysUseDefaultConnectionTolerance && this is CharacterLocomotion && !IsBrushPartOfLockableDraggable(_lastValidBrush) && IsBrushPartOfLockableDraggable(_targetBrush))
			{
				connectionTolerance = 0.1f;
			}
			_areTravelBrushesConnected = GameScene.navManager.TestNavBrushesAreStillConnected(_lastValidBrush, _targetBrush, connectionTolerance);
		}
	}

	protected bool IsDepthReferenceValid(NavBrushComponent brush)
	{
		if (brush.depthReferenceObject != null)
		{
			if (!brush.depthRefOnlyWhenConnected)
			{
				if (Vector3.Dot(brush.transform.TransformDirection(brush.normal), brush.depthReferenceObject.transform.position - brush.transform.position) < 0f)
				{
					return false;
				}
				return true;
			}
			NavBrushComponent component = brush.depthReferenceObject.GetComponent<NavBrushComponent>();
			DebugUtils.DebugAssert(component != null);
			return GameScene.navManager.TestNavBrushesAreStillConnected(brush, component, NavManager.DefaultConnectionTolerance);
		}
		return false;
	}

	protected virtual void UpdateBlockedState()
	{
		animSystem.Play("Idle");
		if (lastValidBrush != null)
		{
			Vector3 vector = lastValidBrush.transform.position;
			if (IsDepthReferenceValid(lastValidBrush))
			{
				vector = CalculateFromDepthReference(vector, lastValidBrush.depthReferenceObject.transform.position);
			}
			base.transform.position = vector;
			base.transform.rotation = GetBetweenBrushOrientation(0f);
		}
	}

	protected virtual void UpdateCustomAnimState()
	{
		if (_customAnimRequest.isPending)
		{
			if (_customAnimRequest.blend)
			{
				animSystem.CrossFade(_customAnimRequest.animName, 1f, PlayMode.StopSameLayer);
			}
			else
			{
				animSystem.Play(_customAnimRequest.animName);
			}
			if (_customAnimRequest.startNormalizeTime != 0f)
			{
				animSystem[_customAnimRequest.animName].normalizedTime = _customAnimRequest.startNormalizeTime;
			}
			_customAnimRequest.isPending = false;
		}
		if (!animSystem.isPlaying)
		{
			locoState = LocomotionState.LocoIdle;
			UpdateIdleState();
		}
		else if (!_customAnimRequest.ignoreNav && lastValidBrush != null)
		{
			Vector3 vector = lastValidBrush.transform.position;
			if (IsDepthReferenceValid(lastValidBrush))
			{
				vector = CalculateFromDepthReference(vector, lastValidBrush.depthReferenceObject.transform.position);
			}
			base.transform.position = vector;
			base.transform.rotation = GetBetweenBrushOrientation(0f);
		}
	}

	protected virtual void UpdateIdleState()
	{
	}

	protected virtual void UpdateStairsIdle()
	{
		if (prevUpDownDir == UpDownState.Up)
		{
			animSystem.Play("StairsUpIdle" + animSuffix);
		}
		else
		{
			animSystem.Play("StairsDownIdle" + animSuffix);
		}
		Vector3 position = lastValidBrush.transform.position;
		base.transform.position = position;
		base.transform.rotation = GetBetweenBrushOrientation(interpT);
	}

	protected virtual void UpdateIdleLadderState()
	{
		Vector3 forward = targetBrush.transform.TransformDirection(-targetBrush.normal);
		if (prevUpDownDir == UpDownState.Up)
		{
			animSystem.Play("LadderUpIdle");
		}
		else
		{
			animSystem.Play("LadderDownIdle");
			forward = targetBrush.transform.TransformDirection(targetBrush.normal);
		}
		Vector3 position = lastValidBrush.transform.position;
		base.transform.position = position;
		forward.Normalize();
		currentDir = forward;
		base.transform.rotation = Quaternion.LookRotation(forward, targetBrush.transform.up);
	}

	protected Vector3 ProjectDirectionIntoBrushPlane(Vector3 dir, NavBrushComponent brush)
	{
		Vector3 up = brush.transform.up;
		Vector3 vector = dir;
		vector -= Vector3.Dot(vector, up) * up;
		vector.Normalize();
		if (vector == Vector3.zero)
		{
			Vector3 normal = brush.normal;
			vector = dir;
			if (Vector3.Dot(vector, up) > 0f)
			{
				normal *= -1f;
			}
			vector = brush.transform.TransformDirection(normal);
		}
		return vector;
	}

	protected Vector3 ProjectNavPointIntoBrushPlane(Vector3 point, NavBrushComponent brush)
	{
		Vector3 vector = brush.transform.position - point;
		Vector3 vector2 = Vector3.Project(vector, brush.transform.up);
		Vector3 vector3 = Vector3.Project(vector, -Camera.main.transform.forward);
		Vector3 vector4 = Vector3.Project(point + vector3 - brush.transform.position, brush.transform.up);
		Vector3 vector5 = point + vector2.magnitude / (vector2 + vector4).magnitude * vector3;
		RenderDebug.DrawLine(point, brush.transform.position, Color.blue);
		RenderDebug.DrawLine(point, vector5, Color.red);
		return vector5;
	}

	protected Quaternion GetBetweenBrushOrientation(float interpParam)
	{
		if ((bool)_exitBoundary)
		{
			Quaternion rotationOnBrushTowardsBoundary = GetRotationOnBrushTowardsBoundary(_lastValidBrush, _exitBoundary);
			Quaternion rotationOnBrushFromBoundary = GetRotationOnBrushFromBoundary(_targetBrush, _entryBoundary);
			if (!AreTravelBrushesConnected())
			{
				interpParam = Mathf.Round(interpParam);
			}
			return Quaternion.Lerp(rotationOnBrushTowardsBoundary, rotationOnBrushFromBoundary, interpParam);
		}
		DebugUtils.DebugAssert(interpParam == 0f, base.gameObject);
		return GetRotationOnBrushFromBoundary(_targetBrush, _entryBoundary);
	}

	private Quaternion GetRotationOnBrushFromBoundary(NavBrushComponent brush, NavBoundaryComponent fromBoundary)
	{
		return Quaternion.LookRotation(ProjectDirectionIntoBrushPlane(brush.transform.position - fromBoundary.transform.position, brush), brush.transform.up);
	}

	private Quaternion GetRotationOnBrushTowardsBoundary(NavBrushComponent brush, NavBoundaryComponent towardsBoundary)
	{
		return Quaternion.LookRotation(ProjectDirectionIntoBrushPlane(towardsBoundary.transform.position - brush.transform.position, brush), brush.transform.up);
	}

	protected virtual void UpdateWalkState()
	{
		animSystem.Play("Walk" + animSuffix);
		animSystem["Walk" + animSuffix].speed = characterVel / _walkAnimAuthoredSpeed;
		(lastValidBrush.transform.position - targetBrush.transform.position).Normalize();
		if (!targetBrush.gameObject.activeSelf)
		{
			Stop();
			return;
		}
		NavBrushComponent navBrushComponent = lastValidBrush;
		NavBrushComponent navBrushComponent2 = targetBrush;
		if (navBrushComponent.hasPermanentConnections && navBrushComponent2.hasPermanentConnections)
		{
			interpT = 1f;
			base.transform.position = targetBrush.transform.position;
			HideCharacter();
		}
		if (interpT < 1f)
		{
			base.transform.rotation = GetBetweenBrushOrientation(interpT);
			base.transform.position = GetBetweenBrushPosition(interpT);
			float num = 0.6f;
			if (this is CharacterLocomotion && !IsBrushPartOfLockableDraggable(_lastValidBrush) && IsBrushPartOfLockableDraggable(_targetBrush))
			{
				num = 1f;
			}
			if (interpT <= num && !AreTravelBrushesConnected())
			{
				Stop();
			}
		}
		interpT += Time.deltaTime * characterVel;
		if (interpT > 1f)
		{
			interpT -= 1f;
			IncrementNodes();
		}
		if (!navBrushComponent.hasPermanentConnections || !targetBrush.hasPermanentConnections)
		{
			return;
		}
		Vector3 position = targetBrush.transform.position;
		for (int i = 0; i < targetBrush.boundaries.Length; i++)
		{
			if (targetBrush.boundaries[i].permanentConnection == null)
			{
				position -= targetBrush.boundaries[i].transform.position;
				break;
			}
		}
		position.Normalize();
		currentDir = -position;
		base.transform.rotation = Quaternion.LookRotation(currentDir, characterUp);
	}

	private bool IsBrushPartOfLockableDraggable(NavBrushComponent brush)
	{
		Transform parent = brush.transform.parent;
		while ((bool)parent)
		{
			Draggable component = parent.GetComponent<Draggable>();
			if ((bool)component && component.lockWhenCharacterPresent)
			{
				return true;
			}
			RotationToMovement component2 = parent.GetComponent<RotationToMovement>();
			if ((bool)component2 && component2.lockWhenCharacterPresent)
			{
				return true;
			}
			parent = parent.parent;
		}
		return false;
	}

	protected Vector3 GetBetweenBrushPosition(float interpParam)
	{
		Vector3 currentPos;
		if (!_exitBoundary)
		{
			currentPos = lastValidBrush.GetNavPosition();
			if (IsDepthReferenceValid(lastValidBrush))
			{
				currentPos = CalculateFromDepthReference(currentPos, lastValidBrush.depthReferenceObject.transform.position);
			}
			return CalculateMagicPlayerProjection(currentPos);
		}
		Vector3 vector = ((interpParam < 0.5f) ? _exitBoundary.transform.position : _entryBoundary.transform.position);
		bool flag = AreTravelBrushesConnected();
		if (flag)
		{
			vector = 0.5f * (_exitBoundary.transform.position + _entryBoundary.transform.position);
		}
		Vector3 vector2 = lastValidBrush.GetNavPosition();
		if (IsDepthReferenceValid(lastValidBrush))
		{
			vector2 = CalculateFromDepthReference(vector2, lastValidBrush.depthReferenceObject.transform.position);
			vector = CalculateFromDepthReference(vector, lastValidBrush.depthReferenceObject.transform.position);
		}
		Vector3 vector3 = targetBrush.GetNavPosition();
		if (IsDepthReferenceValid(targetBrush))
		{
			vector3 = CalculateFromDepthReference(vector3, targetBrush.depthReferenceObject.transform.position);
			if (flag)
			{
				vector = CalculateFromDepthReference(vector, targetBrush.depthReferenceObject.transform.position);
			}
		}
		currentPos = vector2 + (vector3 - vector2) * interpParam;
		currentPos = ((!(interpParam < 0.5f)) ? Vector3.Lerp(vector, vector3, (interpParam - 0.5f) * 2f) : Vector3.Lerp(vector2, vector, interpParam * 2f));
		if (flag)
		{
			return CalculateMagicPlayerProjection(currentPos, vector2, vector3, characterUp);
		}
		if (interpT < 0.5f)
		{
			return CalculateMagicPlayerProjection(currentPos, vector2, vector2, characterUp);
		}
		return CalculateMagicPlayerProjection(currentPos, vector3, vector3, characterUp);
	}

	protected void UpdateWalkDownCornerState()
	{
		if (prevState != locoState)
		{
			animSystem.Play("WalkDown", PlayMode.StopAll);
			animSystem["WalkDown"].speed = ladderSpeed;
			animSystem.Sample();
		}
		if (!animSystem.isPlaying)
		{
			locoState = LocomotionState.LocoEmpty;
			interpT = 1f;
			base.transform.rotation = GetBetweenBrushOrientation(0f);
			base.transform.position = lastValidBrush.transform.position;
			IncrementNodes();
		}
		else
		{
			Vector3 position = lastValidBrush.transform.position;
			base.transform.position = position;
			base.transform.rotation = GetBetweenBrushOrientation(0f);
		}
	}

	protected void UpdateWalkUpCornerState()
	{
		if (prevState != locoState)
		{
			animSystem.Play("WalkUp", PlayMode.StopAll);
			animSystem["WalkUp"].speed = ladderSpeed;
			animSystem.Sample();
		}
		if (!animSystem.isPlaying)
		{
			locoState = LocomotionState.LocoEmpty;
			interpT = 1f;
			base.transform.rotation = GetBetweenBrushOrientation(0f);
			base.transform.position = lastValidBrush.transform.position;
			IncrementNodes();
		}
		else
		{
			Vector3 position = lastValidBrush.transform.position;
			base.transform.position = position;
			base.transform.rotation = GetBetweenBrushOrientation(0f);
		}
	}

	protected void UpdateStartClimbState()
	{
		prevUpDownDir = UpDownState.Up;
		if (prevState != locoState)
		{
			animSystem.Play("LadderUpIn", PlayMode.StopAll);
			animSystem["LadderUpIn"].speed = ladderSpeed;
			animSystem.Sample();
		}
		Vector3 position = lastValidBrush.transform.position;
		base.transform.position = position;
		Vector3 vector = targetBrush.transform.TransformDirection(-targetBrush.normal);
		vector.Normalize();
		if (Vector3.Dot(currentDir, vector) < -0.99f)
		{
			vector += new Vector3(0f - vector.z, 0f, vector.x);
		}
		currentDir += (vector - currentDir) * characterRotVel * Time.deltaTime;
		currentDir.Normalize();
		base.transform.rotation = GetBetweenBrushOrientation(interpT);
		if (!animSystem.isPlaying)
		{
			IncrementNodes();
		}
	}

	protected void UpdateClimbState()
	{
		prevUpDownDir = UpDownState.Up;
		if (prevState != locoState)
		{
			animSystem.Play("LadderUpClimb", PlayMode.StopAll);
			AnimationState animationState = animSystem["LadderUpClimb"];
			animationState.normalizedTime = 0f;
			animationState.speed = ladderSpeed;
			interpT = 0f;
			Vector3 vector = targetBrush.transform.TransformDirection(-targetBrush.normal);
			vector.Normalize();
			currentDir = vector;
			base.transform.rotation = Quaternion.LookRotation(currentDir, characterUp);
			Vector3 position = lastValidBrush.transform.position;
			base.transform.position = position;
			interpT = 0f;
		}
		(lastValidBrush.transform.position - targetBrush.transform.position).Normalize();
		base.transform.rotation = GetBetweenBrushOrientation(interpT);
		if (animSystem.isPlaying)
		{
			AnimationState animationState2 = animSystem["LadderUpClimb"];
			if (animationState2.normalizedTime >= 1f)
			{
				interpT = animationState2.normalizedTime;
			}
			base.transform.rotation = Quaternion.LookRotation(currentDir, characterUp);
			Vector3 position2 = lastValidBrush.transform.position;
			base.transform.position = position2;
			return;
		}
		IncrementNodes();
		if (lastValidBrush.normal.y < 0.3f && targetBrush.normal.y < 0.3f)
		{
			animSystem.Play("LadderUpClimb");
			animSystem["LadderUpClimb"].normalizedTime = 0f;
			Vector3 position3 = lastValidBrush.transform.position;
			base.transform.position = position3;
			interpT = 0f;
		}
	}

	protected void UpdateEndClimbState()
	{
		if (prevState != locoState)
		{
			animSystem.Play("LadderUpOut");
			animSystem["LadderUpOut"].speed = ladderSpeed;
			Vector3 position = lastValidBrush.transform.position;
			base.transform.position = position;
			Vector3 vector = lastValidBrush.transform.TransformDirection(-lastValidBrush.normal);
			vector.Normalize();
			currentDir = vector;
			base.transform.rotation = Quaternion.LookRotation(currentDir, characterUp);
		}
		base.transform.rotation = GetBetweenBrushOrientation(interpT);
		if (!animSystem.isPlaying)
		{
			IncrementNodes();
			interpT = 0f;
		}
		else
		{
			Vector3 position2 = lastValidBrush.transform.position;
			base.transform.position = position2;
		}
	}

	protected void UpdateStartClimbStateDown()
	{
		prevUpDownDir = UpDownState.Down;
		if (prevState != locoState)
		{
			animSystem.Play("LadderDownIn");
			animSystem["LadderDownIn"].speed = ladderSpeed;
		}
		Vector3 vector = targetBrush.transform.TransformDirection(targetBrush.normal);
		vector.Normalize();
		if (Vector3.Dot(currentDir, vector) < -0.99f)
		{
			Vector3 vector2 = Vector3.Cross(characterUp, vector);
			vector2.Normalize();
			vector += vector2 * 0.1f;
		}
		currentDir += (vector - currentDir) * characterRotVel * Time.deltaTime;
		currentDir.Normalize();
		base.transform.rotation = GetBetweenBrushOrientation(interpT);
		Vector3 position = lastValidBrush.transform.position;
		base.transform.position = position;
		if (!animSystem.isPlaying)
		{
			IncrementNodes();
			interpT = 0f;
		}
	}

	protected void UpdateClimbStateDown()
	{
		prevUpDownDir = UpDownState.Down;
		if (prevState != locoState)
		{
			animSystem.Play("LadderDownClimb");
			AnimationState animationState = animSystem["LadderDownClimb"];
			animationState.normalizedTime = 0f;
			animationState.speed = ladderSpeed;
			animSystem.Sample();
			interpT = 0f;
			Vector3 position = lastValidBrush.transform.position;
			Vector3 vector = targetBrush.transform.TransformDirection(targetBrush.normal);
			vector.Normalize();
			currentDir = vector;
			base.transform.rotation = Quaternion.LookRotation(currentDir, characterUp);
			base.transform.position = position;
			interpT = 0f;
		}
		(lastValidBrush.transform.position - targetBrush.transform.position).Normalize();
		base.transform.rotation = GetBetweenBrushOrientation(interpT);
		if (animSystem.isPlaying)
		{
			AnimationState animationState2 = animSystem["LadderDownClimb"];
			if (animationState2.normalizedTime >= 1f)
			{
				interpT = animationState2.normalizedTime;
			}
			Vector3 position2 = lastValidBrush.transform.position;
			base.transform.position = position2;
			return;
		}
		IncrementNodes();
		if (lastValidBrush.normal.y < 0.3f && targetBrush.normal.y < 0.3f)
		{
			animSystem.Play("LadderDownClimb");
			animSystem["LadderDownClimb"].normalizedTime = 0f;
			Vector3 position3 = lastValidBrush.transform.position;
			base.transform.position = position3;
			interpT = 0f;
		}
	}

	protected void UpdateEndClimbStateDown()
	{
		if (prevState != locoState)
		{
			animSystem.Play("LadderDownOut");
			animSystem["LadderDownOut"].speed = ladderSpeed;
			interpT = 0f;
			Vector3 position = lastValidBrush.transform.position;
			Vector3 position2 = lastValidBrush.transform.position;
			Vector3 position3 = targetBrush.transform.position;
			position = CalculateMagicPlayerProjection(position, position2, position3, characterUp);
			base.transform.position = position;
			Vector3 vector = lastValidBrush.transform.TransformDirection(lastValidBrush.normal);
			vector.Normalize();
			currentDir = vector;
			base.transform.rotation = Quaternion.LookRotation(currentDir, characterUp);
		}
		base.transform.rotation = GetBetweenBrushOrientation(interpT);
		if (!animSystem.isPlaying)
		{
			IncrementNodes();
			return;
		}
		Vector3 position4 = lastValidBrush.transform.position;
		Vector3 vector2 = entryBoundary.transform.position - exitBoundary.transform.position;
		if (Vector3.Dot(vector2, Camera.main.transform.forward) < 0f)
		{
			Vector3 vector3 = Vector3.Project(vector2, Camera.main.transform.forward);
			position4 += vector3;
		}
		base.transform.position = position4;
	}

	protected void UpdateStartStairsUpState()
	{
		AnimationState animationState = animSystem["StairsUpIn" + animSuffix];
		prevUpDownDir = UpDownState.Up;
		if (prevState != locoState)
		{
			animSystem.Play("StairsUpIn" + animSuffix, PlayMode.StopAll);
			animationState.speed = stairSpeed;
			animSystem.Sample();
		}
		Vector3 stairPosition = GetStairPosition();
		base.transform.position = stairPosition;
		interpT = animationState.normalizedTime;
		base.transform.rotation = GetBetweenBrushOrientation(interpT);
		if (!animSystem.isPlaying)
		{
			IncrementNodes();
		}
		CheckAndUpdateStairDisconnection(animationState);
	}

	protected void UpdateStairsUpState()
	{
		AnimationState animationState = animSystem["StairsUpClimb" + animSuffix];
		prevUpDownDir = UpDownState.Up;
		if (prevState != locoState)
		{
			animSystem.Play("StairsUpClimb" + animSuffix, PlayMode.StopAll);
			animationState.normalizedTime = 0f;
			animationState.speed = stairSpeed;
			interpT = 0f;
		}
		(lastValidBrush.transform.position - targetBrush.transform.position).Normalize();
		interpT = animationState.normalizedTime;
		base.transform.rotation = GetBetweenBrushOrientation(interpT);
		if (animSystem.isPlaying)
		{
			if (animationState.normalizedTime >= 1f)
			{
				interpT = animationState.normalizedTime;
			}
			Vector3 position = lastValidBrush.transform.position;
			base.transform.position = position;
			return;
		}
		IncrementNodes();
		if (SelectLocoState(lastValidBrush, targetBrush) == LocomotionState.LocoStairsUp)
		{
			animSystem.Play("StairsUpClimb" + animSuffix);
			animationState.normalizedTime = 0f;
			Vector3 position2 = lastValidBrush.transform.position;
			base.transform.position = position2;
			interpT = 0f;
		}
	}

	protected void UpdateEndStairsUpState()
	{
		AnimationState animationState = animSystem["StairsUpOut" + animSuffix];
		if (prevState != locoState)
		{
			animSystem.Play("StairsUpOut" + animSuffix);
			animationState.speed = stairSpeed;
		}
		interpT = animationState.normalizedTime;
		base.transform.rotation = GetBetweenBrushOrientation(interpT);
		if (!animSystem.isPlaying)
		{
			IncrementNodes();
			interpT = 0f;
		}
		else
		{
			Vector3 stairPosition = GetStairPosition();
			base.transform.position = stairPosition;
		}
		CheckAndUpdateStairDisconnection(animationState);
	}

	protected void UpdateStartStairsDownState()
	{
		prevUpDownDir = UpDownState.Down;
		AnimationState animationState = animSystem["StairsDownIn" + animSuffix];
		if (prevState != locoState)
		{
			animSystem.Play("StairsDownIn" + animSuffix, PlayMode.StopAll);
			animationState.speed = stairSpeed;
			animSystem.Sample();
		}
		Vector3 stairPosition = GetStairPosition();
		base.transform.position = stairPosition;
		interpT = animationState.normalizedTime;
		base.transform.rotation = GetBetweenBrushOrientation(interpT);
		if (!animSystem.isPlaying)
		{
			IncrementNodes();
		}
		CheckAndUpdateStairDisconnection(animationState);
	}

	protected void UpdateStairsDownState()
	{
		prevUpDownDir = UpDownState.Down;
		if (prevState != locoState)
		{
			animSystem.Play("StairsDownClimb" + animSuffix, PlayMode.StopAll);
			AnimationState animationState = animSystem["StairsDownClimb" + animSuffix];
			animationState.normalizedTime = 0f;
			animationState.speed = stairSpeed;
			interpT = 0f;
		}
		(lastValidBrush.transform.position - targetBrush.transform.position).Normalize();
		if (animSystem.isPlaying)
		{
			AnimationState animationState2 = animSystem["StairsDownClimb" + animSuffix];
			if (animationState2.normalizedTime >= 1f)
			{
				interpT = animationState2.normalizedTime;
			}
			Vector3 stairPosition = GetStairPosition();
			base.transform.position = stairPosition;
			base.transform.rotation = GetBetweenBrushOrientation(interpT);
			return;
		}
		IncrementNodes();
		if (SelectLocoState(lastValidBrush, targetBrush) == LocomotionState.LocoStairsDown)
		{
			animSystem.Play("StairsDownClimb" + animSuffix);
			animSystem["StairsDownClimb" + animSuffix].normalizedTime = 0f;
			Vector3 stairPosition2 = GetStairPosition();
			base.transform.position = stairPosition2;
			interpT = 0f;
			base.transform.rotation = GetBetweenBrushOrientation(interpT);
		}
	}

	protected void UpdateEndStairsDownState()
	{
		AnimationState animationState = animSystem["StairsDownOut" + animSuffix];
		if (prevState != locoState)
		{
			animSystem.Play("StairsDownOut" + animSuffix);
			animationState.speed = stairSpeed;
		}
		interpT = animationState.normalizedTime;
		base.transform.rotation = GetBetweenBrushOrientation(interpT);
		if (!animSystem.isPlaying)
		{
			IncrementNodes();
			interpT = 0f;
		}
		else
		{
			base.transform.position = GetStairPosition();
		}
		CheckAndUpdateStairDisconnection(animationState);
	}

	private void CheckAndUpdateStairDisconnection(AnimationState state)
	{
		if (!lastValidBrush.hasPermanentConnections && !AreTravelBrushesConnected())
		{
			if (state.normalizedTime < 0.5f)
			{
				SetLocomotionNodes(lastValidBrush, base.transform.forward);
				Stop();
			}
			else
			{
				IncrementNodes();
				interpT = 0f;
			}
		}
	}

	private Vector3 GetStairPosition()
	{
		Vector3 position = lastValidBrush.transform.position;
		position = CalculateMagicPlayerProjection(position, lastValidBrush.transform.position, targetBrush.transform.position, characterUp);
		Vector3 vector = position - lastValidBrush.transform.position;
		if (vector.sqrMagnitude > 0.1f)
		{
			if ((lastValidBrush.GetConnectionBoundary(targetBrush).transform.position - targetBrush.GetConnectionBoundary(lastValidBrush).transform.position).sqrMagnitude > 0.2f)
			{
				vector.Normalize();
				position += vector;
			}
			else
			{
				position = lastValidBrush.transform.position;
			}
		}
		return position;
	}

	public void ShowCharacter()
	{
		Renderer[] componentsInChildren = GetComponentsInChildren<Renderer>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].enabled = true;
		}
	}

	public void HideCharacter()
	{
		Renderer[] componentsInChildren = GetComponentsInChildren<Renderer>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].enabled = false;
		}
	}

	protected void OnDrawGizmosSelected()
	{
		if ((bool)lastValidBrush)
		{
			Gizmos.color = Color.red;
			Gizmos.DrawLine(base.transform.position + 0.5f * base.transform.up, lastValidBrush.transform.position);
			if ((bool)exitBoundary)
			{
				Gizmos.DrawLine(exitBoundary.transform.position, lastValidBrush.transform.position);
			}
		}
		if (targetBrush != lastValidBrush)
		{
			Gizmos.color = Color.green;
			Gizmos.DrawLine(base.transform.position + 0.5f * base.transform.up, targetBrush.transform.position);
			if ((bool)entryBoundary)
			{
				Gizmos.DrawLine(entryBoundary.transform.position, targetBrush.transform.position);
			}
		}
		if (_lastValidBrush != _targetBrush && (bool)GameScene.navManager && !AreTravelBrushesConnected())
		{
			RenderDebug.DrawLine(base.transform.position + 3f * base.transform.up, _lastValidBrush.transform.position, Color.red);
			RenderDebug.DrawLine(base.transform.position + 3f * base.transform.up, _targetBrush.transform.position, Color.red);
		}
	}

	[TriggerableAction]
	public IEnumerator ClearLookAtTarget()
	{
		lookAtObject = false;
		lookAtObjectTarget = null;
		return null;
	}
}
