using System.Collections;
using UnityEngine;

public class AmbienceFader : MonoBehaviour
{
	private WaterfallFilterLogic waterfall;

	private void Start()
	{
		waterfall = Object.FindObjectOfType<WaterfallFilterLogic>();
	}

	[TriggerableAction]
	public IEnumerator FadeOut()
	{
		GameScene.instance.GetComponent<SceneAudio>().StopAmbience();
		if (waterfall != null)
		{
			waterfall.KillWaterfallAudio();
		}
		return null;
	}
}
