using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : BaseLocomotion
{
	private enum TextureAnimState
	{
		Blink = 0,
		Normal = 1,
		Amazed = 2
	}

	public enum AIBehaviorMode
	{
		WalkUpWalls = 0,
		NoWalkUpWalls = 1,
		Idle = 2,
		Sleeping = 3
	}

	private enum TurnDecision
	{
		Straight = 0,
		Left = 1,
		Right = 2,
		Backward = 3,
		Any = 4
	}

	private int frame;

	private float frameIncrement;

	private TextureAnimState textureAnimState;

	private TextureAnimState prevTextureAnimState;

	public Renderer mesh;

	public Material textureAnimMaterial;

	public AIBehaviorMode behaviorMode;

	private int _lastNextNavCheckFrameIdx;

	private static readonly int _navWaitFrames = 6;

	private float uniqueRandomValue;

	public bool isOnScreen;

	public bool debugMe;

	public bool ForceIdle;

	public bool IdleSitting;

	public List<NavBrushComponent> blackingNavPoints = new List<NavBrushComponent>();

	public List<NavBrushComponent> PathCompleteNavPoints = new List<NavBrushComponent>();

	public NavBrushComponent startNav;

	public NavBrushComponent startTargetNav;

	private bool triggerMelt;

	private bool melting;

	private CharacterLocomotion playerCharacter;

	private AnimationAudioEventController _audioEventController;

	public float headTurnAngle = 135f;

	public float headTurnSpeed = 0.2f;

	public float headLookAngleLimit = 85f;

	public float maxEyeCameraAngle = 20f;

	public float headLookDirFlatness = 0.25f;

	public Transform HeadBone;

	private Quaternion prevLookDir;

	public Transform chest;

	private bool triggerScream;

	private bool forceScream;

	private static bool s_blinkInUse = false;

	private float _blockedStartTime;

	public float minBlockedTime = 1f;

	public AnimationAudioEventController audioEventController => _audioEventController;

	public override NavBrushComponent getTargetBrush()
	{
		return targetBrush;
	}

	[TriggerableAction]
	public IEnumerator ChangeBehaviorWalkUpWalls()
	{
		behaviorMode = AIBehaviorMode.WalkUpWalls;
		return null;
	}

	[TriggerableAction]
	public IEnumerator ChangeBehaviorNoWalkUpWalls()
	{
		behaviorMode = AIBehaviorMode.NoWalkUpWalls;
		return null;
	}

	private new void Awake()
	{
		isOnScreen = true;
		base.Awake();
		if (base.transform.parent != null)
		{
			base.transform.parent = null;
		}
	}

	public override void InitNav()
	{
		base.InitNav();
		Vector3 point = GameScene.WorldToPanPoint(base.transform.position);
		NavBrushComponent navBrushComponent = (startNav ? startNav : GameScene.navManager.FindNavBrushBelowPanPoint(point, touchableOnly: false));
		if (!navBrushComponent)
		{
			Debug.Log("No nav brush for " + base.gameObject.name, base.gameObject);
		}
		NavBrushComponent navBrushComponent2 = (startTargetNav ? startTargetNav : FindNextAINavBrush(navBrushComponent, base.transform.forward));
		if ((bool)navBrushComponent2 && (!ForceIdle || !IdleSitting))
		{
			SetLocomotionNodes(navBrushComponent, navBrushComponent2);
		}
		else
		{
			SetLocomotionNodes(navBrushComponent, base.transform.forward);
		}
		currentDir = base.transform.forward;
		if (startNav == null)
		{
			startNav = lastValidBrush;
		}
		if (startTargetNav == null)
		{
			startTargetNav = targetBrush;
		}
		if (targetBrush == null || IsBlackingNavPoint(targetBrush) || targetBrush == lastValidBrush)
		{
			SetLocomotionNodes(lastValidBrush, base.transform.forward);
		}
	}

	private void Start()
	{
		_walkRoundCorners = true;
		uniqueRandomValue = Random.value;
		InitBase();
		_audioEventController = GetComponentInChildren<AnimationAudioEventController>();
		playerCharacter = GameScene.player.GetComponent<CharacterLocomotion>();
		if (!lookAtObjectTarget)
		{
			lookAtObjectTarget = playerCharacter.shadowRootTransform;
		}
	}

	private void UpdateAutoHeadMovement()
	{
		Vector3 vector = lookAtObjectTarget.position - HeadBone.position;
		Quaternion b = HeadBone.transform.rotation;
		if (lookAtObject && vector.sqrMagnitude < 10000f && !base.IsWalkingAroundCorner)
		{
			Vector3 forward = Camera.main.transform.forward;
			Vector3 forward2 = HeadBone.transform.forward;
			_ = base.transform.forward;
			Vector3 a = vector - Vector3.Dot(vector, forward) * forward;
			Vector3 normalized = Vector3.Lerp(a, vector, headLookDirFlatness).normalized;
			Vector3 rhs = forward2 - Vector3.Dot(forward2, forward) * forward;
			float f = 57.29578f * Mathf.Acos(Mathf.Clamp(Vector3.Dot(normalized, rhs), -1f, 1f));
			Vector3 up = base.transform.up;
			up -= Vector3.Dot(up, forward) * forward;
			up.Normalize();
			float value = Vector3.Angle(up, base.transform.up);
			up = Vector3.Lerp(t: Mathf.Clamp(value, 0f - maxEyeCameraAngle, maxEyeCameraAngle) / 90f, a: up, b: base.transform.up);
			up.Normalize();
			if (!(Mathf.Abs(f) > headLookAngleLimit))
			{
				b = Quaternion.LookRotation(normalized, up) * Quaternion.AngleAxis(-90f, Vector3.forward);
			}
		}
		HeadBone.transform.rotation = Quaternion.Lerp(prevLookDir, b, headTurnSpeed);
		prevLookDir = HeadBone.transform.rotation;
	}

	[TriggerableAction]
	public IEnumerator TriggerScream()
	{
		triggerScream = true;
		return null;
	}

	[TriggerableAction]
	public IEnumerator ForceScream()
	{
		forceScream = true;
		return null;
	}

	[TriggerableAction]
	public IEnumerator ForceScreamOff()
	{
		forceScream = false;
		return null;
	}

	[TriggerableAction]
	public IEnumerator TriggerMeltAndDestroy()
	{
		triggerMelt = true;
		melting = true;
		return null;
	}

	private void UpdateUpperBodyDelta()
	{
		Vector3 vector = base.transform.position - GameScene.player.transform.position;
		if (vector.sqrMagnitude < 2.25f && Vector3.Dot(vector.normalized, base.transform.forward) < -0.25f && !base.IsWalkingAroundCorner)
		{
			triggerScream = true;
		}
		if (triggerScream && Vector3.Dot(base.transform.up, GameScene.player.transform.up) == -1f)
		{
			triggerScream = false;
		}
		if ((forceScream || triggerScream) && !animSystem.IsPlaying("Scream"))
		{
			if (IdleSitting && ForceIdle)
			{
				animSystem.Play("SittingScream", PlayMode.StopSameLayer);
				triggerScream = false;
				return;
			}
			triggerScream = false;
			AnimationState animationState = animSystem["ScreamDelta"];
			animationState.layer = 1;
			animationState.AddMixingTransform(chest);
			animSystem.Play("ScreamDelta", PlayMode.StopSameLayer);
		}
		else if (triggerMelt)
		{
			RequestCustomAnim("Melt");
			triggerMelt = false;
		}
	}

	private void UpdateTextureAnimation()
	{
		if (textureAnimState == TextureAnimState.Amazed)
		{
			frame = 7;
		}
		else
		{
			if ((double)Random.value > 0.99 && !s_blinkInUse)
			{
				textureAnimState = TextureAnimState.Blink;
				s_blinkInUse = true;
			}
			if (textureAnimState != 0)
			{
				return;
			}
			if (prevTextureAnimState != textureAnimState)
			{
				frame = 0;
			}
			frameIncrement += Time.deltaTime;
			if (frameIncrement > 1f / 30f)
			{
				frameIncrement = 0f;
				frame++;
			}
			if (frame > 6)
			{
				textureAnimState = TextureAnimState.Normal;
				frame = -1;
				s_blinkInUse = false;
			}
		}
		prevTextureAnimState = textureAnimState;
		float num = 0f;
		float num2 = 0f;
		switch (frame)
		{
		case 0:
			num = -1f / 3f;
			num2 = 1f / 3f;
			break;
		case 1:
			num = -1f / 3f;
			num2 = 0f;
			break;
		case 2:
			num = -1f / 3f;
			num2 = -1f / 3f;
			break;
		case 3:
			num = 0f;
			num2 = 1f / 3f;
			break;
		case 4:
			num = -1f / 3f;
			num2 = -1f / 3f;
			break;
		case 5:
			num = -1f / 3f;
			num2 = 0f;
			break;
		case 6:
			num = -1f / 3f;
			num2 = 1f / 3f;
			break;
		case 7:
			num = -0f;
			num2 = -1f / 3f;
			break;
		default:
			num = 0f;
			num2 = 0f;
			break;
		}
		mesh.material.SetTextureOffset("_MainTex", new Vector2(num, num2));
	}

	public void ResetToStart()
	{
		SetLocomotionNodes(startNav, startTargetNav);
		base.transform.position = CalculateMagicPlayerProjection(startNav.transform.position, startNav.transform.position, startTargetNav.transform.position, base.characterUp);
	}

	public void Teleport(NavBrushComponent brush)
	{
		base.transform.position = brush.transform.position;
		SetLocomotionNodes(brush, base.transform.forward);
	}

	public override void Stop()
	{
		_lastNextNavCheckFrameIdx = Time.frameCount;
		if ((bool)lastValidBrush)
		{
			SetLocomotionNodes(direction: (lastValidBrush != targetBrush) ? (exitBoundary.transform.position - lastValidBrush.transform.position) : ((!lastValidBrush || !entryBoundary) ? base.transform.forward : (lastValidBrush.transform.position - entryBoundary.transform.position)), brush: lastValidBrush);
			interpT = 0f;
			base.locoState = getCorrectIdleState(lastValidBrush);
			base.transform.rotation = GetBetweenBrushOrientation(0f);
		}
	}

	private void LateUpdate()
	{
		UpdateTextureAnimation();
		if (lastValidBrush == null || targetBrush == null)
		{
			return;
		}
		if (!isOnScreen)
		{
			UpdateWhileOffScreen();
			return;
		}
		if (_customAnimRequest.isPending)
		{
			base.locoState = LocomotionState.LocoCustomAnim;
		}
		if (base.locoState == LocomotionState.LocoCustomAnim)
		{
			UpdateStates();
			animSystem.Sample();
		}
		else
		{
			if ((ForceIdle || lastValidBrush == targetBrush) && base.locoState != LocomotionState.LocoBlocked)
			{
				if (ForceIdle || lastValidBrush == targetBrush)
				{
					if (IdleSitting)
					{
						if (!animSystem.IsPlaying("SittingScream"))
						{
							animSystem.Play("Sitting");
							animSystem["Sitting"].speed = 0.75f + uniqueRandomValue * 0.5f;
						}
					}
					else
					{
						animSystem.Play("Idle");
					}
				}
				else
				{
					animSystem.Play("Scream");
				}
				Vector3 vector = lastValidBrush.transform.position + (targetBrush.transform.position - lastValidBrush.transform.position) * interpT;
				Vector3 vector2 = lastValidBrush.transform.position;
				if ((bool)targetBrush.depthReferenceObject)
				{
					vector2 = CalculateFromDepthReference(vector2, targetBrush.depthReferenceObject.transform.position);
				}
				if ((bool)lastValidBrush.depthReferenceObject)
				{
					vector2 = CalculateFromDepthReference(vector2, lastValidBrush.depthReferenceObject.transform.position);
				}
				if (Vector3.Dot(targetBrush.transform.TransformDirection(targetBrush.normal), Camera.main.transform.forward) < 0f && Vector3.Dot(lastValidBrush.transform.TransformDirection(lastValidBrush.normal), Camera.main.transform.forward) < 0f)
				{
					base.transform.position = CalculateMagicPlayerProjection(vector, vector2, targetBrush.transform.position, base.characterUp);
				}
				else
				{
					base.transform.position = vector;
				}
				base.transform.rotation = GetBetweenBrushOrientation(0f);
				if (!ForceIdle && Time.frameCount - _lastNextNavCheckFrameIdx >= _navWaitFrames)
				{
					NavBrushComponent navBrushComponent = FindNextAINavBrush(targetBrush, targetBrush.transform.position - entryBoundary.transform.position);
					_lastNextNavCheckFrameIdx = Time.frameCount;
					if (navBrushComponent == null)
					{
						SetLocomotionNodes(targetBrush, entryBoundary);
					}
					else
					{
						SetLocomotionNodes(targetBrush, navBrushComponent);
					}
				}
			}
			else
			{
				base.locoState = SelectLocoState(lastValidBrush, targetBrush);
				UpdateStates();
				animSystem.Sample();
			}
			for (int i = 0; i < PathCompleteNavPoints.Count; i++)
			{
				if (PathCompleteNavPoints[i] == lastValidBrush)
				{
					ForceIdle = true;
				}
			}
		}
		UpdateShadow();
		UpdateAutoHeadMovement();
		UpdateUpperBodyDelta();
		if (melting && base.locoState == LocomotionState.LocoIdle)
		{
			melting = false;
			base.gameObject.SetActive(value: false);
		}
	}

	public void WalkTo(NavBrushComponent brush)
	{
		SetLocomotionNodes(lastValidBrush, brush);
	}

	protected override void IncrementNodes()
	{
		NavBrushComponent navBrushComponent = targetBrush;
		if (targetBrush != lastValidBrush)
		{
			navBrushComponent = FindNextAINavBrush(targetBrush, targetBrush.transform.position - entryBoundary.transform.position);
			_lastNextNavCheckFrameIdx = Time.frameCount;
		}
		if (navBrushComponent != null)
		{
			SetLocomotionNodes(targetBrush, navBrushComponent);
			if (lastValidBrush.hasPermanentConnections && targetBrush.hasPermanentConnections)
			{
				interpT = 1f;
			}
			else
			{
				interpT = 0f;
			}
		}
		else
		{
			SetLocomotionNodes(targetBrush, entryBoundary);
			interpT = 0f;
		}
	}

	private bool IsBlackingNavPoint(NavBrushComponent brush)
	{
		for (int i = 0; i < blackingNavPoints.Count; i++)
		{
			if (blackingNavPoints[i] == brush)
			{
				return true;
			}
		}
		return false;
	}

	private NavAccessFlags GetBehaviorAccessFlags()
	{
		NavAccessFlags result = NavAccessFlags.Crow;
		switch (behaviorMode)
		{
		case AIBehaviorMode.NoWalkUpWalls:
			result = NavAccessFlags.CrowGrounded;
			break;
		case AIBehaviorMode.WalkUpWalls:
			result = NavAccessFlags.Crow;
			break;
		}
		return result;
	}

	private NavBrushComponent FindNextAINavBrush(NavBrushComponent decisionBrush, Vector3 currentTravelDirection)
	{
		Vector3 up = decisionBrush.transform.up;
		Quaternion quaternion = Quaternion.AngleAxis(-90f, up);
		Quaternion quaternion2 = Quaternion.AngleAxis(90f, up);
		NavBrushLink[] array = new NavBrushLink[5];
		NavBoundaryComponent[] boundaries = decisionBrush.boundaries;
		for (int i = 0; i < 5; i++)
		{
			Vector3 vector = currentTravelDirection;
			vector -= Vector3.Dot(vector, up) * up;
			switch ((TurnDecision)i)
			{
			case TurnDecision.Right:
				vector = quaternion2 * vector;
				break;
			case TurnDecision.Left:
				vector = quaternion * vector;
				break;
			case TurnDecision.Any:
				vector = Quaternion.AngleAxis(180f, up) * vector;
				break;
			case TurnDecision.Backward:
				vector = Quaternion.AngleAxis(180f, up) * vector;
				break;
			}
			vector.Normalize();
			NavBoundaryComponent[] array2 = boundaries;
			foreach (NavBoundaryComponent navBoundaryComponent in array2)
			{
				float num = -2f;
				for (int k = 0; k < navBoundaryComponent.links.Count; k++)
				{
					NavBrushLink navBrushLink = navBoundaryComponent.links[k];
					if ((GetBehaviorAccessFlags() & navBrushLink.flags) == 0)
					{
						continue;
					}
					NavBoundaryComponent otherBoundary = navBrushLink.GetOtherBoundary(navBoundaryComponent);
					if (!(otherBoundary != null) || (i != 3 && !(otherBoundary.parentBrush != lastValidBrush)))
					{
						continue;
					}
					Vector3 vector2 = navBoundaryComponent.transform.position - decisionBrush.transform.position;
					vector2.Normalize();
					float num2 = Vector3.Dot(vector2, vector);
					if (num2 > 0.9f && !IsBlackingNavPoint(otherBoundary.parentBrush))
					{
						bool flag = num2 > num;
						if (!flag)
						{
							Vector3 lhs = otherBoundary.parentBrush.transform.position - otherBoundary.transform.position;
							lhs.Normalize();
							NavBoundaryComponent otherBoundary2 = array[i].GetOtherBoundary(decisionBrush);
							Vector3 lhs2 = otherBoundary2.parentBrush.transform.position - otherBoundary2.transform.position;
							lhs2.Normalize();
							flag = Vector3.Dot(lhs, vector2) > Vector3.Dot(lhs2, vector2);
						}
						if (flag && GameScene.navManager.TestNavBrushesAreStillConnected(decisionBrush, otherBoundary.parentBrush, NavManager.DefaultConnectionTolerance))
						{
							array[i] = navBrushLink;
						}
					}
				}
			}
		}
		if (array[0] != null)
		{
			return array[0].GetOtherBoundary(decisionBrush).parentBrush;
		}
		if (array[1] != null && array[2] == null)
		{
			return array[1].GetOtherBoundary(decisionBrush).parentBrush;
		}
		if (array[2] != null && array[1] == null)
		{
			return array[2].GetOtherBoundary(decisionBrush).parentBrush;
		}
		if (array[3] != null)
		{
			return array[3].GetOtherBoundary(decisionBrush).parentBrush;
		}
		if (array[4] != null)
		{
			return array[4].GetOtherBoundary(decisionBrush).parentBrush;
		}
		return null;
	}

	protected override void UpdateBlockedState()
	{
		if (prevState != LocomotionState.LocoBlocked)
		{
			StartBlockedState();
		}
		if (!animSystem.IsPlaying("ScreamDelta"))
		{
			animSystem.Play("Scream");
		}
		if (lastValidBrush != null)
		{
			Vector3 vector = GetBetweenBrushPosition(interpT);
			if (IsDepthReferenceValid(lastValidBrush))
			{
				vector = CalculateFromDepthReference(vector, lastValidBrush.depthReferenceObject.transform.position);
			}
			base.transform.position = vector;
			base.transform.rotation = GetFaceBrushRotation(playerCharacter.currentBrush, base.currentBrush);
		}
	}

	protected override void StartBlockedState()
	{
		_blockedStartTime = Time.time;
	}

	protected override void UpdateIdleState()
	{
		animSystem.Play("Idle");
		if (lastValidBrush != null)
		{
			Vector3 vector = lastValidBrush.transform.position;
			if (targetBrush != lastValidBrush)
			{
				SetLocomotionNodes(targetBrush, entryBoundary);
			}
			if (IsDepthReferenceValid(lastValidBrush))
			{
				vector = CalculateFromDepthReference(vector, lastValidBrush.depthReferenceObject.transform.position);
			}
			base.transform.position = vector;
			base.transform.rotation = GetBetweenBrushOrientation(0f);
		}
	}

	public override Vector3 getShadowRootPosition()
	{
		if (IdleSitting && ForceIdle)
		{
			return base.transform.position + base.transform.forward * 0.25f;
		}
		if (ForceIdle)
		{
			return base.getShadowRootPosition();
		}
		if (base.locoState == LocomotionState.LocoWalkDownCorner || base.locoState == LocomotionState.LocoWalkUpCorner)
		{
			AnimationState animationState = animSystem["WalkUp"];
			if (base.locoState == LocomotionState.LocoWalkDownCorner)
			{
				animationState = animSystem["WalkDown"];
			}
			if (animationState.normalizedTime < 0.5f)
			{
				return lastValidBrush.transform.position;
			}
			return targetBrush.transform.position;
		}
		if (base.locoState == LocomotionState.LocoEmpty)
		{
			return base.currentBrush.transform.position;
		}
		return base.getShadowRootPosition();
	}

	public override float getShadowIntensity()
	{
		float result = base.getShadowIntensity();
		if (base.locoState == LocomotionState.LocoWalkDownCorner)
		{
			result = Mathf.Abs(animSystem["WalkDown"].normalizedTime - 0.5f) * 2f;
		}
		else if (base.locoState == LocomotionState.LocoWalkUpCorner)
		{
			result = Mathf.Abs(animSystem["WalkUp"].normalizedTime - 0.5f) * 2f;
		}
		return result;
	}

	public override Vector3 getShadowUp()
	{
		if (base.locoState == LocomotionState.LocoWalkDownCorner || base.locoState == LocomotionState.LocoWalkUpCorner || base.locoState == LocomotionState.LocoEmpty)
		{
			AnimationState animationState = animSystem["WalkUp"];
			if (base.locoState == LocomotionState.LocoWalkDownCorner)
			{
				animationState = animSystem["WalkDown"];
			}
			if (animationState == null || animationState.normalizedTime < 0.5f)
			{
				return lastValidBrush.transform.TransformDirection(lastValidBrush.normal);
			}
			return targetBrush.transform.TransformDirection(targetBrush.normal);
		}
		return base.characterUp;
	}

	[TriggerableAction]
	public IEnumerator SetCrowIdle()
	{
		Stop();
		ForceIdle = true;
		return null;
	}

	[TriggerableAction]
	public IEnumerator StopSitting()
	{
		Stop();
		IdleSitting = false;
		return null;
	}

	[TriggerableAction]
	public IEnumerator SetCrowWalking()
	{
		ForceIdle = false;
		_lastNextNavCheckFrameIdx = Time.frameCount;
		return null;
	}

	[TriggerableAction]
	public IEnumerator StartLookingAtObject()
	{
		lookAtObject = true;
		return null;
	}

	[TriggerableAction]
	public IEnumerator StopLookingAtObject()
	{
		lookAtObject = false;
		return null;
	}

	[TriggerableAction]
	public IEnumerator StartAmazedEyes()
	{
		textureAnimState = TextureAnimState.Amazed;
		return null;
	}

	[TriggerableAction]
	public IEnumerator StopAmazedEyes()
	{
		textureAnimState = TextureAnimState.Blink;
		return null;
	}

	protected override bool IsBlocked()
	{
		if (ForceIdle)
		{
			return false;
		}
		if (_blockedStartTime != 0f && Time.time - _blockedStartTime < minBlockedTime)
		{
			return true;
		}
		if (targetBrush != lastValidBrush)
		{
			bool flag = lastValidBrush == playerCharacter.lastValidBrush || lastValidBrush == playerCharacter.targetBrush;
			if (GameScene.navManager.TestNavBrushesAreStillConnected(targetBrush, lastValidBrush, NavManager.DefaultConnectionTolerance))
			{
				flag |= targetBrush == playerCharacter.lastValidBrush || targetBrush == playerCharacter.targetBrush;
			}
			if (flag)
			{
				if (playerCharacter.targetBrush == lastValidBrush && base.IsWalkingAroundCorner)
				{
					return false;
				}
				RenderDebug.DrawLine(base.transform.position + base.transform.up, playerCharacter.transform.position, Color.red);
				return true;
			}
		}
		return false;
	}

	protected override void EndBlockedState()
	{
	}

	private void UpdateWhileOffScreen()
	{
		base.transform.position = GetBetweenBrushPosition(interpT);
		float num = interpT;
		if (!exitBoundary && num != 0f)
		{
			num = 0f;
		}
		base.transform.rotation = GetBetweenBrushOrientation(num);
	}
}
