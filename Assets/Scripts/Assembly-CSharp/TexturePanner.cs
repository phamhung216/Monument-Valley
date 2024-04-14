using UnityEngine;

public class TexturePanner : MonoBehaviour
{
	public Renderer[] renderers;

	public Vector2 panSpeed;

	private Vector2 offset;

	public Material panMaterial;

	public int materialIndex;

	public string textureName = "_MainTex";

	public MaterialInstantiator matInst;

	private void Start()
	{
		if (renderers.Length == 0)
		{
			renderers = new Renderer[1];
			renderers[0] = GetComponent<Renderer>();
		}
		if (!panMaterial)
		{
			panMaterial = renderers[0].materials[materialIndex];
		}
		if ((bool)matInst)
		{
			panMaterial = matInst.instantiatedMaterial;
		}
		offset = panMaterial.GetTextureOffset(textureName);
	}

	private void Update()
	{
		offset += panSpeed * Time.deltaTime;
		if ((bool)panMaterial)
		{
			panMaterial.SetTextureOffset(textureName, offset);
		}
	}

	public void SetPanMat(Material mat)
	{
		panMaterial = mat;
	}
}
