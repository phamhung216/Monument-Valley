using System;
using System.Collections;
using UnityEngine;

public class Storyteller : MonoBehaviour
{
	public TriggerableActionSequence afterFirstTalked;

	public TriggerableActionSequence afterTalked;

	public AutoInterp speechBubbleInterp;

	public AutoInterp fadeoutInterp;

	public ParticleSystem storytellerParticles;

	public Transform speechBubble;

	public ProximityTrigger trigger;

	private Vector3 speechBubbleRotation = new Vector3(-35.26439f, 225f, 0f);

	private bool _triggered;

	public Transform model;

	public float lookAtAngleLimit;

	public bool lookInRealSpace;

	private Vector3 _modelStartDir;

	private void Start()
	{
		speechBubble.eulerAngles = speechBubbleRotation;
		_modelStartDir = base.transform.parent.InverseTransformDirection(-model.up);
	}

	[TriggerableAction]
	public IEnumerator FadeOutStoryteller()
	{
		storytellerParticles.Stop();
		trigger.DisableTrigger();
		fadeoutInterp.ReverseInterp();
		if (!_triggered)
		{
			speechBubbleInterp.ReverseInterp();
		}
		return null;
	}

	[TriggerableAction]
	public IEnumerator TalkStarted()
	{
		GameScene.instance.GetComponent<SceneAudio>().BeginStorytellerAmbience();
		if (!_triggered)
		{
			speechBubbleInterp.ReverseInterp();
		}
		return null;
	}

	[TriggerableAction(true)]
	public IEnumerator TalkEnded()
	{
		GameScene.instance.GetComponent<SceneAudio>().EndStorytellerAmbience();
		afterTalked.TriggerActions();
		if (!_triggered)
		{
			_triggered = true;
			if ((bool)afterFirstTalked)
			{
				afterFirstTalked.TriggerActions();
			}
		}
		yield return null;
	}

	private void Update()
	{
		UpdateHeadLook();
	}

	private void UpdateHeadLook()
	{
		Vector3 vector = model.transform.position + base.transform.up * 1.5f;
		Vector3 position = GameScene.player.GetComponent<CharacterLocomotion>().HeadBone.position;
		Vector3 vector2;
		if (lookInRealSpace)
		{
			vector2 = position - vector;
		}
		else
		{
			Vector3 forward = Camera.main.transform.forward;
			Vector3 b = position - (position.y - vector.y) / forward.y * forward;
			Vector3 vector3 = position;
			vector3.y = vector.y;
			float t = Mathf.Clamp(0.125f * (vector3 - vector).sqrMagnitude, 0f, 1f);
			Vector3 vector4 = Vector3.Lerp(vector3, b, t);
			vector2 = vector4 - vector;
			vector2.Normalize();
			RenderDebug.DrawLine(vector, vector4, Color.red);
		}
		Vector3 up = Vector3.up;
		Vector3 target = vector2 - Vector3.Dot(vector2, up) * up;
		target.Normalize();
		Quaternion b2 = Quaternion.LookRotation(Vector3.RotateTowards(base.transform.parent.TransformDirection(_modelStartDir), target, lookAtAngleLimit * ((float)Math.PI / 180f), 0f), up) * Quaternion.Euler(270f, 0f, 0f);
		model.transform.rotation = Quaternion.Slerp(model.transform.rotation, b2, 0.2f);
	}
}
