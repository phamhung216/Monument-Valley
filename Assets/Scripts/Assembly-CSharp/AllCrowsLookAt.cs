using System.Collections;
using UnityEngine;

public class AllCrowsLookAt : MonoBehaviour
{
	private AIController[] crows;

	private static string _iconTexture = "GizmoCrowsLookAt";

	private void Start()
	{
		FindCrows();
	}

	[TriggerableAction]
	public IEnumerator AllCrowsLookAtTarget()
	{
		if (crows == null)
		{
			FindCrows();
		}
		for (int i = 0; i < crows.Length; i++)
		{
			AIController aIController = crows[i];
			if (aIController != null)
			{
				aIController.lookAtObjectTarget = base.transform;
			}
		}
		return null;
	}

	private void FindCrows()
	{
		crows = Object.FindObjectsOfType(typeof(AIController)) as AIController[];
	}

	private void OnDrawGizmos()
	{
		Gizmos.DrawIcon(base.transform.position, _iconTexture);
	}
}
