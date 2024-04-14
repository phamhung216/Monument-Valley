using System;
using System.Collections;
using System.Collections.Generic;
using Fabric;
using UnityEngine;

public class CharacterLocomotion : BaseLocomotion
{
	public Transform thunderbird;

	public NavRequest navRequest;

	private List<NavBrushComponent> currentRoute;

	private int currentNodeID;

	private int nextNodeID;

	public GameObject animatedObject;

	public Transform shadowRootTransform;

	private BaseLocomotion[] allAIObjects;

	public bool sad;

	public float sadWalkSpeed = 2f;

	public bool curseLifting;

	public float headTurnAngle = 135f;

	public float headTurnSpeed = 0.2f;

	public bool hatRemoved;

	private float _walkStartTime;

	private Vector3 lastValidVerticalPos = new Vector3(0f, 0f, 0f);

	private bool navResetPending;

	private bool endSequencePending;

	private bool endSequenceInProgress;

	private bool endingAnimLoop;

	private Matrix4x4 hatOffset;

	private int _frameCounter;

	private Vector3 headUp;

	public Transform HeadBone;

	public Transform HatBone;

	private Quaternion prevLookDir;

	private int endingAnimIdx;

	private string[] endingAnims = new string[5] { "GoToSleep", "Sleeping", "Sleeping", "WakeUp", "WalkBackwards" };

	private float maxInterpToStopAtLastValidBrush = 0.1f;

	private BaseLocomotion _threat;

	public override NavBrushComponent lastValidBrush => base.lastValidBrush;

	public int getCurrentNode()
	{
		return currentNodeID;
	}

	public int getNextNode()
	{
		return nextNodeID;
	}

	public override NavBrushComponent getTargetBrush()
	{
		return targetBrush;
	}

	private void Start()
	{
		InitBase();
		base.locoState = LocomotionState.LocoIdle;
		prevState = LocomotionState.LocoIdle;
		interpT = 0f;
		currentNodeID = 0;
		nextNodeID = 0;
		currentDir = new Vector3(0f, 0f, 1f);
		if (!GameScene.navManager)
		{
			throw new Exception("Missing NavManager for Player object");
		}
		Checkpoint checkpoint = LevelProgress.FastForwardLevel();
		if ((bool)checkpoint)
		{
			base.transform.position = checkpoint.transform.position;
			base.transform.rotation = checkpoint.transform.rotation;
			GameScene.navManager.ScanAllConnections();
		}
		if ((bool)GameScene.instance)
		{
			Vector3 point = GameScene.WorldToPanPoint(base.transform.position);
			NavBrushComponent navBrushComponent = GameScene.navManager.FindNavBrushBelowPanPoint(point, touchableOnly: false);
			if (!navBrushComponent)
			{
				D.Error("Failed to find nav node for player at " + base.transform.position.ToString());
			}
			SetLocomotionNodes(navBrushComponent, base.transform.forward);
		}
		animSystem = animatedObject.GetComponent<Animation>();
		allAIObjects = UnityEngine.Object.FindObjectsOfType(typeof(BaseLocomotion)) as BaseLocomotion[];
		lastValidVerticalPos = base.transform.position;
		hatOffset = HeadBone.worldToLocalMatrix * HatBone.localToWorldMatrix;
		headUp = base.transform.InverseTransformDirection(-HeadBone.right);
	}

	protected override void SetLocomotionNodes(NavBrushComponent brush, Vector3 direction)
	{
		base.SetLocomotionNodes(brush, direction);
		if (!lastValidBrush.isAccessibleByPlayer && (bool)GameScene.navManager)
		{
			GameScene.navManager.NotifyReconfigurationEnded();
		}
	}

	protected override void SetLocomotionNodes(NavBrushComponent from, NavBrushComponent to)
	{
		base.SetLocomotionNodes(from, to);
		if (!lastValidBrush.isAccessibleByPlayer && (bool)GameScene.navManager)
		{
			GameScene.navManager.NotifyReconfigurationEnded();
		}
	}

