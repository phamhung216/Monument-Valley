using UnityEngine;

public class DrawRotatorNodeTriggerGizmo : MonoBehaviour
{
	public bool display;

	protected Color gizmoOutline = Color.magenta;

	protected Color gizmoFill = new Color(1f, 0f, 1f, 0.3f);

	private void OnDrawGizmos()
	{
		if (display)
		{
			Vector3 size = Vector3.one * 3f;
			Gizmos.color = gizmoOutline;
			Gizmos.DrawWireCube(base.transform.position, size);
			Gizmos.color = gizmoFill;
			Gizmos.DrawCube(base.transform.position, size);
		}
	}
}
