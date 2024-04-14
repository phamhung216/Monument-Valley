using UnityEngine;

public class NSidedRoofClip : MonoBehaviour
{
	public MaterialInstantiator[] mats;

	public Transform planeL;

	public Transform planeR;

	private Vector4 _p0 = Vector4.zero;

	private Vector4 _p1 = Vector4.zero;

	private void LateUpdate()
	{
		Vector3 up = planeL.up;
		up.Normalize();
		_p0.Set(up.x, up.y, up.z, 0f);
		_p0.w = Vector3.Dot(up, planeL.position);
		Vector3 up2 = planeR.up;
		up2.Normalize();
		_p1.Set(up2.x, up2.y, up2.z, 0f);
		_p1.w = Vector3.Dot(up2, planeR.position);
		for (int i = 0; i < mats.Length; i++)
		{
			MaterialInstantiator obj = mats[i];
			obj.instantiatedMaterial.SetVector("_Plane", _p0);
			obj.instantiatedMaterial.SetVector("_Plane2", _p1);
		}
	}
}
