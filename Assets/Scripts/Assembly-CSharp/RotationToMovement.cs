using System;
using UnityEngine;

public class RotationToMovement : MonoBehaviour
{
	public enum Mode
	{
		Angle = 0,
		Sine = 1,
		ConstrainedAngle = 2,
		Triangle = 3
	}

	public Rotatable sourceRot;

	public Vector3 deltaPos;

	public float numRotations = 1f;

	public bool lockWhenCharacterPresent = true;

	public bool useZeroStartPosition;

	public int rotationOffset;

	public Mode mode;

	private Vector3 _startPos;

	private float _param;

	private bool _wasMoving;

	public bool forceUpdateEveryFrame;

	private void Start()
	{
		if (lockWhenCharacterPresent)
		{
			sourceRot.characterDetector.AddParent(base.gameObject);
		}
		if (useZeroStartPosition)
		{
			base.transform.localPosition = Vector3.zero;
		}
		_startPos = base.transform.localPosition;
	}

	private void Update()
	{
		if (sourceRot.dragging || sourceRot.snapping || _wasMoving || forceUpdateEveryFrame)
		{
			_wasMoving = sourceRot.dragging || sourceRot.snapping;
			_param = (sourceRot.currentAngle + (float)rotationOffset) / 360f / numRotations;
			switch (mode)
			{
			case Mode.Angle:
				_param = 1f * _param;
				break;
			case Mode.ConstrainedAngle:
				_param = 1f * _param % 2f;
				if (_param < 0f)
				{
					_param *= -1f;
				}
				if (_param > 1f)
				{
					_param = 2f - _param;
				}
				break;
			case Mode.Sine:
				_param = 0.5f - 0.5f * Mathf.Cos((float)Math.PI * 2f * _param);
				break;
			case Mode.Triangle:
				_param = 2f * _param % 2f;
				if (_param < 0f)
				{
					_param += 2f;
				}
				if (_param > 1f)
				{
					_param = 2f - _param;
				}
				break;
			}
			Vector3 localPosition = _startPos + deltaPos * _param;
			base.transform.localPosition = localPosition;
		}
		else
		{
			_wasMoving = false;
		}
	}

	private void OnDrawGizmos()
	{
	}
}
