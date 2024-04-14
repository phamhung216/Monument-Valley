using System.Collections;
using UnityEngine;

public class CharacterLookAt : MonoBehaviour
{
	public BaseLocomotion character;

	private static string _iconTexture = "GizmoCharacterLookAt";

	[TriggerableAction]
	public IEnumerator LookAtMe()
	{
		character.lookAtObject = true;
		character.lookAtObjectTarget = base.transform;
		return null;
	}

	private void OnDrawGizmos()
	{
		Gizmos.DrawIcon(base.transform.position, _iconTexture);
	}
}
