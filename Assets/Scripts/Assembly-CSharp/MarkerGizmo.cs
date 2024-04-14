using UnityEngine;

public class MarkerGizmo : MonoBehaviour
{
	public Color color = Color.cyan;

	public string iconTexture = "";

	private void OnDrawGizmos()
	{
		Gizmos.color = color;
		if (iconTexture.Length > 0)
		{
			Gizmos.DrawIcon(base.transform.position, iconTexture);
		}
		else
		{
			Gizmos.DrawCube(base.transform.position, 0.3f * Vector3.one);
		}
	}
}