	private void UpdateAutoHeadMovement()
	{
		float t = 1f;
		Vector3 vector;
		if (lookAtObject && (bool)lookAtObjectTarget)
		{
			vector = lookAtObjectTarget.position;
		}
		else if (Camera.main.GetComponent<TouchHandler>().touchActive)
		{
			vector = Camera.main.GetComponent<TouchHandler>().touchPosWorldSpace;
		}
		else if (base.locoState == LocomotionState.LocoCustomAnim)
		{
			float num = 1f;
			t = 1f - Mathf.Clamp(animSystem[_customAnimRequest.animName].time / num, 0f, 1f);
			if (_customAnimRequest.detachHat)
			{
				t = 0f;
			}
			vector = HeadBone.TransformPoint(10f * Vector3.forward);
		}
		else
		{
			vector = HeadBone.TransformPoint(10f * Vector3.forward);
		}
		Vector3 vector2 = vector - HeadBone.transform.position;
		Quaternion quaternion = HeadBone.transform.rotation;
		if (vector2.sqrMagnitude < 400f)
		{
			Vector3 forward = HeadBone.transform.forward;
			Vector3 forward2 = base.transform.forward;
			Vector3 normalized = vector2.normalized;
			if (Vector3.Dot(forward2, normalized) > 1f - headTurnAngle / 180f && Vector3.Dot(forward, forward2) > 0.5f)
			{
				Vector3 upwards = base.transform.TransformDirection(headUp);
				quaternion = Quaternion.LookRotation(normalized, upwards) * Quaternion.AngleAxis(-90f, Vector3.forward);
			}
		}
		Quaternion b = Quaternion.Lerp(prevLookDir, quaternion, headTurnSpeed);
		HeadBone.transform.rotation = Quaternion.Lerp(quaternion, b, t);
		if (base.locoState != LocomotionState.LocoCustomAnim || !_customAnimRequest.detachHat || (lookAtObject && (bool)lookAtObjectTarget))
		{
			HatBone.position = HeadBone.localToWorldMatrix * hatOffset * new Vector4(0f, 0f, 0f, 1f);
			Vector3 upwards2 = (HeadBone.localToWorldMatrix * hatOffset).MultiplyVector(Vector3.up);
			Vector3 forward3 = (HeadBone.localToWorldMatrix * hatOffset).MultiplyVector(Vector3.forward);
			HatBone.rotation = Quaternion.LookRotation(forward3, upwards2);
		}
		prevLookDir = HeadBone.transform.rotation;
		if (hatRemoved)
		{
			HatBone.localScale = Vector3.zero;
		}
	}

	private void DebugTeleport()
	{
		if (Input.GetMouseButtonUp(1))
		{
			TeleportPlayerToScreenPosition(Input.mousePosition);
		}
	}

	public void TeleportPlayerToScreenPosition(Vector2 screenPosition)
	{
		NavBrushComponent navBrushComponent = GameScene.navManager.FindNavBrushBelowPanPoint(GameScene.ScreenToPanPoint(screenPosition, GameScene.player.GetComponent<PlayerInput>().currentCamera), touchableOnly: true);
		if ((bool)navBrushComponent)
		{
			Teleport(navBrushComponent);
		}
	}

