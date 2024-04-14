using System.Collections;
using UnityEngine;

public class CrowTeleport : MonoBehaviour
{
	public NavBrushComponent destinationBrush;

	public AIController crow;

	private static string _iconTexture = "GizmoCrowsLookAt";

	[TriggerableAction]
	public IEnumerator Teleport()
	{
		if (crow != null)
		{
			crow.transform.position = destinationBrush.transform.position;
			crow.transform.rotation = base.transform.rotation;
			crow.Teleport(destinationBrush);
		}
		return null;
	}

	private void OnDrawGizmos()
	{
		Vector3 position = base.transform.position;
		Gizmos.DrawIcon(position, _iconTexture);
		if ((bool)destinationBrush)
		{
			Gizmos.color = Color.blue;
			Gizmos.DrawLine(base.transform.position, destinationBrush.transform.position);
		}
		Gizmos.color = Color.green;
		Gizmos.DrawLine(position, position + base.transform.forward * 2f);
	}
}
