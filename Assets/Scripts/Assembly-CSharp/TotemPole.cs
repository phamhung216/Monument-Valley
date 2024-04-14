using System.Collections;
using System.Collections.Generic;
using Fabric;
using UnityEngine;

public class TotemPole : BaseLocomotion
{
	public AnimationCurveDefinition MoveCurve;

	public ParticleSystem moveParticles;

	public Transform headBone;

	public TotemEffectZone effectZone;

	public Renderer totemRenderer;

	public bool dragging;

	public string movementOneShotEvent;

	public bool isCube;

	public static float TotemConnectionTolerance = NavManager.DefaultConnectionTolerance;

	private BaseLocomotion[] _allAIObjects;

	private Vector3 _targetPanPos = new Vector3(0f, 0f, 0f);

	private NavBrushComponent[] _surfaceNav;

	private CharacterLocomotion _mainPlayer;

	private float _headRotAmount;

	private bool _headRotComplete = true;

	private Quaternion _localOrientation;

	private bool _updatedThisFrame;

	private static TotemPole[] _allTotems;

	private List<AnimationState> _availableAnimations;

	private MoverAudio _moverAudio;

	public Transform[] eyes;

	private float _animTime;

	private bool lastFrameSpecial;

	private BaseLocomotion _blocker;

	public BaseLocomotion blocker => _blocker;

