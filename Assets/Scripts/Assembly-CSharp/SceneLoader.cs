using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
	public static string nextLevelName;

	private void Start()
	{
		Resources.UnloadUnusedAssets();
		GC.Collect();
		SceneManager.LoadScene(nextLevelName);
	}
}
