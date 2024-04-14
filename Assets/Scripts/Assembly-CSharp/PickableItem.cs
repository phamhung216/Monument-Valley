using System.Collections;
using UnityEngine;

public class PickableItem : MonoBehaviour
{
	private Transform _oldParent;

	[TriggerableAction]
	public IEnumerator AttachToPlayer()
	{
		Transform parent = GameScene.player.transform.Find("Ida").Find("IdaArmature").Find("root_c")
			.Find("artifact_c");
		_oldParent = base.transform.parent;
		base.transform.parent = parent;
		base.transform.localRotation = new Quaternion(0f, 0f, 0f, 1f);
		base.transform.localPosition = Vector3.zero;
		base.transform.localScale = Vector3.one;
		return null;
	}

	[TriggerableAction]
	public IEnumerator DetachFromPlayer()
	{
		base.transform.parent = _oldParent;
		return null;
	}
}