	private void Start()
	{
		Vector2 vector = GameScene.WorldToPanPoint(base.transform.position);
		NavBrushComponent navBrushComponent = GameScene.navManager.FindNavBrushBelowPanPoint(vector, touchableOnly: false);
		if ((bool)navBrushComponent)
		{
			_localOrientation = Quaternion.Inverse(navBrushComponent.transform.rotation) * base.transform.rotation;
			SetLocomotionNodes(navBrushComponent, base.transform.forward);
		}
		else
		{
			_localOrientation = Quaternion.identity;
		}
		_allAIObjects = Object.FindObjectsOfType(typeof(BaseLocomotion)) as BaseLocomotion[];
		navAccess = NavAccessFlags.Totem | NavAccessFlags.NotBlocked;
		_surfaceNav = GetComponentsInChildren<NavBrushComponent>();
		GameScene.instance.EnsurePlayer();
		_mainPlayer = GameScene.player.GetComponent<CharacterLocomotion>();
		Stop();
		_moverAudio = GetComponent<MoverAudio>();
		if (_moverAudio != null)
		{
			_moverAudio.motionType = MoverAudio.MotionType.Totem;
			_moverAudio.audioEvent = "Totem/Drag";
		}
		if (_allTotems == null || _allTotems.Length == 0)
		{
			_allTotems = Object.FindObjectsOfType<TotemPole>();
		}
		if (animSystem != null)
		{
			_availableAnimations = new List<AnimationState>();
			if (animSystem["IdleNoRotation"] != null)
			{
				_availableAnimations.Add(animSystem["IdleNoRotation"]);
			}
			if (animSystem["IdleRotation"] != null)
			{
				_availableAnimations.Add(animSystem["IdleRotation"]);
			}
			if (animSystem["HeadBounce"] != null)
			{
				_availableAnimations.Add(animSystem["HeadBounce"]);
			}
			if (animSystem["BodyBounce"] != null)
			{
				_availableAnimations.Add(animSystem["BodyBounce"]);
			}
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
		base.Stop();
		dragging = false;
		_targetPanPos = GameScene.WorldToPanPoint(base.transform.position);
		SetLocomotionNodes(lastValidBrush, base.transform.rotation);
	}

	private void Update()
	{
		_updatedThisFrame = false;
	}

	private void LateUpdate()
	{
		if (!_updatedThisFrame)
		{
			UpdateTotem();
		}
		if (base.locoState == LocomotionState.LocoWalk)
		{
			SearchTotem(this);
		}
	}

	private static void SearchTotem(TotemPole parentTotem)
	{
		for (int i = 0; i < _allTotems.Length; i++)
		{
			TotemPole totemPole = _allTotems[i];
			if (!(totemPole != parentTotem))
			{
				continue;
			}
			for (int j = 0; j < parentTotem._surfaceNav.Length; j++)
			{
				if (parentTotem._surfaceNav[j] == totemPole.lastValidBrush)
				{
					totemPole.UpdateTotem();
					SearchTotem(totemPole);
				}
			}
		}
	}

	private void UpdateTotem()
	{
		_updatedThisFrame = true;
		base.locoState = SelectLocoState(lastValidBrush, targetBrush);
		UpdateStates();
		if (!(headBone != null))
		{
			return;
		}
		Quaternion b = Quaternion.Euler(0f, 270f, 270f);
		if (effectZone != null && (effectZone.forceHeadRight || effectZone.forceHeadLeft))
		{
			if (effectZone.forceHeadLeft)
			{
				b = Quaternion.Euler(0f, 90f, 270f);
			}
			if (_headRotAmount < 1f)
			{
				_headRotAmount += 0.1f;
			}
			else
			{
				_headRotAmount = 1f;
			}
		}
		else if (_headRotAmount > 0f)
		{
			_headRotAmount -= 0.1f;
		}
		else
		{
			_headRotAmount = 0f;
		}
		if (_headRotAmount > 0f || !_headRotComplete)
		{
			if (_headRotAmount <= 0f)
			{
				_headRotComplete = true;
			}
			else
			{
				_headRotComplete = false;
			}
			Quaternion a = Quaternion.Euler(0f, 180f, 270f);
			headBone.transform.rotation = Quaternion.Slerp(a, b, _headRotAmount);
		}
	}

	protected void SetLocomotionNodes(NavBrushComponent brush, Quaternion rotation)
	{
		if ((bool)lastValidBrush)
		{
			_localOrientation = Quaternion.Inverse(brush.transform.rotation) * rotation;
		}
		base.transform.rotation = rotation;
		base.SetLocomotionNodes(brush, base.transform.forward);
	}

	protected override void SetLocomotionNodes(NavBrushComponent brush, Vector3 direction)
	{
		if ((bool)lastValidBrush)
		{
			_localOrientation = Quaternion.Inverse(brush.transform.rotation) * lastValidBrush.transform.rotation * _localOrientation;
		}
		base.SetLocomotionNodes(brush, direction);
	}

	protected override void SetLocomotionNodes(NavBrushComponent from, NavBrushComponent to)
	{
		if ((bool)lastValidBrush)
		{
			_localOrientation = Quaternion.Inverse(from.transform.rotation) * lastValidBrush.transform.rotation * _localOrientation;
		}
		base.SetLocomotionNodes(from, to);
	}

	private bool IsPlayerWalkingOnMe()
	{
		if (!_mainPlayer)
		{
			_mainPlayer = GameScene.player.GetComponent<CharacterLocomotion>();
		}
		for (int i = 0; i < _surfaceNav.Length; i++)
		{
			if (_mainPlayer.lastValidBrush == _surfaceNav[i])
			{
				return true;
			}
		}
		return false;
	}

	private bool IsHeadConnectedToNav()
	{
		return false;
	}

	private BaseLocomotion GetBrushBlocker(NavBrushComponent target)
	{
		for (int i = 0; i < _allAIObjects.Length; i++)
		{
			if (!(_allAIObjects[i] == this) && (_allAIObjects[i].lastValidBrush == target || _allAIObjects[i].targetBrush == target))
			{
				return _allAIObjects[i];
			}
		}
		return null;
	}

	private void UpdateBlink()
	{
		if (Random.Range(0, 500) != 1)
		{
			return;
		}
		AnimationState animationState = animSystem["Blink"];
		animationState.layer = 1;
		for (int i = 0; i < eyes.Length; i++)
		{
			animationState.AddMixingTransform(eyes[i]);
		}
		bool flag = false;
		foreach (AnimationState item in animSystem)
		{
			if (animSystem.IsPlaying(item.name))
			{
				flag = true;
				break;
			}
		}
		if (!flag)
		{
			animSystem.Play("Blink", PlayMode.StopSameLayer);
		}
	}

	protected override void UpdateIdleState()
	{
		interpT = 0f;
		if (animSystem != null)
		{
			UpdateBlink();
			if (IsPlayerWalkingOnMe())
			{
				animSystem.Play("IdleNoRotation");
			}
			else
			{
				int num = 0;
				_animTime += Time.deltaTime;
				bool flag = true;
				bool flag2 = true;
				if (effectZone != null)
				{
					flag2 = effectZone.allowHeadRotation;
					flag = effectZone.allowBounce;
				}
				bool flag3 = false;
				for (int i = 0; i < _availableAnimations.Count; i++)
				{
					flag3 |= _availableAnimations[i].enabled;
				}
				foreach (AnimationState item in animSystem)
				{
					if (animSystem.IsPlaying(item.name))
					{
						flag3 = false;
						break;
					}
				}
				if (_animTime > 5f && !flag3)
				{
					_animTime = 0f;
					if (lastFrameSpecial)
					{
						lastFrameSpecial = false;
						animSystem.Play("T-Pose");
					}
					else
					{
						switch (Random.Range(0, 10))
						{
						case 0:
							if ((bool)animSystem["IdleNoRotation"])
							{
								animSystem.Play("IdleNoRotation");
								lastFrameSpecial = true;
							}
							break;
						case 1:
							if (flag2 && (bool)animSystem["IdleRotation"])
							{
								animSystem.Play("IdleRotation");
								lastFrameSpecial = true;
							}
							break;
						case 2:
							if (flag && (bool)animSystem["HeadBounce"])
							{
								animSystem.Play("HeadBounce");
								lastFrameSpecial = true;
							}
							break;
						case 3:
							if (flag && (bool)animSystem["BodyBounce"])
							{
								animSystem.Play("BodyBounce");
								lastFrameSpecial = true;
							}
							break;
						case 4:
							if ((bool)animSystem["IdleNoRotation"])
							{
								animSystem.Play("IdleNoRotation");
								lastFrameSpecial = true;
							}
							break;
						default:
							animSystem.Play("T-Pose");
							break;
						}
					}
				}
			}
		}
		if (lastValidBrush != null)
		{
			Vector3 vector = lastValidBrush.transform.position;
			AttemptSetTargetBrush();
			if (IsDepthReferenceValid(lastValidBrush))
			{
				vector = CalculateFromDepthReference(vector, lastValidBrush.depthReferenceObject.transform.position);
			}
			base.transform.position = vector;
			base.transform.rotation = lastValidBrush.transform.rotation * _localOrientation;
			Quaternion quaternion = Quaternion.FromToRotation(base.transform.up, lastValidBrush.transform.up);
			base.transform.rotation = quaternion * base.transform.rotation;
		}
	}

	protected override void UpdateWalkState()
	{
		if (interpT > 1f)
		{
			interpT -= 1f;
			IncrementNodes();
			moveParticles.Emit(3);
			if ((bool)EventManager.Instance && movementOneShotEvent != null && movementOneShotEvent.Length > 0)
			{
				EventManager.Instance.PostEvent(movementOneShotEvent, EventAction.PlaySound);
			}
		}
		(lastValidBrush.transform.position - targetBrush.transform.position).Normalize();
		Vector3 vector = (lastValidBrush.transform.position + targetBrush.transform.position) * 0.5f;
		if ((bool)lastValidBrush.GetConnectionBoundary(targetBrush))
		{
			vector = lastValidBrush.GetConnectionBoundary(targetBrush).transform.position;
		}
		Vector3 vector2 = lastValidBrush.transform.position;
		if (IsDepthReferenceValid(lastValidBrush))
		{
			vector2 = CalculateFromDepthReference(vector2, lastValidBrush.depthReferenceObject.transform.position);
			vector = CalculateFromDepthReference(vector, lastValidBrush.depthReferenceObject.transform.position);
		}
		Vector3 vector3 = targetBrush.transform.position;
		if (IsDepthReferenceValid(targetBrush))
		{
			vector3 = CalculateFromDepthReference(vector3, targetBrush.depthReferenceObject.transform.position);
			vector = CalculateFromDepthReference(vector, targetBrush.depthReferenceObject.transform.position);
		}
		_ = lastValidBrush;
		_ = targetBrush;
		if (interpT < 1f)
		{
			float num = ((MoveCurve != null) ? MoveCurve.curve.Evaluate(interpT) : interpT);
			Vector3 vector4 = vector2 + (vector3 - vector2) * num;
			vector4 = ((!(num < 0.5f)) ? (vector + (vector3 - vector) * ((num - 0.5f) * 2f)) : (vector2 + (vector - vector2) * (num * 2f)));
			vector4 = CalculateMagicPlayerProjection(vector4, vector2, vector3, base.characterUp);
			base.transform.position = vector4;
			if (num < 0.5f && !GameScene.navManager.TestNavBrushesAreStillConnected(lastValidBrush, targetBrush, TotemConnectionTolerance))
			{
				Stop();
			}
			if (_moverAudio != null)
			{
				_moverAudio.NotifyMove();
			}
		}
		base.transform.rotation = lastValidBrush.transform.rotation * _localOrientation;
		Quaternion quaternion = Quaternion.FromToRotation(base.transform.up, lastValidBrush.transform.up);
		base.transform.rotation = quaternion * base.transform.rotation;
		interpT += Time.deltaTime * characterVel;
	}

	protected override void IncrementNodes()
	{
		NavBrushComponent navBrushComponent = lastValidBrush;
		if (lastValidBrush != targetBrush && !GameScene.navManager.TestNavBrushesAreStillConnected(lastValidBrush, targetBrush, TotemConnectionTolerance))
		{
			SetLocomotionNodes(lastValidBrush, base.transform.forward);
			base.locoState = getCorrectIdleState(lastValidBrush);
		}
		if (lastValidBrush != targetBrush)
		{
			SetLocomotionNodes(targetBrush, base.transform.forward);
		}
		AttemptSetTargetBrush();
		if (lastValidBrush != navBrushComponent && lastValidBrush == targetBrush)
		{
			GameScene.navManager.NotifyReconfigurationBegan(base.gameObject);
			GameScene.navManager.NotifyReconfigurationEnded();
		}
	}

	private void AttemptSetTargetBrush()
	{
		if (dragging)
		{
			NavBrushComponent bestBrush = null;
			BaseLocomotion moveBlocker = null;
			GetBestNeighbourForDestination(lastValidBrush, _targetPanPos, out bestBrush, out moveBlocker);
			_blocker = moveBlocker;
			SetLocomotionNodes(lastValidBrush, bestBrush);
			RenderDebug.DrawSphere(targetBrush.transform.position, Color.red, 0.2f);
		}
	}

	private Vector3 ProjectPanSpacePosToTotemMovePlane(Vector3 panSpacePos)
	{
		Ray ray = GameScene.PanToWorldRay(panSpacePos);
		new Plane(lastValidBrush.transform.up, lastValidBrush.transform.position).Raycast(ray, out var enter);
		return ray.GetPoint(enter);
	}

	private void GetBestNeighbourForDestination(NavBrushComponent currentBrush, Vector3 targetPanPos, out NavBrushComponent bestBrush, out BaseLocomotion moveBlocker)
	{
		NavBoundaryComponent[] boundaries = currentBrush.boundaries;
		Vector3 b = ProjectPanSpacePosToTotemMovePlane(targetPanPos);
		float num = Vector3.Distance(ProjectPanSpacePosToTotemMovePlane(currentBrush.panSpacePos), b);
		bestBrush = currentBrush;
		moveBlocker = null;
		NavBoundaryComponent[] array = boundaries;
		foreach (NavBoundaryComponent navBoundaryComponent in array)
		{
			for (int j = 0; j < navBoundaryComponent.links.Count; j++)
			{
				NavBrushLink navBrushLink = navBoundaryComponent.links[j];
				NavBoundaryComponent otherBoundary = navBrushLink.GetOtherBoundary(navBoundaryComponent);
				if ((NavAccessFlags.Totem & navBrushLink.flags) == 0 || !GameScene.navManager.TestNavBrushesAreStillConnected(navBoundaryComponent.parentBrush, otherBoundary.parentBrush, TotemConnectionTolerance) || !(otherBoundary != null) || (otherBoundary.transform.parent.GetComponent<NavBrushComponent>().type != 0 && otherBoundary.transform.parent.GetComponent<NavBrushComponent>().type != NavBrushComponent.Type.Hole))
				{
					continue;
				}
				float num2 = Vector3.Distance(ProjectPanSpacePosToTotemMovePlane(otherBoundary.parentBrush.panSpacePos), b);
				if (num2 < num && GameScene.IsWorldPointOnScreen(otherBoundary.parentBrush.transform.position, Camera.main, 1f))
				{
					BaseLocomotion brushBlocker = GetBrushBlocker(otherBoundary.transform.parent.GetComponent<NavBrushComponent>());
					if (!brushBlocker)
					{
						num = num2;
						bestBrush = otherBoundary.parentBrush;
					}
					else
					{
						moveBlocker = brushBlocker;
					}
				}
			}
		}
	}

	public void SetTargetPanSpacePos(Vector3 pos)
	{
		_targetPanPos = pos;
	}

	public void Teleport(NavBrushComponent destinationBrush, Quaternion rotation)
	{
		GameScene.navManager.NotifyReconfigurationBegan(base.gameObject);
		base.transform.position = destinationBrush.transform.position;
		SetLocomotionNodes(destinationBrush, rotation);
		GameScene.navManager.NotifyReconfigurationEnded();
	}

	[TriggerableAction]
	public IEnumerator EnableRenderer()
	{
		totemRenderer.enabled = true;
		return null;
	}

	[TriggerableAction]
	public IEnumerator DisableRenderer()
	{
		totemRenderer.enabled = false;
		return null;
	}

	[TriggerableAction]
	public IEnumerator ReduceConnectionTolerance()
	{
		TotemConnectionTolerance = 0.05f;
		return null;
	}

	[TriggerableAction]
	public IEnumerator ResetConnectionTolerance()
	{
		TotemConnectionTolerance = NavManager.DefaultConnectionTolerance;
		return null;
	}

	protected override void UpdateBlockedState()
	{
		UpdateIdleState();
	}

	public bool IsHigherThanCube()
	{
		if (!isCube)
		{
			return true;
		}
		for (int i = 0; i < _surfaceNav.Length; i++)
		{
			for (int j = 0; j < _allTotems.Length; j++)
			{
				TotemPole totemPole = _allTotems[j];
				if (totemPole != this && totemPole.lastValidBrush == _surfaceNav[i])
				{
					return true;
				}
			}
		}
		return false;
	}

	public bool CanMoveInPanSpaceDirection(Vector3 panSpaceDirection)
	{
		Vector3 targetPanPos = lastValidBrush.panSpacePos + 2f * panSpaceDirection.normalized;
		GetBestNeighbourForDestination(lastValidBrush, targetPanPos, out var bestBrush, out var _);
		return bestBrush != lastValidBrush;
	}
}