	private void LateUpdate()
	{
		if (endSequenceInProgress)
		{
			if (endSequencePending)
			{
				endSequencePending = false;
				base.shadow.gameObject.SetActive(value: false);
				animSystem.Play(endingAnims[endingAnimIdx], PlayMode.StopAll);
			}
			if (!animSystem.isPlaying && endingAnimIdx < endingAnims.Length - 1)
			{
				endingAnimIdx++;
				if (endingAnimLoop)
				{
					animSystem[endingAnims[endingAnimIdx]].wrapMode = WrapMode.Loop;
				}
				animSystem.Play(endingAnims[endingAnimIdx], PlayMode.StopAll);
			}
		}
		else
		{
			if (sad)
			{
				animSuffix = "Sad";
			}
			else if (curseLifting)
			{
				animSuffix = "CurseLifting";
			}
			else
			{
				animSuffix = "";
			}
			characterVel = (sad ? sadWalkSpeed : 3f);
			if (_customAnimRequest.isPending)
			{
				base.locoState = LocomotionState.LocoCustomAnim;
			}
			if (base.locoState != LocomotionState.LocoCustomAnim)
			{
				LocomotionState fromState = base.locoState;
				if (lastValidBrush != targetBrush && !lastValidBrush.GetConnectionBoundary(targetBrush))
				{
					SetLocomotionNodes(lastValidBrush, base.transform.forward);
				}
				base.locoState = SelectLocoState(lastValidBrush, targetBrush);
				if (DoesWalkTimerNeedReset(fromState, base.locoState))
				{
					_frameCounter = 0;
					_walkStartTime = Time.time;
				}
			}
			UpdateStates();
		}
		animSystem.Sample();
		UpdateAutoHeadMovement();
		UpdateShadow();
		_frameCounter--;
		if (_frameCounter < 0)
		{
			_frameCounter = 5;
			if ((bool)EventManager.Instance)
			{
				EventManager.Instance.SetParameter("Ida/Footstep/Default", "TimeSpentWalking", Time.time - _walkStartTime);
			}
		}
	}

	protected override void StartBlockedState()
	{
		interpT = 0f;
		SetLocomotionNodes(lastValidBrush, base.transform.forward);
		currentRoute = null;
		currentNodeID = 0;
		nextNodeID = 0;
		navRequest = null;
		base.locoState = LocomotionState.LocoBlocked;
	}

	private bool IsNextRouteNodeBlockedByCharacter(BaseLocomotion character)
	{
		if (targetBrush == character.currentBrush)
		{
			return true;
		}
		return false;
	}

	private bool IsAdjacentToThreat(BaseLocomotion character)
	{
		if (base.IsOnLadder || base.IsOnStairs)
		{
			return false;
		}
		if (lastValidBrush.IsConnectedToBrush(character.lastValidBrush) || lastValidBrush.IsConnectedToBrush(character.targetBrush))
		{
			return true;
		}
		return false;
	}

	protected override bool IsBlocked()
	{
		BaseLocomotion[] array = allAIObjects;
		foreach (BaseLocomotion baseLocomotion in array)
		{
			if (!(baseLocomotion != this))
			{
				continue;
			}
			AIController aIController = baseLocomotion as AIController;
			if ((bool)aIController && aIController.IsWalkingAroundCorner && (aIController.lastValidBrush == targetBrush || aIController.targetBrush == targetBrush))
			{
				_threat = baseLocomotion;
				return true;
			}
			if (lastValidBrush == targetBrush)
			{
				if (IsAdjacentToThreat(baseLocomotion) && !IsUntravelledRouteClearOfCharacter(baseLocomotion))
				{
					_threat = baseLocomotion;
					return true;
				}
			}
			else if (IsNextRouteNodeBlockedByCharacter(baseLocomotion))
			{
				_threat = baseLocomotion;
				return true;
			}
		}
		return false;
	}

	protected override void EndBlockedState()
	{
	}

	private bool DoesWalkTimerNeedReset(LocomotionState fromState, LocomotionState toState)
	{
		if (fromState == LocomotionState.LocoIdle || fromState == LocomotionState.LocoIdleLadder || fromState == LocomotionState.LocoStairsIdle)
		{
			return fromState != toState;
		}
		return false;
	}

	private bool IsUntravelledRouteClearOfCharacter(BaseLocomotion character)
	{
		if (currentRoute != null && currentRoute.Count > 1)
		{
			bool flag = false;
			for (int i = currentNodeID; i < currentRoute.Count; i++)
			{
				if (currentRoute[i] == character.currentBrush)
				{
					flag = true;
					break;
				}
			}
			return !flag;
		}
		return false;
	}

