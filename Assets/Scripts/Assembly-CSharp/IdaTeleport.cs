using System.Collections;
using UnityEngine;

public class IdaTeleport : MonoBehaviour
{
	public NavBrushComponent destinationBrush;

	private CharacterLocomotion _ida;

	private static string _iconTexture = "GizmoCharacterLookAt";

	[TriggerableAction]
	public IEnumerator Teleport()
	{
		_ida = GameScene.player.GetComponent<CharacterLocomotion>();
		if (_ida != null)
		{
			_ida.transform.position = destinationBrush.transform.position;
			_ida.transform.rotation = base.transform.rotation;
			_ida.Teleport(destinationBrush);
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
