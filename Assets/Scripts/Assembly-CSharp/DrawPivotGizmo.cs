using UnityEngine;

public class DrawPivotGizmo : MonoBehaviour
{
	private void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawSphere(base.transform.position, 0.5f);
	}
}
