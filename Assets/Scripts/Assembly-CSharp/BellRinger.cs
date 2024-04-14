using System.Collections;
using UnityEngine;

public class BellRinger : InteractiveItem
{
	public Animation berryDoorway;

	private bool animActive;

	public GameObject manPickupBerry;

	public GameObject manShrug;

	public GameObject fruit;

	private BerryPositionTrigger trigger;

	public bool stageComplete;

	private void Start()
	{
		trigger = GameObject.Find("BerryTreeTrigger").GetComponent<BerryPositionTrigger>();
	}

	public override void Trigger()
	{
		base.Trigger();
		if (!animActive)
		{
			animActive = true;
			GetComponentInChildren<Animation>().Play();
			StartCoroutine(StartAnimationSequence());
		}
	}

	private IEnumerator StartAnimationSequence()
	{
		yield return new WaitForSeconds(2f);
		OpenDoor();
		yield return new WaitForSeconds(2f);
		if (trigger.berryCaught)
		{
			manPickupBerry.SetActive(value: true);
			fruit.SetActive(value: false);
			Animation anim = manPickupBerry.GetComponentInChildren<Animation>();
			anim.Play();
			while (anim.isPlaying)
			{
				yield return null;
			}
			manPickupBerry.SetActive(value: false);
			stageComplete = true;
		}
		else
		{
			manShrug.SetActive(value: true);
			Animation anim = manShrug.GetComponentInChildren<Animation>();
			anim.Play();
			while (anim.isPlaying)
			{
				yield return null;
			}
			manShrug.SetActive(value: false);
			animActive = false;
		}
		CloseDoor();
	}

	private void OpenDoor()
	{
		berryDoorway[berryDoorway.clip.name].time = 0f;
		berryDoorway[berryDoorway.clip.name].speed = 1f;
		berryDoorway.Play();
	}

	private void CloseDoor()
	{
		berryDoorway[berryDoorway.clip.name].time = berryDoorway[berryDoorway.clip.name].length;
		berryDoorway[berryDoorway.clip.name].speed = -2.5f;
		berryDoorway.Play();
	}
}
