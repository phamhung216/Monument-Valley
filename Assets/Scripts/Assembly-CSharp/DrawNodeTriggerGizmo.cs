using UnityEngine;

public class DrawNodeTriggerGizmo : MonoBehaviour
{
	protected Color gizmoOutline = Color.green;

	protected Color gizmoFill = new Color(0f, 1f, 0f, 0.3f);

	private void OnDrawGizmos()
	{
		Vector3 one = Vector3.one;
		Gizmos.color = gizmoOutline;
		Gizmos.DrawWireCube(base.transform.position, one);
		Gizmos.color = gizmoFill;
		Gizmos.DrawCube(base.transform.position, one);
	}
}
