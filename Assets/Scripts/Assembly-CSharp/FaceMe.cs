using UnityEngine;

public class FaceMe : MonoBehaviour
{
	public enum FaceMeMode
	{
		Constrain_1_axes = 0,
		Constrain_2_axes = 1
	}

	public FaceMeMode faceMeMode;

	public bool doWobble;

	private float rotAngle;

	public float wobbleSpeed = 0.1f;

	public float wobbleAmount = 10f;

	private void Start()
	{
		DebugUtils.DebugAssert(base.transform.parent != null);
		rotAngle = Random.value * 90f;
	}

	private void LateUpdate()
	{
		Vector3 vector = -Camera.main.transform.forward;
		Vector3 vector2 = base.transform.parent.transform.up;
		if (doWobble)
		{
			rotAngle += wobbleSpeed;
			vector2 = Quaternion.AngleAxis(Mathf.Sin(rotAngle) * wobbleAmount, vector) * vector2;
		}
		Vector3 vector3 = Vector3.Cross(vector2, vector);
		vector3.Normalize();
		if (faceMeMode == FaceMeMode.Constrain_1_axes)
		{
			vector = Vector3.Cross(vector3, vector2);
			vector.Normalize();
		}
		else
		{
			vector2 = Vector3.Cross(vector, vector3);
			vector2.Normalize();
		}
		Quaternion rotation = default(Quaternion);
		rotation.SetLookRotation(vector, vector2);
		base.transform.rotation = rotation;
	}
}
