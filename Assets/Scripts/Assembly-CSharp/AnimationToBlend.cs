using UnityEngine;

public class AnimationToBlend : MonoBehaviour
{
	public float amount;

	public MeshRenderer targetRenderer;

	private Material targetMat;

	private void Start()
	{
		targetMat = targetRenderer.material;
	}

	private void LateUpdate()
	{
		targetMat.SetFloat("_Blend", amount);
	}
}
