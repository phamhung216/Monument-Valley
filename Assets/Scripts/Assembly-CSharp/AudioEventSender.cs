using Fabric;
using UnityEngine;

public class AudioEventSender : MonoBehaviour
{
	public void PlayAudioEvent(string name)
	{
		if ((bool)EventManager.Instance)
		{
			EventManager.Instance.PostEvent(name, EventAction.PlaySound);
		}
	}

	public void EndLoop()
	{
	}
}
