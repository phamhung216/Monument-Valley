using System.Collections;
using UnityEngine;

public class NotifySceneInvertedCamera : MonoBehaviour
{
	public Camera invertedCamera;

	[TriggerableAction]
	public IEnumerator SetAsCurrentInvertedCamera()
	{
		GameScene.instance.invertedCamera = invertedCamera;
		return null;
	}
}
