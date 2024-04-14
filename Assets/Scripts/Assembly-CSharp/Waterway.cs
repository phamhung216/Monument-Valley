using UnityEngine;

public class Waterway : MonoBehaviour
{
	public WaterSection waterSection;

	public MeshRenderer waterRenderer;

	private void Start()
	{
		if (waterRenderer == null)
		{
			waterRenderer = (MeshRenderer)GetComponent<Renderer>();
		}
	}

	private void Update()
	{
	}
}
