using System;
using System.Collections;
using UnityEngine;

public class ProximityTrigger : TriggerItem
{
	public NavBrushComponent targetBrush;

	public Transform[] testObjects;

	public ActionSequence actions = new ActionSequence();

	public ActionSequence exitActions = new ActionSequence();

	public Animation triggerAnimation;

	public AnimationClip onTriggerAnimationClip;

	public AnimationClip onExitAnimationClip;

	public bool isMultiTrigger;

	public bool haltPlayer;

	public bool disableNavOnExit;

	private bool _triggered;

	private Collider _collider;

	private bool _exitTriggered;

	private static string s_iconTexture = "GizmoTrigger";

	public bool triggered => _triggered;

	protected void Start()
	{
		_collider = GetComponent<Collider>();
		if (targetBrush == null && !_collider)
		{
			D.Error("Trigger has no target brush or collider to test against.", this);
		}
	}

	private bool TestObjectForTrigger(Transform test)
	{
		BaseLocomotion component = test.GetComponent<BaseLocomotion>();
		if (targetBrush != null && component != null)
		{
			return component.lastValidBrush == targetBrush;
		}
		if (_collider != null)
		{
			return _collider.bounds.Contains(test.position);
		}
		return false;
	}

	private void DirectTrigger()
	{
		if (haltPlayer && (bool)targetBrush)
		{
			for (int i = 0; i < testObjects.Length; i++)
			{
				PlayerInput component = testObjects[i].GetComponent<PlayerInput>();
				if ((bool)component)
				{
					component.CancelMove();
					component.StartMoveInputCooldown(0.5f);
					continue;
				}
				TotemPoleInput component2 = testObjects[i].GetComponent<TotemPoleInput>();
				if ((bool)component2)
				{
					component2.CancelDrag();
				}
			}
		}
		Trigger();
	}

	protected void Update()
	{
		if (testObjects == null || testObjects.Length == 0)
		{
			testObjects = new Transform[1] { GameScene.player.transform };
		}
		bool flag = false;
		for (int i = 0; i < testObjects.Length; i++)
		{
			if (TestObjectForTrigger(testObjects[i]))
			{
				if (!_triggered)
				{
					DirectTrigger();
				}
				flag = true;
			}
		}
		if (_triggered && !flag && !_exitTriggered)
		{
			ExitTrigger();
		}
		if (isMultiTrigger && _triggered && !flag)
		{
			UnTrigger();
		}
	}

	protected void UnTrigger()
	{
		_triggered = false;
	}

	[TriggerableAction]
	public IEnumerator DoTrigger()
	{
		if (isMultiTrigger || !_triggered)
		{
			Trigger();
		}
		return null;
	}

	[TriggerableAction]
	public IEnumerator ResetButton()
	{
		ResetButtonVisual();
		_triggered = false;
		return null;
	}

	[TriggerableAction]
	public IEnumerator DisableTrigger()
	{
		isMultiTrigger = false;
		_triggered = true;
		_exitTriggered = true;
		return null;
	}

	[TriggerableAction]
	public IEnumerator MuteTriggerAudio()
	{
		GetComponentInChildren<AnimationAudioEventController>().enabled = false;
		return null;
	}

	[TriggerableAction]
	public IEnumerator UnMuteTriggerAudio()
	{
		GetComponentInChildren<AnimationAudioEventController>().enabled = true;
		return null;
	}

	[TriggerableAction]
	public IEnumerator SetVisuallyTriggered()
	{
		if ((bool)triggerAnimation && (bool)onTriggerAnimationClip)
		{
			triggerAnimation.Play(onTriggerAnimationClip.name);
			triggerAnimation[onTriggerAnimationClip.name].normalizedTime = 1f;
			triggerAnimation.Sample();
			PressureSwitchMatSwap componentInChildren = base.transform.GetComponentInChildren<PressureSwitchMatSwap>();
			if ((bool)componentInChildren)
			{
				componentInChildren.SwapMatToAlternate();
			}
		}
		return null;
	}

	public override void Trigger()
	{
		if ((bool)triggerAnimation && (bool)onTriggerAnimationClip && (!_triggered || isMultiTrigger))
		{
			triggerAnimation[onTriggerAnimationClip.name].time = 0f;
			triggerAnimation[onTriggerAnimationClip.name].speed = 1f;
			triggerAnimation.Play(onTriggerAnimationClip.name);
		}
		_triggered = true;
		_exitTriggered = false;
		PlaySound();
		AnalyticsTrigger component = GetComponent<AnalyticsTrigger>();
		if (null != component)
		{
			component.SendAnalyticsEvent();
		}
		try
		{
			StartCoroutine(actions.DoSequence());
		}
		catch (Exception ex)
		{
			D.Error("Trigger " + base.name + " Coroutine threw exception " + ex, base.gameObject);
		}
	}

	private void ResetButtonVisual()
	{
		if ((bool)onExitAnimationClip && (bool)triggerAnimation)
		{
			triggerAnimation[onExitAnimationClip.name].time = 0f;
			triggerAnimation.Play(onExitAnimationClip.name);
		}
	}

	public void ExitTrigger()
	{
		if (isMultiTrigger && (bool)triggerAnimation)
		{
			ResetButtonVisual();
		}
		if (disableNavOnExit && (bool)targetBrush)
		{
			targetBrush.touchable = false;
			targetBrush.discarded = true;
		}
		_exitTriggered = true;
		StartCoroutine(exitActions.DoSequence());
	}

	public virtual void PlaySound()
	{
	}

	private void OnDrawGizmos()
	{
		Gizmos.DrawIcon(base.transform.position, s_iconTexture);
		if ((bool)targetBrush)
		{
			Gizmos.color = Color.blue;
			Gizmos.DrawLine(base.transform.position, targetBrush.transform.position);
			if (disableNavOnExit)
			{
				Gizmos.color = Color.red;
				Gizmos.DrawWireSphere(base.transform.position, 0.5f);
			}
		}
	}
}
