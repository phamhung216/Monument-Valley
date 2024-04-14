using System.Collections;
using UnityEngine;

public class FinaleEpilogueLogic : MonoBehaviour
{
	public Transform thunderbird;

	public Transform flyingContainer;

	public ParticleSystem skyParticles;

	public Transform crowContainer;

	private Animation[] _crowAnims;

	private void Start()
	{
		_crowAnims = crowContainer.GetComponentsInChildren<Animation>();
		Animation[] crowAnims = _crowAnims;
		foreach (Animation animation in crowAnims)
		{
			animation["Fly"].time = Random.Range(0f, animation["Fly"].length);
			animation["Fly"].wrapMode = WrapMode.Loop;
		}
	}

	[TriggerableAction]
	public IEnumerator SetIdaPositionToThunderbird()
	{
		GameScene.player.transform.position = thunderbird.position;
		GameScene.player.transform.rotation = thunderbird.rotation;
		return null;
	}

	[TriggerableAction]
	public IEnumerator MoveThunderbirdToFlyingPosition()
	{
		thunderbird.parent = flyingContainer;
		thunderbird.position = flyingContainer.position;
		thunderbird.rotation = flyingContainer.rotation;
		GameScene.player.transform.position = thunderbird.position;
		GameScene.player.transform.rotation = thunderbird.rotation;
		thunderbird.GetComponentInChildren<Animation>()["Fly"].wrapMode = WrapMode.Loop;
		thunderbird.GetComponentInChildren<Animation>().Play("Fly");
		skyParticles.Play();
		Animation[] crowAnims = _crowAnims;
		for (int i = 0; i < crowAnims.Length; i++)
		{
			crowAnims[i].Play("Fly");
		}
		return null;
	}
}
