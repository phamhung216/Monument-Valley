using System.Collections;
using UnityEngine;

public class ScreenshotHelper : MonoBehaviour
{
	public Camera[] cameras;

	public CameraPanController[] cameraPanControllers;

	public GameObject[] turnOff;

	public float newCameraSize = 17f;

	[TriggerableAction]
	public IEnumerator EnterScreenshotMode()
	{
		CameraPanController[] array = cameraPanControllers;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].enabled = false;
		}
		Camera[] array2 = cameras;
		for (int i = 0; i < array2.Length; i++)
		{
			array2[i].orthographicSize = newCameraSize;
		}
		GameObject[] array3 = turnOff;
		for (int i = 0; i < array3.Length; i++)
		{
			array3[i].SetActive(value: false);
		}
		return null;
	}
}
