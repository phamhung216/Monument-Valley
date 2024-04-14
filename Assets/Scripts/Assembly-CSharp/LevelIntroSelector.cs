using System.Collections;
using UnityEngine;

public class LevelIntroSelector : MonoBehaviour
{
	public TriggerableActionSequence normalLevelIntroFadeInSequence;

	public TriggerableActionSequence expansionLevelIntroFadeInSequence;

	public TriggerableActionSequence normalLevelIntroFadeOutSequence;

	public TriggerableActionSequence expansionLevelIntroFadeOutSequence;

	[TriggerableAction]
	public IEnumerator LevelIntroFadeIn()
	{
		if (LevelManager.Instance.levelData[LevelManager.CurrentLevel].expansion)
		{
			expansionLevelIntroFadeInSequence.TriggerActions();
		}
		else
		{
			normalLevelIntroFadeInSequence.TriggerActions();
		}
		return null;
	}

	[TriggerableAction]
	public IEnumerator LevelIntroFadeOut()
	{
		if (LevelManager.Instance.levelData[LevelManager.CurrentLevel].expansion)
		{
			expansionLevelIntroFadeOutSequence.TriggerActions();
		}
		else
		{
			normalLevelIntroFadeOutSequence.TriggerActions();
		}
		return null;
	}
}
