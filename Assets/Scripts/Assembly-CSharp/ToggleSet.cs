using UnityEngine;

public class ToggleSet : MonoBehaviour
{
	public Transform[] targets;

	public Vector3 inactivePosition;

	private void Awake()
	{
		Transform[] array = targets;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].gameObject.SetActive(value: true);
		}
	}

	public void Show()
	{
		Transform[] array = targets;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].localPosition = Vector3.zero;
		}
	}

	public void Hide()
	{
		Transform[] array = targets;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].localPosition = inactivePosition;
		}
	}
}