	private bool IsThreatenedByAICharacter()
	{
		_threat = null;
		return false;
	}

	public void LogRoute(string prefix)
	{
	}

	private bool ApplyPendingRequest(bool willIncrement)
	{
		if (!navResetPending)
		{
			return false;
		}
		if (navResetPending && navRequest != null && navRequest.status == NavRequest.RequestStatus.Complete)
		{
			navResetPending = false;
			currentNodeID = 0;
			nextNodeID = Mathf.Min(1, navRequest.route.Count - 1);
			interpT = 0f;
			currentRoute = new List<NavBrushComponent>(navRequest.route);
			if (willIncrement && currentRoute.Count > 1 && currentRoute[0] == lastValidBrush)
			{
				currentRoute.RemoveAt(0);
			}
			NavBrushComponent navBrushComponent = lastValidBrush;
			NavBrushComponent navBrushComponent2 = null;
			if (currentNodeID < currentRoute.Count)
			{
				navBrushComponent = currentRoute[currentNodeID];
			}
			navBrushComponent2 = ((nextNodeID >= currentRoute.Count) ? lastValidBrush : currentRoute[nextNodeID]);
			if (navBrushComponent != navBrushComponent2 && !GameScene.navManager.TestNavBrushesAreStillConnected(navBrushComponent, navBrushComponent2, NavManager.DefaultConnectionTolerance))
			{
				navBrushComponent2 = navBrushComponent;
				currentRoute = null;
			}
			if (navBrushComponent == navBrushComponent2)
			{
				SetLocomotionNodes(navBrushComponent, base.transform.forward);
			}
			else
			{
				SetLocomotionNodes(navBrushComponent, navBrushComponent2);
			}
			if (currentRoute == null || currentRoute.Count < 1)
			{
				base.locoState = getCorrectIdleState(lastValidBrush);
			}
			return true;
		}
		if (navRequest == null)
		{
			currentRoute = null;
			currentNodeID = 0;
			nextNodeID = 0;
			base.locoState = getCorrectIdleState(lastValidBrush);
			return true;
		}
		return false;
	}

	protected override void IncrementNodes()
	{
		_ = currentNodeID;
		_ = nextNodeID;
		bool flag = false;
		if (navResetPending)
		{
			flag = ApplyPendingRequest(willIncrement: true);
			if (currentRoute == null)
			{
				SetLocomotionNodes(lastValidBrush, base.transform.forward);
				return;
			}
		}
		if (!flag)
		{
			currentNodeID = Mathf.Min(currentNodeID + 1, currentRoute.Count - 1);
			nextNodeID = Mathf.Min(nextNodeID + 1, currentRoute.Count - 1);
		}
		skipTransitionZonesNodes();
		NavBrushComponent navBrushComponent = currentRoute[currentNodeID];
		NavBrushComponent navBrushComponent2 = ((nextNodeID < currentRoute.Count) ? currentRoute[nextNodeID] : navBrushComponent);
		if (navBrushComponent != navBrushComponent2)
		{
			bool flag2 = false;
			if (!GameScene.navManager.TestNavBrushesAreStillConnected(navBrushComponent, navBrushComponent2, NavManager.DefaultConnectionTolerance) && !IsBrushTeleporter(navBrushComponent))
			{
				flag2 = true;
			}
			if (flag2)
			{
				navBrushComponent2 = navBrushComponent;
				currentRoute = null;
				base.locoState = getCorrectIdleState(navBrushComponent);
			}
		}
		if (navBrushComponent == navBrushComponent2)
		{
			SetLocomotionNodes(navBrushComponent, base.transform.forward);
		}
		else
		{
			SetLocomotionNodes(navBrushComponent, navBrushComponent2);
		}
	}

