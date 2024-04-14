using System;
using UnityEngine;

public class RotateSegment : MonoBehaviour
{
	public enum axis
	{
		X = 0,
		Y = 1,
		Z = 2
	}

	public Rotatable master;

	public Transform target;

	public float offset;

	public axis rotationAxis;

	private void LateUpdate()
	{
		Vector3 eulerAngles = target.eulerAngles;
		float num = offset + 45f + 45f * Mathf.Cos(0.5f * master.currentAngle * ((float)Math.PI / 180f));
		switch (rotationAxis)
		{
		case axis.X:
			target.eulerAngles = new Vector3(num, eulerAngles.y, eulerAngles.z);
			break;
		case axis.Y:
			target.eulerAngles = new Vector3(eulerAngles.x, num, eulerAngles.z);
			break;
		case axis.Z:
			target.eulerAngles = new Vector3(eulerAngles.x, eulerAngles.y, num);
			break;
		}
	}
}
