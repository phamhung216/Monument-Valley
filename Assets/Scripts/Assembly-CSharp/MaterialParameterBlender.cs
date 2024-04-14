using System.Collections;
using UnityEngine;

public class MaterialParameterBlender : MonoBehaviour
{
	public MaterialInstantiator targetMaterialInstantiator;

	public Material sourceA;

	public Material sourceB;

	public DescentLightningLogic lightning;

	public Interpolation lightningInterp;

	public AutoInterp interp;

	private bool hasShadowColor;

	private Color _lightningColor0 = Color.clear;

	private Color _lightningColor1 = Color.clear;

	private Color _lightningColor2 = Color.clear;

	private void Start()
	{
		hasShadowColor = targetMaterialInstantiator.instantiatedMaterial.HasProperty("_ShadowColour") && sourceA.HasProperty("_ShadowColour") && sourceB.HasProperty("_ShadowColour");
	}

	private void LateUpdate()
	{
		UpdateBlend();
	}

	[TriggerableAction]
	public IEnumerator BlendNow()
	{
		UpdateBlend();
		return null;
	}

	private void UpdateBlend()
	{
		if (lightning != null && lightningInterp != null)
		{
			_lightningColor0 = lightning.lightningMat.GetColor("_LightColour0") * lightningInterp.interpAmount;
			_lightningColor1 = lightning.lightningMat.GetColor("_LightColour1") * lightningInterp.interpAmount;
			_lightningColor2 = lightning.lightningMat.GetColor("_LightColour2") * lightningInterp.interpAmount;
		}
		targetMaterialInstantiator.instantiatedMaterial.SetColor("_LightColour0", Color.Lerp(sourceA.GetColor("_LightColour0"), sourceB.GetColor("_LightColour0"), interp.interpAmount) + _lightningColor0);
		targetMaterialInstantiator.instantiatedMaterial.SetColor("_LightColour1", Color.Lerp(sourceA.GetColor("_LightColour1"), sourceB.GetColor("_LightColour1"), interp.interpAmount) + _lightningColor1);
		targetMaterialInstantiator.instantiatedMaterial.SetColor("_LightColour2", Color.Lerp(sourceA.GetColor("_LightColour2"), sourceB.GetColor("_LightColour2"), interp.interpAmount) + _lightningColor2);
		targetMaterialInstantiator.instantiatedMaterial.SetColor("_LightColour3", Color.Lerp(sourceA.GetColor("_LightColour3"), sourceB.GetColor("_LightColour3"), interp.interpAmount));
		targetMaterialInstantiator.instantiatedMaterial.SetColor("_AmbientColour1", Color.Lerp(sourceA.GetColor("_AmbientColour1"), sourceB.GetColor("_AmbientColour1"), interp.interpAmount));
		UpdateColor("_ShadowColour");
	}

	private void UpdateColor(string uniformName)
	{
		if (hasShadowColor)
		{
			targetMaterialInstantiator.instantiatedMaterial.SetColor(uniformName, Color.Lerp(sourceA.GetColor(uniformName), sourceB.GetColor(uniformName), interp.interpAmount));
		}
	}
}
