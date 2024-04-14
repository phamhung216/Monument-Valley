using System;
using UnityEngine;

public class FloatPlatform : MonoBehaviour
{
	private enum State
	{
		Normal = 0,
		Dropping = 1,
		Dropped = 2
	}

	public Vector3 deltaPosition;

	public float duration = 2f;

	public bool startWithRandomOffset;

	public NavBrushComponent navBrush;

	public BaseLocomotion character;

	public float dropAmount;

	public float dropBobAmount;

	public float dropLerpSpeed1;

	public float dropLerpSpeed2;

	private float _time;

	private Vector3 _startPosition;

	private float _dropParam;

	private State _state;

	private void Start()
	{
		_startPosition = base.transform.localPosition;
		if (startWithRandomOffset)
		{
			_time = UnityEngine.Random.Range(0f, duration);
		}
	}

	private void Update()
	{
		if (character.currentBrush == navBrush)
		{
			_dropParam = Mathf.Lerp(_dropParam, 1f, dropLerpSpeed1);
			if (_state == State.Normal)
			{
				_state = State.Dropping;
				_time %= duration;
				if (_time > 0.5f * duration)
				{
					_time = duration - _time;
				}
			}
			if (_state == State.Dropping)
			{
				_time = Mathf.MoveTowards(_time, 0f, dropLerpSpeed2);
				if (_time <= 0f)
				{
					_time = 0f;
					_state = State.Dropped;
				}
			}
		}
		else
		{
			_dropParam = Mathf.Lerp(_dropParam, 0f, dropLerpSpeed1);
			_state = State.Normal;
		}
		float num = 0f - Mathf.Cos((float)Math.PI * 2f * (_time / duration));
		num *= Mathf.Lerp(1f, dropBobAmount, _dropParam);
		num -= _dropParam * (dropAmount + (1f - dropBobAmount));
		base.transform.localPosition = _startPosition + 0.5f * deltaPosition * num;
		if (_state != State.Dropping)
		{
			_time += Time.deltaTime;
		}
	}
}
