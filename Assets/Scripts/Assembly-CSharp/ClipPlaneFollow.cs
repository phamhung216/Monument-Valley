using UnityEngine;

public class ClipPlaneFollow : MonoBehaviour
{
	public MaterialInstantiator materialInstantiator;

	public Transform followThis;

	public string planeParameterName;

	public bool useLocalY;

	private Vector4 plane;

	private void Start()
	{
		if (string.IsNullOrEmpty(planeParameterName))
		{
			planeParameterName = "_Plane";
		}
	}

	private void LateUpdate()
	{
		plane = materialInstantiator.instantiatedMaterial.GetVector(planeParameterName);
		plane = new Vector4(plane.x, plane.y, plane.z, useLocalY ? followThis.localPosition.y : followThis.position.y);
		materialInstantiator.instantiatedMaterial.SetVector(planeParameterName, plane);
	}
}
