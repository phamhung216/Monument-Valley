using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TriggerLoadScene : MonoBehaviour
{
	public LevelSelectLogic levelSelectLogic;

	public string sceneName;

	public LevelName levelName;

	private bool _triggered;

	[TriggerableAction]
	public IEnumerator LoadScene()
	{
		if (!_triggered)
		{
			_triggered = true;
			if (string.IsNullOrEmpty(sceneName))
			{
				TextMesh componentInChildren = GetComponentInChildren<TextMesh>();
				if ((bool)componentInChildren)
				{
					sceneName = componentInChildren.text;
				}
			}
			SceneManager.LoadScene(sceneName);
		}
		return null;
	}

	[TriggerableAction]
	public IEnumerator LoadLevel()
	{
		if (levelName != 0)
		{
			LevelManager.Instance.LoadLevel(levelName);
		}
		return null;
	}

	[TriggerableAction]
	public IEnumerator SelectLevel()
	{
		levelSelectLogic.SelectLevel(levelName);
		return null;
	}
}
