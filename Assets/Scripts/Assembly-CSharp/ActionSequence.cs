using System;
using System.Collections;
using System.Collections.Generic;
using UnityCommon;
using UnityEngine;

[Serializable]
public class ActionSequence
{
	public List<TriggerAction> actions = new List<TriggerAction>();

	public bool debugMe;

	public int actionCount => actions.Count;

	public IEnumerator DoSequence()
	{
		foreach (TriggerAction action in actions)
		{
			if (!GameScene.logActionSequences)
			{
				_ = debugMe;
			}
			action.debugMe = debugMe;
			if ((OrientationOverrideManager.IsLandscape() && action.orientationBehaviour == TriggerAction.OrientationBehaviour.ExecuteInPortrait) || (OrientationOverrideManager.IsPortrait() && action.orientationBehaviour == TriggerAction.OrientationBehaviour.ExecuteInLandscape))
			{
				continue;
			}
			IEnumerator actionEnum = action.RunAction();
			if (actionEnum == null)
			{
				continue;
			}
			while (actionEnum.MoveNext())
			{
				if (!GameScene.logActionSequences)
				{
					_ = debugMe;
				}
				if (TriggerAction.FastForward)
				{
					DebugUtils.DebugAssert(actionEnum.Current == null);
					break;
				}
				yield return actionEnum.Current;
			}
			if (!GameScene.logActionSequences)
			{
				_ = debugMe;
			}
		}
		if (!GameScene.logActionSequences)
		{
			_ = debugMe;
		}
	}

	public void SanityCheck(GameObject owner)
	{
		_ = debugMe;
		for (int i = 0; i < actions.Count; i++)
		{
			actions[i].SanityCheck(i, owner);
		}
	}
}
