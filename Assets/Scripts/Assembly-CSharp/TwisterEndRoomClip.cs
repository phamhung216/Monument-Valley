using UnityEngine;

public class TwisterEndRoomClip : MonoBehaviour
{
	public Transform doorway;

	public MaterialInstantiator[] mats;

	private Vector3 _left = new Vector3(-0.5f, 0f, 0.5f);

	private Vector3 _right = new Vector3(0.5f, 0f, 0.5f);

	private Vector4 result = Vector4.zero;

	private void LateUpdate()
	{
		Vector3 forward = Camera.main.transform.forward;
		Vector3 rhs = doorway.parent.TransformPoint(doorway.localPosition + _left);
		Vector3 rhs2 = doorway.parent.TransformPoint(doorway.localPosition + _right);
		for (int i = 0; i < mats.Length; i++)
		{
			Vector3 lhs = Vector3.Cross(forward, -Vector3.up);
			result.Set(lhs.x, lhs.y, lhs.z, 0f);
			result.w = Vector3.Dot(lhs, rhs2);
			mats[i].instantiatedMaterial.SetVector("_Plane", result);
			lhs = Vector3.Cross(forward, Vector3.up);
			result.Set(lhs.x, lhs.y, lhs.z, 0f);
			result.w = Vector3.Dot(lhs, rhs);
			mats[i].instantiatedMaterial.SetVector("_Plane2", result);
		}
	}
}
