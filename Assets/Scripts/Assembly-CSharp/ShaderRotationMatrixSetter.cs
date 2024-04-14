using UnityEngine;

public class ShaderRotationMatrixSetter : MonoBehaviour
{
	public Transform targetTransform;

	public Material material;

	public MaterialInstantiator materialInstantiator;

	private void Start()
	{
		if (material != null)
		{
			material.SetMatrix("_NewRotation", targetTransform.worldToLocalMatrix);
		}
		if (materialInstantiator != null)
		{
			materialInstantiator.instantiatedMaterial.SetMatrix("_NewRotation", targetTransform.worldToLocalMatrix);
		}
	}
}
