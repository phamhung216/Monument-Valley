using System.Collections;
using UnityEngine;

public class TriggerableActionSequence : MonoBehaviour
{
	public ActionSequence actions = new ActionSequence();

	private static string _iconTexture = "GizmoSequence";

	private void Start()
	{
	}

	[TriggerableAction]
	public IEnumerator TriggerActions()
	{
		StartCoroutine(actions.DoSequence());
		return null;
	}

	[TriggerableAction(true)]
	public IEnumerator RunSequence()
	{
		_ = GameScene.logActionSequences;
		IEnumerator action = actions.DoSequence();
		if (action != null)
		{
			while (action.MoveNext())
			{
				_ = GameScene.logActionSequences;
				yield return action.Current;
			}
		}
		_ = GameScene.logActionSequences;
	}

	private void OnDrawGizmos()
	{
		Gizmos.DrawIcon(base.transform.position, _iconTexture);
	}
}
