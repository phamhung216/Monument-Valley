using System.Collections;
using UnityEngine;

public class LayerChanger : MonoBehaviour
{
	public GameObject target;

	public string newLayerName;

	[TriggerableAction]
	public IEnumerator Change()
	{
		target.layer = LayerMask.NameToLayer(newLayerName);
		return null;
	}
}
