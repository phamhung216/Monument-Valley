using UnityEngine;

public class MaterialBlender : MonoBehaviour
{
	public MeshRenderer[] targetMeshRenderers;

	public SkinnedMeshRenderer[] targetSkinnedMeshRenderers;

	public MaterialInstantiator materialInstantiator;

	public Interpolation interp;

	private void LateUpdate()
	{
		MeshRenderer[] array = targetMeshRenderers;
		foreach (MeshRenderer meshRenderer in array)
		{
			if ((bool)meshRenderer)
			{
				meshRenderer.material.SetFloat("_Blend", interp.interpAmount);
			}
		}
		SkinnedMeshRenderer[] array2 = targetSkinnedMeshRenderers;
		foreach (SkinnedMeshRenderer skinnedMeshRenderer in array2)
		{
			if ((bool)skinnedMeshRenderer)
			{
				skinnedMeshRenderer.material.SetFloat("_Blend", interp.interpAmount);
			}
		}
		if (materialInstantiator != null)
		{
			materialInstantiator.instantiatedMaterial.SetFloat("_Blend", interp.interpAmount);
		}
	}
}
