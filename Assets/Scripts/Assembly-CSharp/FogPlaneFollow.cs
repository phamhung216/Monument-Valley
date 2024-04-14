using UnityEngine;

public class FogPlaneFollow : MonoBehaviour
{
	public MaterialInstantiator materialInstantiator;

	public Transform followThis;

	private Vector4 plane;

	private void LateUpdate()
	{
		plane = materialInstantiator.instantiatedMaterial.GetVector("_Plane");
		plane = new Vector4(plane.x, plane.y, plane.z, 0f - followThis.position.y);
		materialInstantiator.instantiatedMaterial.SetVector("_Plane", plane);
	}
}
