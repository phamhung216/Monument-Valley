using System.Collections;
using UnityEngine;

public class LevelSelectLogic : MonoBehaviour
{
	private LevelName _selectedLevel;

	public LevelTextSetter levelTextSetter;

	public TriggerableActionSequence loadLevelSequence;

	public TriggerableActionSequence showResumePromptSequence;

	private void Start()
	{
	}

	[TriggerableAction]
	public IEnumerator DoLoad()
	{
		LoadSelectedLevel();
		return null;
	}

	[TriggerableAction]
	public IEnumerator ClearUnlockedLevelsAndRestart()
	{
		UserDataController.Instance.Reset();
		_selectedLevel = LevelName.Prelude;
		LoadSelectedLevel();
		return null;
	}

	[TriggerableAction]
	public IEnumerator ChooseLoadLevelOrResumePrompt()
	{
		if (LevelProgress.GetLastCheckpointLevel() == _selectedLevel)
		{
			showResumePromptSequence.TriggerActions();
		}
		else
		{
			loadLevelSequence.TriggerActions();
		}
		return null;
	}

	[TriggerableAction]
	public IEnumerator LoadSelectedLevel()
	{
		LevelManager.Instance.LoadLevel(_selectedLevel);
		return null;
	}

	public void SelectLevel(LevelName level)
	{
		_selectedLevel = level;
		levelTextSetter.SetTextForLevel(_selectedLevel);
	}
}
