using System.Collections;
using Fabric;
using UnityEngine;

public class AudioSwitcher : MonoBehaviour
{
	public string audioEvent;

	public string switchTo;

	public GameObject audioOwner;

	private SwitchComponent _switchComponent;

	private void Start()
	{
		if (!audioOwner)
		{
			audioOwner = GameScene.instance.gameObject;
		}
	}

	[TriggerableAction]
	public IEnumerator SwitchEvent()
	{
		if ((bool)EventManager.Instance)
		{
			EventManager.Instance.PostEvent(audioEvent, EventAction.SetSwitch, switchTo, audioOwner);
		}
		return null;
	}
}
