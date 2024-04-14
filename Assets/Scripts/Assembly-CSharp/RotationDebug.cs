using UnityEngine;

public class RotationDebug : MonoBehaviour
{
	public Vector3 eulerAnglesInEditor;

	public Vector3 eulerAnglesForGameplay;

	public void SetForEditor()
	{
		base.transform.localEulerAngles = eulerAnglesInEditor;
	}

	public void SetForGameplay()
	{
		base.transform.localEulerAngles = eulerAnglesForGameplay;
	}

	public static void SetAllForEditor()
	{
		RotationDebug[] array = Object.FindObjectsOfType(typeof(RotationDebug)) as RotationDebug[];
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetForEditor();
		}
	}

	public static void SetAllForGameplay()
	{
		RotationDebug[] array = Object.FindObjectsOfType(typeof(RotationDebug)) as RotationDebug[];
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetForGameplay();
		}
	}
}
