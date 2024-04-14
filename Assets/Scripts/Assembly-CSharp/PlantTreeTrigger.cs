using System.Collections;
using UnityEngine;

public class PlantTreeTrigger : MonoBehaviour
{
	public Dragger roller;

	private BellRinger ringer;

	public Animation treeDoorway;

	private bool animActive;

	public bool stageComplete;

	public GameObject manPlantBerry;

	private void Start()
	{
		ringer = GameObject.Find("BellContainer").GetComponent<BellRinger>();
	}

	private void Update()
	{
		if (!animActive && ringer.stageComplete && !roller.dragging && !roller.snapping && GetComponent<Collider>().bounds.Contains(roller.transform.position))
		{
			animActive = true;
			StartCoroutine(StartAnimationSequence());
		}
	}

	private IEnumerator StartAnimationSequence()
	{
		yield return new WaitForSeconds(1f);
		roller.enabled = false;
		OpenDoor();
		yield return new WaitForSeconds(2f);
		manPlantBerry.SetActive(value: true);
		Animation anim = manPlantBerry.GetComponentInChildren<Animation>();
		anim.Play();
		while (anim.isPlaying)
		{
			yield return null;
		}
		manPlantBerry.SetActive(value: false);
		CloseDoor();
		roller.enabled = true;
		stageComplete = true;
	}

	private void OpenDoor()
	{
		treeDoorway[treeDoorway.clip.name].time = 0f;
		treeDoorway[treeDoorway.clip.name].speed = 1f;
		treeDoorway.Play();
	}

	private void CloseDoor()
	{
		treeDoorway[treeDoorway.clip.name].time = treeDoorway[treeDoorway.clip.name].length;
		treeDoorway[treeDoorway.clip.name].speed = -2.5f;
		treeDoorway.Play();
	}
}
