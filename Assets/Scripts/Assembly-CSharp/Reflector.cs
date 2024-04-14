using UnityEngine;

public class Reflector : MonoBehaviour
{
	public Transform master;

	public Transform slave;

	public Transform clampYMinPoint;

	public Transform clampYMaxPoint;

	private Vector3 clampMin;

	private Vector3 clampMax;

	private void Start()
	{
		clampMin = -10000f * Vector3.one;
		clampMax = 10000f * Vector3.one;
		clampMin.z = 1f;
		Vector3 vector = base.transform.InverseTransformPoint(clampYMinPoint.position);
		Vector3 vector2 = base.transform.InverseTransformPoint(clampYMaxPoint.position);
		clampMin.x = Mathf.Min(vector.x, vector2.x);
		clampMax.x = Mathf.Max(vector.x, vector2.x);
		clampMin.y = Mathf.Min(vector.y, vector2.y);
		clampMax.y = Mathf.Max(vector.y, vector2.y);
	}

	private void LateUpdate()
	{
		Vector3 position = base.transform.InverseTransformPoint(master.position);
		position.z *= -1f;
		position.x = Mathf.Clamp(position.x, clampMin.x, clampMax.x);
		position.y = Mathf.Clamp(position.y, clampMin.y, clampMax.y);
		position.z = Mathf.Clamp(position.z, clampMin.z, clampMax.z);
		slave.position = base.transform.TransformPoint(position);
		slave.eulerAngles = new Vector3(master.eulerAngles.x, 0f - master.eulerAngles.y, master.eulerAngles.z);
	}
}
