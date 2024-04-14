using UnityEngine;

public class MeshCombineTag : MonoBehaviour
{
	public enum MeshTag
	{
		CombineAllBelow = 0,
		DoNotCombine = 1
	}

	public MeshTag meshTag;

	public bool staticBatch;

	public bool staticLightMap = true;

	public bool staticLightMapOffset = true;

	public string finalLayerName;
}
