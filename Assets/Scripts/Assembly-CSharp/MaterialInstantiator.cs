using UnityEngine;

public class MaterialInstantiator : MonoBehaviour
{
	public Material materialToSearchFor;

	private Material _instantiatedMaterial;

	public Transform parentToSearch;

	public TexturePanner texturePanner;

	public bool debugMe;

	public Material instantiatedMaterial => _instantiatedMaterial;

	private void Awake()
	{
		if (!(materialToSearchFor != null))
		{
			return;
		}
		if (parentToSearch == null)
		{
			parentToSearch = base.transform;
		}
		_instantiatedMaterial = Object.Instantiate(materialToSearchFor);
		_instantiatedMaterial.name = materialToSearchFor.name + "_MaterialInstantiator";
		if ((bool)texturePanner)
		{
			texturePanner.SetPanMat(_instantiatedMaterial);
		}
		if (parentToSearch == null)
		{
			parentToSearch = base.transform;
		}
		Renderer[] componentsInChildren = parentToSearch.GetComponentsInChildren<Renderer>();
		foreach (Renderer renderer in componentsInChildren)
		{
			if (renderer.sharedMaterial == materialToSearchFor)
			{
				renderer.sharedMaterial = _instantiatedMaterial;
			}
			Material[] sharedMaterials = renderer.sharedMaterials;
			bool flag = false;
			for (int j = 0; j < renderer.sharedMaterials.Length; j++)
			{
				_ = debugMe;
				if (renderer.sharedMaterials[j] == materialToSearchFor)
				{
					flag = true;
					sharedMaterials[j] = _instantiatedMaterial;
					_ = debugMe;
				}
			}
			if (flag)
			{
				renderer.sharedMaterials = sharedMaterials;
			}
		}
		PressureSwitchMatSwap[] componentsInChildren2 = parentToSearch.GetComponentsInChildren<PressureSwitchMatSwap>();
		foreach (PressureSwitchMatSwap pressureSwitchMatSwap in componentsInChildren2)
		{
			if (materialToSearchFor == pressureSwitchMatSwap.originalMat)
			{
				pressureSwitchMatSwap.originalMat = _instantiatedMaterial;
			}
			if (materialToSearchFor == pressureSwitchMatSwap.alternateMat)
			{
				pressureSwitchMatSwap.alternateMat = _instantiatedMaterial;
			}
		}
	}
}
