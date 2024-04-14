using UnityEngine;

public class DrawTriggerGizmo : MonoBehaviour
{
	protected Color gizmoOutline = Color.yellow;

	protected Color gizmoFill = new Color(1f, 1f, 0f, 0.3f);

	private Vector3 _size = Vector3.one;

	private Collider _collider;

	private ProximityTrigger _trigger;

	private void OnDrawGizmos()
	{
		if (_collider == null)
		{
			_collider = GetComponent<Collider>();
		}
		if (_collider != null)
		{
			_size = _collider.bounds.size;
		}
		Gizmos.color = gizmoOutline;
		Gizmos.DrawWireCube(base.transform.position, _size);
		if (_trigger == null)
		{
			_trigger = GetComponent<ProximityTrigger>();
		}
		if ((bool)_trigger && (bool)_trigger.targetBrush)
		{
			Gizmos.DrawLine(base.transform.position, _trigger.targetBrush.transform.position);
		}
		Gizmos.color = gizmoFill;
		Gizmos.DrawCube(base.transform.position, _size);
	}
}
