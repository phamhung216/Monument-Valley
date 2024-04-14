using UnityEngine;

public class DebugNote : MonoBehaviour
{
	public string debugNote;

	public Texture2D icon;

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawIcon(base.transform.position + new Vector3(0f, 0.75f, 0f), icon.name);
	}
}
