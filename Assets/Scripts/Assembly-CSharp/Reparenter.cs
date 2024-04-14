using System.Collections;
using UnityEngine;

public class Reparenter : MonoBehaviour
{
	public Transform parentTransform;

	public Transform[] childTransforms;

	public bool setLocalPositionZero;

	public bool setLocalRotationZero;

	[TriggerableAction]
	public IEnumerator AssignNewParent()
	{
		Transform[] array = childTransforms;
		foreach (Transform transform in array)
		{
			transform.parent = parentTransform;
			if (setLocalPositionZero)
			{
				transform.localPosition = Vector3.zero;
			}
			if (setLocalRotationZero)
			{
				transform.localRotation = Quaternion.identity;
			}
		}
		return null;
	}
}
