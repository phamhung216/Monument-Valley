using UnityEngine;

public class LevelTextSetter : MonoBehaviour
{
	public UIText chapterTitle;

	public UIText levelName;

	public UIText levelSubTitle;

	public UIText expansionChapterTitle;

	public UIText expansionLevelName;

	private void Start()
	{
		if (LevelManager.CurrentLevel != 0)
		{
			SetTextForLevel(LevelManager.CurrentLevel);
		}
	}

	public void SetTextForLevel(LevelName level)
	{
		int levelChapterNumber = LevelManager.Instance.GetLevelChapterNumber(level);
		string text = level.ToString("G").ToLower();
		if (LevelManager.Instance.levelData[level].expansion)
		{
			expansionChapterTitle.text = "$(exp" + levelChapterNumber + "_title)";
			expansionLevelName.text = "$(level_" + text + "_name)";
			return;
		}
		if (chapterTitle.gameObject.activeInHierarchy)
		{
			if (levelChapterNumber == 0)
			{
				chapterTitle.text = "";
			}
			else
			{
				chapterTitle.text = "$(ch" + levelChapterNumber + "_title)";
			}
		}
		if (levelName.gameObject.activeInHierarchy)
		{
			levelName.text = "$(level_" + text + "_name)";
		}
		if ((bool)levelSubTitle && levelSubTitle.gameObject.activeInHierarchy)
		{
			levelSubTitle.text = "$(level_" + text + "_subtitle)";
		}
	}
}
