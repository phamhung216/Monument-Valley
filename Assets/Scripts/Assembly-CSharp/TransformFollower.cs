using System.Collections;
using UnityEngine;

public class TransformFollower : MonoBehaviour
{
	public bool followRotation = true;

	public bool followPosition;

	public bool followX;

	public bool followY;

	public bool followZ;

	public Transform targetTransform;

	public bool localSpace = true;

	public float multiplier = 1f;

	public Vector3 rotationOffset = Vector3.zero;

	public bool clampPosition;

	public Vector3 minPos;

	public Vector3 maxPos;

	private Quaternion _rotationOffset = Quaternion.identity;

	[TriggerableAction]
	public IEnumerator EnableClampPosition()
	{
		clampPosition = true;
		return null;
	}

	[TriggerableAction]
	public IEnumerator DisableClampPosition()
	{
		clampPosition = false;
		return null;
	}

	[TriggerableAction]
	public IEnumerator StartFollowingPosition()
	{
		followPosition = true;
		return null;
	}

	[TriggerableAction]
	public IEnumerator StopFollowingPosition()
	{
		followPosition = false;
		return null;
	}

	public void SetRotationOffset(Vector3 eulerAngles)
	{
		rotationOffset = eulerAngles;
		_rotationOffset = Quaternion.Euler(rotationOffset);
	}

	private void Start()
	{
		SetRotationOffset(rotationOffset);
	}

	private void LateUpdate()
	{
		if (followRotation)
		{
			if (localSpace)
			{
				Quaternion quaternion = targetTransform.localRotation * _rotationOffset;
				if (quaternion != base.transform.localRotation)
				{
					base.transform.localRotation = quaternion;
				}
			}
			else
			{
				Quaternion quaternion2 = targetTransform.rotation * _rotationOffset;
				if (quaternion2 != base.transform.rotation)
				{
					base.transform.rotation = quaternion2;
				}
			}
		}
		if (!followPosition)
		{
			return;
		}
		if (localSpace)
		{
			Vector3 localPosition = base.transform.localPosition;
			Vector3 localPosition2 = targetTransform.localPosition;
			Vector3 vector = localPosition;
			vector.x = (followX ? (localPosition2.x * multiplier) : localPosition.x);
			vector.y = (followY ? (localPosition2.y * multiplier) : localPosition.y);
			vector.z = (followZ ? (localPosition2.z * multiplier) : localPosition.z);
			if (clampPosition)
			{
				vector = Vector3.Max(vector, minPos);
				vector = Vector3.Min(vector, maxPos);
			}
			if (vector != localPosition)
			{
				base.transform.localPosition = vector;
			}
		}
		else
		{
			Vector3 position = base.transform.position;
			Vector3 position2 = targetTransform.position;
			Vector3 vector2 = position;
			vector2.x = (followX ? (position2.x * multiplier) : position.x);
			vector2.y = (followY ? (position2.y * multiplier) : position.y);
			vector2.z = (followZ ? (position2.z * multiplier) : position.z);
			if (clampPosition)
			{
				vector2 = Vector3.Max(vector2, minPos);
				vector2 = Vector3.Min(vector2, maxPos);
			}
			if (vector2 != position)
			{
				base.transform.position = vector2;
			}
		}
	}
}
