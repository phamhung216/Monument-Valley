using System.Collections;
using UnityEngine;

public class DoorConnector : MonoBehaviour
{
	public DoorComponent doorA;

	public DoorComponent doorB;

	public bool unlockDoorsAfterConnection;

	[TriggerableAction]
	public IEnumerator MakeConnection()
	{
		doorA.ConnectTo(doorB);
		if (unlockDoorsAfterConnection)
		{
			doorA.UnlockDoor();
			doorB.UnlockDoor();
		}
		return null;
	}
}
