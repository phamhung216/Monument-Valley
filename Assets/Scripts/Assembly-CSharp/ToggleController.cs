using System.Collections;
using UnityEngine;

public class ToggleController : MonoBehaviour
{
	public ToggleSet[] toggleSets;

	public void ShowOnly(int index)
	{
		for (int i = 0; i < toggleSets.Length; i++)
		{
			if (i != index)
			{
				toggleSets[i].Hide();
			}
		}
		toggleSets[index].Show();
	}

	[TriggerableAction]
	public IEnumerator HideAll()
	{
		for (int i = 0; i < toggleSets.Length; i++)
		{
			toggleSets[i].Hide();
		}
		return null;
	}

	[TriggerableAction]
	public IEnumerator ShowAll()
	{
		for (int i = 0; i < toggleSets.Length; i++)
		{
			toggleSets[i].Show();
		}
		return null;
	}

	[TriggerableAction]
	public IEnumerator ShowOnly_0()
	{
		ShowOnly(0);
		return null;
	}

	[TriggerableAction]
	public IEnumerator ShowOnly_1()
	{
		ShowOnly(1);
		return null;
	}

	[TriggerableAction]
	public IEnumerator ShowOnly_2()
	{
		ShowOnly(2);
		return null;
	}

	[TriggerableAction]
	public IEnumerator ShowOnly_3()
	{
		ShowOnly(3);
		return null;
	}
}
