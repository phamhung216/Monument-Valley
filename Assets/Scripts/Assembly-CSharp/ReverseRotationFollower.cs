using UnityEngine;

public class ReverseRotationFollower : MonoBehaviour
{
	public Transform targetTransform;

	private void Update()
	{
		Vector3 localEulerAngles = targetTransform.localEulerAngles;
		float x = localEulerAngles.x;
		float y = localEulerAngles.y;
		float z = localEulerAngles.z;
		float num = Mathf.DeltaAngle(315f, y);
		localEulerAngles.Set(x, 315f - num, z);
		base.transform.localEulerAngles = localEulerAngles;
	}
}
