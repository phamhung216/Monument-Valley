using System.Collections;
using UnityEngine;

public class ResumePromptPageController : MonoBehaviour
{
	public UIText chapterNameLabel;

	[TriggerableAction]
	public IEnumerator ResetProgress()
	{
		LevelProgress.ClearProgress();
		return null;
	}
}
