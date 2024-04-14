using System.Collections;
using UnityEngine;

public class NavUtils : MonoBehaviour
{
	public int newZone;

	public static void SetZoneForChildren(GameObject parent, int zone)
	{
		NavBrushComponent[] componentsInChildren = parent.transform.GetComponentsInChildren<NavBrushComponent>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].zone = zone;
		}
	}

	public static void EnableNavForChildren(GameObject parent, bool enable)
	{
		NavBrushComponent[] componentsInChildren = parent.transform.GetComponentsInChildren<NavBrushComponent>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].gameObject.SetActive(enable);
		}
	}

	[TriggerableAction]
	public IEnumerator DoSetZone()
	{
		SetZoneForChildren(base.gameObject, newZone);
		return null;
	}

	[TriggerableAction]
	public IEnumerator DoDisableNav()
	{
		EnableNavForChildren(base.gameObject, enable: false);
		return null;
	}

	[TriggerableAction]
	public IEnumerator DoEnableNav()
	{
		EnableNavForChildren(base.gameObject, enable: true);
		return null;
	}
}
