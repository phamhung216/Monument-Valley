using UnityEngine;

public class InsideBoxClip : MonoBehaviour
{
	public MaterialInstantiator[] mats;

	public Transform planeLeft;

	public Transform planeRight;

	public Transform pivotLeft;

	public Transform pivotRight;

	private void Update()
	{
		Vector3 forward = Camera.main.transform.forward;
		float num = Vector3.Dot(forward, pivotLeft.forward);
		planeLeft.up = Vector3.Cross(pivotLeft.forward * ((!(num < 0f)) ? 1 : (-1)), forward);
		if (Vector3.Angle(planeLeft.up, pivotLeft.up) >= 90f)
		{
			planeLeft.up = pivotLeft.right;
		}
		planeRight.up = Vector3.Cross(forward, pivotRight.forward);
		Vector3 up = planeLeft.up;
		up.Normalize();
		Vector4 value = new Vector4(up.x, up.y, up.z, 0f);
		value.w = Vector3.Dot(up, planeLeft.position);
		Vector3 up2 = planeRight.up;
		up2.Normalize();
		Vector4 value2 = new Vector4(up2.x, up2.y, up2.z, 0f);
		value2.w = Vector3.Dot(up2, planeRight.position);
		for (int i = 0; i < mats.Length; i++)
		{
			MaterialInstantiator obj = mats[i];
			obj.instantiatedMaterial.SetVector("_Plane", value);
			obj.instantiatedMaterial.SetVector("_Plane2", value2);
		}
	}
}
