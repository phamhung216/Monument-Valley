using System.Collections;
using UnityEngine;

public class TriggerObjectActive : MonoBehaviour
{
	[Tooltip("If left null, will act on its own Game Object")]
	public GameObject affectedObject;

	public bool startActive = true;

	private bool _containsNav;

	private void Start()
	{
		if (affectedObject == null)
		{
			affectedObject = base.gameObject;
		}
		affectedObject.SetActive(startActive);
		_containsNav = affectedObject.GetComponentsInChildren<NavBrushComponent>().Length != 0;
	}

	[TriggerableAction]
	public IEnumerator DoTrigger()
	{
		Activate();
		return null;
	}

	[TriggerableAction]
	public IEnumerator Activate()
	{
		if (affectedObject != null)
		{
			affectedObject.SetActive(value: true);
			if (_containsNav)
			{
				GameScene.navManager.NotifyReconfigurationEnded();
			}
		}
		return null;
	}

	[TriggerableAction]
	public IEnumerator DeActivate()
	{
		if (affectedObject != null)
		{
			affectedObject.SetActive(value: false);
			if (_containsNav)
			{
				GameScene.navManager.NotifyReconfigurationEnded();
			}
		}
		return null;
	}
}
