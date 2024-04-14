using UnityEngine;

public class MaterialDataCopy : MonoBehaviour
{
	public Material sourceMaterial;

	public Material[] targetMaterials;

	public bool copyUseLightMapFlag;

	public bool copyLightAdd = true;

	public bool copyShadowBoost = true;

	public void DoCopy()
	{
		if (!(sourceMaterial != null))
		{
			return;
		}
		Material[] array = targetMaterials;
		foreach (Material dstMat in array)
		{
			CopyFloat("_LightIntensity", sourceMaterial, dstMat);
			CopyColor("_LightColour0", sourceMaterial, dstMat);
			CopyColor("_LightColour1", sourceMaterial, dstMat);
			CopyColor("_LightColour2", sourceMaterial, dstMat);
			CopyColor("_LightColour3", sourceMaterial, dstMat);
			CopyColor("_ShadowColour", sourceMaterial, dstMat);
			CopyColor("_LightTint", sourceMaterial, dstMat);
			if (copyShadowBoost)
			{
				CopyColor("_ShadowRamp", sourceMaterial, dstMat);
			}
			if (copyLightAdd)
			{
				CopyColor("_AmbientColour1", sourceMaterial, dstMat);
			}
			if (copyUseLightMapFlag)
			{
				CopyFloat("_UseLightMap", sourceMaterial, dstMat);
			}
		}
	}

	private void CopyColor(string propertyName, Material srcMat, Material dstMat)
	{
		if (srcMat.HasProperty(propertyName) && dstMat.HasProperty(propertyName))
		{
			dstMat.SetColor(propertyName, sourceMaterial.GetColor(propertyName));
		}
	}

	private void CopyFloat(string propertyName, Material srcMat, Material dstMat)
	{
		if (srcMat.HasProperty(propertyName) && dstMat.HasProperty(propertyName))
		{
			dstMat.SetFloat(propertyName, sourceMaterial.GetFloat(propertyName));
		}
	}
}
