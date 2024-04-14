using System.Collections.Generic;
using UnityEngine;

public class ComponentEnabler : MonoBehaviour
{
	public MonoBehaviour target;

	public List<ObjectProximityTrigger> someTriggered = new List<ObjectProximityTrigger>();

	private void Update()
	{
		bool flag = false;
		foreach (ObjectProximityTrigger item in someTriggered)
		{
			flag |= item.isTriggered;
		}
		if (target != null)
		{
			target.enabled = flag;
		}
	}
}
