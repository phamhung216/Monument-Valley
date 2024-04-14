using System;
using System.Collections;
using UnityCommon;
using UnityEngine;

public class CameraShaker : MonoBehaviour
{
	private float _shaketimer = 100f;

	public float maxShakeTime = 0.5f;

	public float noiseShakeMagnitude = 3f;

	public float shakeFrequency = 11.111111f;

	public float shakeDecay = 2f;

	private bool _allowProceduralShake = true;

	public bool forceShake;

	public AnimationClip stunAnimation;

	private Animation _animation;

	public EditorButton testShakeToggle = new EditorButton("DebugToggleShake", runtimeOnly: true);

	public EditorButton testShake = new EditorButton("DebugActivateShake", runtimeOnly: true);

	private void Update()
	{
		if (_allowProceduralShake)
		{
			UpdateShake(Time.deltaTime);
		}
	}

	public void DebugToggleShake()
	{
		if (forceShake)
		{
			EndShake();
		}
		else
		{
			StartShake();
		}
	}

	public void DebugActivateShake()
	{
		ActivateShake();
	}

	[TriggerableAction]
	public IEnumerator ActivateShake()
	{
		if (TriggerAction.FastForward)
		{
			return null;
		}
		_shaketimer = 0f;
		return null;
	}

	[TriggerableAction]
	public IEnumerator StunShake()
	{
		if (TriggerAction.FastForward)
		{
			return null;
		}
		_allowProceduralShake = false;
		if (_animation == null)
		{
			_animation = GetComponent<Animation>();
		}
		_animation.clip = stunAnimation;
		_animation.Play();
		return null;
	}

	public void EnableProceduralShake()
	{
		_allowProceduralShake = true;
	}

	[TriggerableAction]
	public IEnumerator StartShake()
	{
		if (TriggerAction.FastForward)
		{
			return null;
		}
		_shaketimer = 0f;
		forceShake = true;
		return null;
	}

	[TriggerableAction]
	public IEnumerator EndShake()
	{
		forceShake = false;
		_shaketimer = maxShakeTime;
		return null;
	}

	private void UpdateShake(float timeStep)
	{
		_shaketimer += timeStep;
		float num = 0f;
		num = ((!(_shaketimer < maxShakeTime) && !forceShake) ? Mathf.SmoothStep(1f, 0f, (_shaketimer - maxShakeTime) / shakeDecay) : 1f);
		float x = num * Mathf.Sin(_shaketimer * shakeFrequency * 2f * (float)Math.PI);
		Vector3 localPosition = noiseShakeMagnitude * new Vector3(x, 0f, 0f);
		base.transform.localPosition = localPosition;
	}
}