	[TriggerableAction]
	public IEnumerator StopMoving()
	{
		Stop();
		return null;
	}

	public override void Stop()
	{
		SetLocomotionNodes(lastValidBrush, base.transform.forward);
		currentRoute = null;
		base.locoState = getCorrectIdleState(lastValidBrush);
		interpT = 0f;
	}

	private bool ShouldSkipNode(NavBrushComponent brush)
	{
		NavBoundaryComponent[] componentsInChildren = brush.GetComponentsInChildren<NavBoundaryComponent>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			if ((componentsInChildren[i].transform.position - brush.transform.position).magnitude > 1E-05f)
			{
				return false;
			}
		}
		return true;
	}

	private void skipTransitionZonesNodes()
	{
		if (currentRoute != null)
		{
			while (currentNodeID < currentRoute.Count - 1 && ShouldSkipNode(currentRoute[currentNodeID]))
			{
				currentNodeID++;
			}
			while (nextNodeID < currentRoute.Count - 1 && ShouldSkipNode(currentRoute[nextNodeID]))
			{
				nextNodeID++;
			}
		}
	}

	[TriggerableAction]
	public IEnumerator ExitDoorBrush()
	{
		if (IsBrushTeleporter(lastValidBrush))
		{
			ShowCharacter();
			InsertTeleporterExitPoint(lastValidBrush);
			ApplyPendingRequest(willIncrement: false);
			lastValidBrush.GetOwningDoor().NotifyDelayedExit();
		}
		return null;
	}

	private void InsertTeleporterExitPoint(NavBrushComponent targetBrush)
	{
		NavRequest navRequest = new NavRequest();
		NavBoundaryComponent[] boundaries = targetBrush.boundaries;
		foreach (NavBoundaryComponent navBoundaryComponent in boundaries)
		{
			if (navBoundaryComponent.permanentConnection == null && navBoundaryComponent.links.Count > 0)
			{
				navRequest.route.Add(lastValidBrush);
				navRequest.route.Add(navBoundaryComponent.links[0].GetOtherBoundary(navBoundaryComponent).transform.parent.GetComponent<NavBrushComponent>());
				navRequest.status = NavRequest.RequestStatus.Complete;
				AddMoveRequest(navRequest);
				break;
			}
		}
	}

	private bool IsBrushTeleporter(NavBrushComponent targetBrush)
	{
		if (targetBrush != null)
		{
			return targetBrush.hasPermanentConnections;
		}
		return false;
	}

	private string RouteToString(List<NavBrushComponent> route)
	{
		if (route == null)
		{
			return "";
		}
		string text = "{";
		for (int i = 0; i < route.Count; i++)
		{
			text += route[i].name;
			if (i < route.Count - 1)
			{
				text += ", ";
			}
		}
		return text + "}";
	}

	public void AddMoveRequest(NavRequest request)
	{
		if (base.locoState != LocomotionState.LocoBlocked || !_threat || !request.route.Contains(_threat.currentBrush) || !(_threat.currentBrush != lastValidBrush))
		{
			navRequest = request;
			if (request != null)
			{
				navResetPending = true;
			}
		}
	}

	public void Teleport(Vector3 newPos)
	{
		base.transform.position = newPos;
		Vector2 vector = GameScene.WorldToPanPoint(base.transform.position);
		SetLocomotionNodes(GameScene.navManager.FindNavBrushBelowPanPoint(vector, touchableOnly: false), base.transform.forward);
	}

	public void Teleport(NavBrushComponent brush)
	{
		SetLocomotionNodes(brush, base.transform.forward);
	}

	public override Vector3 getShadowRootPosition()
	{
		NavBrushComponent navBrushComponent = null;
		if (base.locoState == LocomotionState.LocoEndLadderDown)
		{
			navBrushComponent = ((lastValidBrush.type != NavBrushComponent.Type.Ladder) ? lastValidBrush : targetBrush);
		}
		else if (base.locoState == LocomotionState.LocoEndLadder)
		{
			navBrushComponent = ((lastValidBrush.type != NavBrushComponent.Type.Ladder) ? lastValidBrush : targetBrush);
		}
		else if (base.locoState == LocomotionState.LocoStartStairsUp || base.locoState == LocomotionState.LocoStairsUp || base.locoState == LocomotionState.LocoEndStairsUp || base.locoState == LocomotionState.LocoStartStairsDown || base.locoState == LocomotionState.LocoStairsDown || base.locoState == LocomotionState.LocoEndStairsDown || base.locoState == LocomotionState.LocoStairsIdle || base.locoState == LocomotionState.LocoCustomAnim)
		{
			navBrushComponent = null;
		}
		Vector3 vector = (0f - _magicPosCameraOffset) * Camera.main.transform.forward;
		Vector3 position = shadowRootTransform.transform.position;
		if ((bool)navBrushComponent)
		{
			Vector3 position2 = navBrushComponent.transform.InverseTransformPoint(position);
			position2.y = 0f;
			lastValidVerticalPos = navBrushComponent.transform.TransformPoint(position2);
		}
		else
		{
			lastValidVerticalPos = position - 0.5f * base.characterUp;
			lastValidVerticalPos += 2f * vector;
		}
		return lastValidVerticalPos;
	}

	public override float getShadowIntensity()
	{
		if (base.locoState == LocomotionState.LocoStartLadder)
		{
			AnimationState animationState = animSystem["LadderUpIn"];
			if (!animSystem.isPlaying)
			{
				return 0f;
			}
			float num = 1f - (animationState.normalizedTime - 0f) * 1f;
			if (num < 0f)
			{
				num = 0f;
			}
			return num;
		}
		if (base.locoState == LocomotionState.LocoStartLadderDown)
		{
			AnimationState animationState2 = animSystem["LadderDownIn"];
			if (!animSystem.isPlaying)
			{
				return 0f;
			}
			float num2 = 1f - (animationState2.normalizedTime - 0f) * 4f;
			if (num2 < 0f)
			{
				num2 = 0f;
			}
			return num2;
		}
		if (base.locoState == LocomotionState.LocoBlocked && lastValidBrush.type == NavBrushComponent.Type.Ladder)
		{
			return 0f;
		}
		if (base.locoState == LocomotionState.LocoIdleLadder)
		{
			return 0f;
		}
		if (base.locoState == LocomotionState.LocoLadder)
		{
			return 0f;
		}
		if (base.locoState == LocomotionState.LocoEndLadder)
		{
			AnimationState animationState3 = animSystem["LadderUpOut"];
			if (!animSystem.isPlaying)
			{
				return 1f;
			}
			float num3 = 1f;
			if (animationState3 != null)
			{
				num3 = (animationState3.normalizedTime - 0.75f) * 4f;
			}
			if (num3 > 1f)
			{
				num3 = 1f;
			}
			return num3;
		}
		if (base.locoState == LocomotionState.LocoLadderDown)
		{
			return 0f;
		}
		if (base.locoState == LocomotionState.LocoEndLadderDown)
		{
			AnimationState animationState4 = animSystem["LadderDownOut"];
			if (!animSystem.isPlaying)
			{
				return 1f;
			}
			float num4 = (animationState4.normalizedTime - 0f) * 1f;
			if (num4 > 1f)
			{
				num4 = 1f;
			}
			return num4;
		}
		return 1f;
	}

	public void StopAtTargetBrush()
	{
		if (currentRoute != null)
		{
			NavBrushComponent stopBrush = targetBrush;
			int num = currentRoute.FindLastIndex((NavBrushComponent brush) => stopBrush == brush);
			if (num >= 0)
			{
				currentRoute.RemoveRange(num + 1, currentRoute.Count - (num + 1));
			}
		}
	}

	public void StopMove()
	{
		NavRequest navRequest = new NavRequest();
		navRequest.status = NavRequest.RequestStatus.Complete;
		navRequest.route = new List<NavBrushComponent>();
		if (currentRoute != null)
		{
			if (currentNodeID >= 0)
			{
				navRequest.route.Add(currentRoute[currentNodeID]);
				navRequest.route.Add(currentRoute[currentNodeID]);
			}
			else
			{
				navRequest.route.Add(currentRoute[0]);
				navRequest.route.Add(currentRoute[0]);
			}
		}
		if (navRequest.route.Count == 0)
		{
			navRequest.route.Add(lastValidBrush);
			navRequest.route.Add(lastValidBrush);
		}
		AddMoveRequest(navRequest);
		ApplyPendingRequest(willIncrement: false);
	}

	protected override void UpdateBlockedState()
	{
		if (prevState != LocomotionState.LocoBlocked)
		{
			StartBlockedState();
		}
		if ((bool)lastValidBrush && lastValidBrush.type == NavBrushComponent.Type.Ladder)
		{
			UpdateIdleLadderState();
			return;
		}
		if (_threat is TotemPole)
		{
			PlayTotemAnimation(_threat as TotemPole);
		}
		else
		{
			animSystem.Play("Scared");
		}
		if (lastValidBrush != null)
		{
			if ((bool)_threat)
			{
				base.transform.rotation = GetFaceBrushRotation(_threat.currentBrush, lastValidBrush);
			}
			Vector3 vector = lastValidBrush.transform.position;
			if (IsDepthReferenceValid(lastValidBrush))
			{
				vector = CalculateFromDepthReference(vector, lastValidBrush.depthReferenceObject.transform.position);
			}
			base.transform.position = vector;
		}
	}

	protected override void UpdateStairsIdle()
	{
		bool flag = false;
		for (int i = 0; i < allAIObjects.Length; i++)
		{
			if (!(allAIObjects[i] != null))
			{
				continue;
			}
			Vector3 vector = base.transform.position - allAIObjects[i].transform.position;
			TotemPole totemPole = allAIObjects[i] as TotemPole;
			float num = (totemPole ? 1.1f : 2.25f);
			if (vector.sqrMagnitude < num && Vector3.Dot(vector.normalized, base.transform.forward) < -0.25f)
			{
				flag = true;
				if (allAIObjects[i] is TotemPole)
				{
					PlayTotemAnimation(totemPole);
				}
				else
				{
					animSystem.Play("Scared");
				}
				break;
			}
		}
		if (!flag)
		{
			if (prevUpDownDir == UpDownState.Up)
			{
				animSystem.Play("StairsUpIdle");
			}
			else
			{
				animSystem.Play("StairsDownIdle" + animSuffix);
			}
		}
		Vector3 position = lastValidBrush.transform.position;
		base.transform.position = position;
		base.transform.rotation = GetBetweenBrushOrientation(0f);
	}

	private void PlayTotemAnimation(TotemPole totem)
	{
		if (totem.IsHigherThanCube())
		{
			animSystem.Play("TouchTotem");
		}
		else
		{
			animSystem.Play("TouchTotemBlock");
		}
	}

	protected override void UpdateStates()
	{
		LocomotionState locomotionState = base.locoState;
		if ((uint)locomotionState <= 2u || (uint)(locomotionState - 18) <= 1u)
		{
			ApplyPendingRequest(willIncrement: false);
		}
		base.UpdateStates();
	}

	protected override void UpdateIdleState()
	{
		bool flag = false;
		for (int i = 0; i < allAIObjects.Length; i++)
		{
			if (!(allAIObjects[i] != null))
			{
				continue;
			}
			Vector3 vector = base.transform.position - allAIObjects[i].transform.position;
			TotemPole totemPole = allAIObjects[i] as TotemPole;
			float num = (totemPole ? 1.1f : 2.25f);
			float num2 = Vector3.Dot(base.transform.up, allAIObjects[i].transform.up);
			if (vector.sqrMagnitude < num && Vector3.Dot(vector.normalized, base.transform.forward) < -0.25f)
			{
				flag = true;
				if (allAIObjects[i] is TotemPole)
				{
					PlayTotemAnimation(totemPole);
				}
				else if (num2 == -1f)
				{
					flag = false;
				}
				else
				{
					animSystem.Play("Scared");
				}
				break;
			}
		}
		if (!flag)
		{
			animSystem.Play("Idle" + animSuffix);
		}
		if (lastValidBrush != null)
		{
			base.transform.position = GetBetweenBrushPosition(0f);
			base.transform.rotation = GetBetweenBrushOrientation(0f);
		}
		if (!IsBrushTeleporter(lastValidBrush))
		{
			return;
		}
		Vector3 position = lastValidBrush.transform.position;
		for (int j = 0; j < lastValidBrush.boundaries.Length; j++)
		{
			if (lastValidBrush.boundaries[j].permanentConnection == null)
			{
				position -= lastValidBrush.boundaries[j].transform.position;
				break;
			}
		}
		position.Normalize();
		currentDir = -position;
		base.transform.rotation = Quaternion.LookRotation(currentDir, base.characterUp);
		forceDirection(base.transform.rotation);
		bool flag2 = true;
		DoorComponent owningDoor = lastValidBrush.GetOwningDoor();
		if ((bool)owningDoor)
		{
			flag2 = owningDoor.autoExit;
		}
		if (flag2)
		{
			ExitDoorBrush();
		}
	}

	protected override void UpdateCustomAnimState()
	{
		base.UpdateCustomAnimState();
		if (base.locoState != LocomotionState.LocoCustomAnim)
		{
			prevLookDir = HeadBone.transform.rotation;
		}
	}

	[TriggerableAction]
	public IEnumerator TryToFlyAnim()
	{
		sad = true;
		RequestCustomAnim("TryToFly", ignoreNav: false, blend: true);
		return null;
	}

	[TriggerableAction]
	public IEnumerator MakeIdaSad()
	{
		sad = true;
		return null;
	}

	[TriggerableAction]
	public IEnumerator MakeIdaNotSad()
	{
		sad = false;
		return null;
	}

	[TriggerableAction]
	public IEnumerator StartCurseLift()
	{
		curseLifting = true;
		return null;
	}

	[TriggerableAction]
	public IEnumerator EndCurseLift()
	{
		curseLifting = false;
		return null;
	}

	[TriggerableAction]
	public IEnumerator GoToSleep()
	{
		RequestCustomAnim("GoToSleep");
		return null;
	}

	[TriggerableAction]
	public IEnumerator StartEndSequence()
	{
		endSequencePending = true;
		endSequenceInProgress = true;
		return null;
	}

	[TriggerableAction]
	public IEnumerator PickedUpByThunderbird()
	{
		endingAnims = new string[2] { "PickedUpByThunderbird", "TakeOff" };
		endingAnimIdx = -1;
		return null;
	}

	[TriggerableAction]
	public IEnumerator Fly()
	{
		endingAnims = new string[1] { "Fly" };
		endingAnimLoop = true;
		endingAnimIdx = -1;
		return null;
	}

	[TriggerableAction]
	public IEnumerator RemoveHat()
	{
		hatRemoved = true;
		return null;
	}

	[TriggerableAction]
	public IEnumerator ReturnHat()
	{
		hatRemoved = false;
		return null;
	}

	public void forceDirection(Vector3 lookDir)
	{
		Quaternion orientation = Quaternion.LookRotation(lookDir, base.characterUp);
		forceDirection(orientation);
	}

	public void forceDirection(Quaternion orientation)
	{
		base.transform.rotation = orientation;
	}

	private new void OnDrawGizmosSelected()
	{
		base.OnDrawGizmosSelected();
	}
}
