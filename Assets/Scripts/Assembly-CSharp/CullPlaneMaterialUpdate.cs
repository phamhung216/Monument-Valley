using UnityEngine;

public class CullPlaneMaterialUpdate : MonoBehaviour
{
	public Material[] materialsToUpdate;

	public Vector3 relPlaneDir = new Vector3(0f, 1f, 0f);

	private void Start()
	{
	}

	private void LateUpdate()
	{
		for (int i = 0; i < materialsToUpdate.Length; i++)
		{
			Vector3 lhs = base.transform.TransformDirection(relPlaneDir);
			float w = Vector3.Dot(lhs, base.transform.position) - 0.5f;
			Vector4 value = new Vector4(lhs.x, lhs.y, lhs.z, w);
			materialsToUpdate[i].SetVector("_Plane", value);
		}
	}
}
