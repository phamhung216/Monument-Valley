using System;
using UnityEngine;

[Serializable]
public class NotchedSlider
{
	public enum NotchType
	{
		Bounce = 0,
		Magnet = 1
	}

	public float pos;

	public float vel;

	public bool wrapping;

	public float origin;

	public float min;

	public float max;

	public float notchSeparation = 1f;

	public NotchType notchType = NotchType.Magnet;

	private float _dragLinear;

	private float _notchSpringConstLinear;

	private float _notchMaxSpeed;

	private float _snapMaxSpeed;

	[Range(0f, 1f)]
	private float edgeElasticity;

	private bool _stationary = true;

	private bool _isCaptured;

	private float _bounceDir;

	private int _captureNotchIdx;

	private int _minNotchIdx;

	private int _maxNotchIdx;

	private float timeRemainder;

	public bool stationary => _stationary;

	public void DisableEdgeBounce()
	{
		edgeElasticity = 0f;
	}

	public NotchedSlider(bool wrapping, float min, float max, float notchSeparation)
	{
		if (notchSeparation >= 90f)
		{
			this.wrapping = wrapping;
			this.min = min;
			this.max = max;
			this.notchSeparation = notchSeparation;
			_notchMaxSpeed = 360f;
			_dragLinear = 1f;
			_notchSpringConstLinear = 360f;
			_snapMaxSpeed = 35f;
			edgeElasticity = 0.3f;
		}
		else
		{
			this.wrapping = wrapping;
			this.min = min;
			this.max = max;
			this.notchSeparation = notchSeparation;
			_notchMaxSpeed = 5f;
			_dragLinear = 5f;
			_notchSpringConstLinear = 20f;
			_snapMaxSpeed = 1.5f;
			edgeElasticity = 0.3f;
		}
	}

	public void Start()
	{
		_stationary = false;
		_isCaptured = false;
		_captureNotchIdx = -1;
		_minNotchIdx = Mathf.CeilToInt(GetPosParam(min));
		_maxNotchIdx = Mathf.FloorToInt(GetPosParam(max));
		if (Mathf.Abs(vel) < _notchMaxSpeed)
		{
			int nearestNotch = GetNearestNotch(pos);
			CaptureWith(nearestNotch);
			_bounceDir = Mathf.Sign(NotchIdxToPos(nearestNotch) - pos);
		}
	}

	public float Advance(Transform parent, Vector3 dir)
	{
		float num = 1f / 120f;
		timeRemainder += Time.deltaTime;
		while (timeRemainder > num)
		{
			AdvanceFixed(num, parent, dir);
			timeRemainder -= num;
		}
		return pos;
	}

	private void AdvanceFixed(float timeStep, Transform parent, Vector3 dir)
	{
		int nearestNotch = GetNearestNotch(pos);
		int num = SelectNotch(pos, vel);
		if (Mathf.Abs(vel) < _notchMaxSpeed)
		{
			switch (notchType)
			{
			case NotchType.Bounce:
				if (!_isCaptured)
				{
					CaptureWith(num);
					if (NotchIdxToPos(num) == pos)
					{
						_bounceDir = Mathf.Sign(vel);
					}
					else
					{
						_bounceDir = Mathf.Sign((float)num - pos);
					}
				}
				break;
			case NotchType.Magnet:
				CaptureWith(nearestNotch);
				_bounceDir = Mathf.Sign(NotchIdxToPos(nearestNotch) - pos);
				break;
			}
		}
		float num2 = vel;
		float num3 = pos;
		float notchForce = GetNotchForce(pos - NotchIdxToPos(num), vel);
		float num4 = num2 + notchForce * timeStep;
		float num5 = num4;
		float num6 = (0f - _dragLinear) * num5;
		float num7 = num4 + num6 * timeStep;
		if (Mathf.Sign(num7) != Mathf.Sign(num4))
		{
			num7 = 0f;
		}
		float num8 = 0.5f * (num2 + num7) * timeStep;
		float num9 = num3 + num8;
		vel = num7;
		pos = num9;
		if (_isCaptured && Mathf.Sign(vel) == _bounceDir)
		{
			if (vel > 0f && pos > NotchIdxToPos(_captureNotchIdx))
			{
				HitNotch(_captureNotchIdx);
			}
			if (vel < 0f && pos < NotchIdxToPos(_captureNotchIdx))
			{
				HitNotch(_captureNotchIdx);
			}
		}
		if (wrapping)
		{
			return;
		}
		if (pos > max && vel > 0f)
		{
			if (!_isCaptured && NotchIdxToPos(_maxNotchIdx) == max)
			{
				CaptureWith(_maxNotchIdx);
				_bounceDir = Mathf.Sign(vel);
			}
			Bounce(max, _maxNotchIdx);
		}
		if (pos < min && vel < 0f)
		{
			if (!_isCaptured && NotchIdxToPos(_minNotchIdx) == min)
			{
				CaptureWith(_minNotchIdx);
				_bounceDir = Mathf.Sign(vel);
			}
			Bounce(min, _minNotchIdx);
		}
	}

