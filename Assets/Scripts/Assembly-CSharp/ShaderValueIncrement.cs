using System.Collections.Generic;
using UnityEngine;

public class ShaderValueIncrement : MonoBehaviour
{
	public List<Material> materials = new List<Material>();

	private void Start()
	{
		for (int i = 0; i < materials.Count; i++)
		{
			Vector3 up = base.gameObject.transform.up;
			float w = Vector3.Dot(base.transform.up, base.gameObject.transform.position);
			materials[i].SetVector("_Plane", -new Vector4(up.x, up.y, up.z, w));
		}
	}

	private void Update()
	{
		for (int i = 0; i < materials.Count; i++)
		{
			Vector3 up = base.transform.up;
			float w = Vector3.Dot(base.transform.up, base.transform.position);
			materials[i].SetVector("_Plane", -new Vector4(up.x, up.y, up.z, w));
		}
	}
}
