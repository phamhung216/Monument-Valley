using UnityEngine;

public class UVScroller : MonoBehaviour
{
	public Vector2 scrollSpeed;

	public Material material;

	private void LateUpdate()
	{
		material.SetTextureOffset("_MainTex", scrollSpeed * Time.time);
	}
}