	public void CaptureWith(int notchIdx)
	{
		_isCaptured = true;
		_captureNotchIdx = notchIdx;
	}

	private float NotchIdxToPos(int idx)
	{
		return origin + (float)idx * notchSeparation;
	}

	private void HitNotch(int notch)
	{
		switch (notchType)
		{
		case NotchType.Bounce:
			Bounce(NotchIdxToPos(notch), notch);
			break;
		case NotchType.Magnet:
			vel *= edgeElasticity;
			if (Mathf.Abs(vel) < _snapMaxSpeed && Mathf.Abs(pos - NotchIdxToPos(notch)) < 0.01f * notchSeparation)
			{
				Snap(notch);
			}
			break;
		}
	}

	private void Bounce(float bouncePos, int notchIdx)
	{
		vel *= 0f - edgeElasticity;
		pos = bouncePos;
		if (Mathf.Abs(vel) < _snapMaxSpeed && NotchIdxToPos(notchIdx) == bouncePos)
		{
			Snap(notchIdx);
		}
	}

	private void Snap(int notchIdx)
	{
		vel = 0f;
		pos = NotchIdxToPos(notchIdx);
		_stationary = true;
	}

	private float GetPosParam(float pos)
	{
		return (pos - origin) / notchSeparation;
	}

	public float GetNotchProximityParam(float pos)
	{
		float num = NotchIdxToPos(GetNearestNotch(pos));
		return Mathf.Clamp(1f - 2f * Mathf.Abs((pos - num) / notchSeparation), 0f, 1f);
	}

	private int GetNearestNotch(float pos)
	{
		int num = Mathf.RoundToInt(GetPosParam(pos));
		if (!wrapping)
		{
			num = Mathf.Clamp(num, _minNotchIdx, _maxNotchIdx);
		}
		return num;
	}

	private int GetNotchBelow(float pos)
	{
		int num = Mathf.FloorToInt(GetPosParam(pos));
		if (!wrapping)
		{
			num = Mathf.Clamp(num, _minNotchIdx, _maxNotchIdx);
		}
		return num;
	}

	private int GetNotchAbove(float pos)
	{
		int num = Mathf.CeilToInt(GetPosParam(pos));
		if (!wrapping)
		{
			num = Mathf.Clamp(num, _minNotchIdx, _maxNotchIdx);
		}
		return num;
	}

	private int SelectNotch(float pos, float vel)
	{
		if (_isCaptured)
		{
			return _captureNotchIdx;
		}
		if (Mathf.Abs(vel) < _snapMaxSpeed)
		{
			return GetNearestNotch(pos);
		}
		if (vel >= 0f)
		{
			return GetNotchAbove(pos);
		}
		return GetNotchBelow(pos);
	}

	public float GetNotchForce(float notchToPoint, float velocity)
	{
		if (!_isCaptured)
		{
			return 0f;
		}
		return (0f - Mathf.Sign(notchToPoint)) * _notchSpringConstLinear;
	}
}
