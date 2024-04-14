using UnityEngine;

public class DrawUpGizmo : MonoBehaviour
{
	private void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawRay(base.transform.position, base.transform.up);
	}
}
