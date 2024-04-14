using System.Collections;
using UnityEngine;

public class Interpolation : MonoBehaviour
{
	public float interpAmount;

	[TriggerableAction]
	public IEnumerator SetToOne()
	{
		interpAmount = 1f;
		return null;
	}

	[TriggerableAction]
	public IEnumerator SetToZero()
	{
		interpAmount = 0f;
		return null;
	}
}
