using System.Collections;
using UnityEngine;

public class TotemTeleport : MonoBehaviour
{
	public NavBrushComponent destinationBrush;

	public TotemPole targetTotem;

	private static string _iconTexture = "GizmoTotemTeleport";

	private void Start()
	{
		if (targetTotem == null)
		{
			targetTotem = Object.FindObjectOfType(typeof(TotemPole)) as TotemPole;
		}
		if (targetTotem == null)
		{
			D.Error("TotemTeleport: No totem Found in scene.");
		}
	}

	[TriggerableAction]
	public IEnumerator Teleport()
	{
		if (targetTotem != null)
		{
			GameScene.navManager.NotifyReconfigurationBegan(targetTotem.gameObject);
			targetTotem.Teleport(destinationBrush, base.transform.rotation);
			GameScene.navManager.NotifyReconfigurationEnded();
		}
		return null;
	}

	private void OnDrawGizmos()
	{
		Gizmos.DrawIcon(base.transform.position, _iconTexture);
		if ((bool)destinationBrush)
		{
			Gizmos.color = Color.blue;
			Gizmos.DrawLine(base.transform.position, destinationBrush.transform.position);
		}
	}
}
