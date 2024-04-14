using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinaleCinematicLogic : MonoBehaviour
{
	private List<CrowFinale> crows;

	public CrowFinale firstCrow;

	private float meltDelay = 3f;

	public Rotatable starRotator;

	public TriggerableActionSequence postStarSequence;

	public GameObject realCrowPrefab;

	public ToggleController toggle;

	public ToggleController totemToggle;

	public Transform towerRoot;

	public Rotatable towerRotator;

	private int crowIdx;

	public AutoInterp[] stars;

	private void Start()
	{
		CrowFinale[] obj = Object.FindObjectsOfType(typeof(CrowFinale)) as CrowFinale[];
		crows = new List<CrowFinale>();
		CrowFinale[] array = obj;
		foreach (CrowFinale item in array)
		{
			crows.Add(item);
		}
		crows.Sort(delegate(CrowFinale a, CrowFinale b)
		{
			if (a.transform.position.y > b.transform.position.y)
			{
				return -1;
			}
			return (a.transform.position.y < b.transform.position.y) ? 1 : 0;
		});
		int index = crows.IndexOf(firstCrow);
		CrowFinale value = crows[0];
		crows[0] = firstCrow;
		crows[index] = value;
		for (int j = 0; j < crows.Count; j++)
		{
			crows[j].animSystem.GetComponentInChildren<SkinnedMeshRenderer>().enabled = false;
		}
	}

	private void Update()
	{
		if (starRotator.dragEnabled && starRotator.isStationary && Mathf.Abs(Mathf.DeltaAngle(starRotator.currentAngle, 180f)) < 1f)
		{
			starRotator.DisableDrag();
			postStarSequence.TriggerActions();
		}
	}

	[TriggerableAction]
	public IEnumerator StartCrowsWalking()
	{
		meltDelay = 2f;
		StartCoroutine(CrowWalkCoroutine());
		return null;
	}

	private IEnumerator CrowWalkCoroutine()
	{
		for (int i = 0; i < crows.Count; i++)
		{
			crows[i].animSystem.GetComponentInChildren<SkinnedMeshRenderer>().enabled = true;
			crows[i].StartMoving();
			if (meltDelay > 0.2f)
			{
				meltDelay *= 0.8f;
			}
			yield return new WaitForSeconds(meltDelay);
		}
	}

	[TriggerableAction]
	public IEnumerator StartCrowsMelting()
	{
		meltDelay = 2f;
		crowIdx = 0;
		StartCoroutine(CrowsMeltCoroutine());
		return null;
	}

	private IEnumerator CrowsMeltCoroutine()
	{
		for (int i = 0; i < crows.Count; i++)
		{
			crows[i].TriggerMelt();
			if (meltDelay > 0.25f)
			{
				meltDelay *= 0.75f;
			}
			yield return new WaitForSeconds(meltDelay);
		}
	}

	[TriggerableAction]
	public IEnumerator DoPostIdolTowerSetup()
	{
		if (toggle != null && towerRoot != null)
		{
			GameScene.navManager.NotifyReconfigurationBegan(towerRoot.gameObject);
			towerRotator.ApplyAngle(360f);
			toggle.ShowOnly_1();
			GameScene.navManager.NotifyReconfigurationEnded();
			totemToggle.ShowOnly_0();
		}
		return null;
	}

	[TriggerableAction(true)]
	public IEnumerator FadeStarsIn()
	{
		AutoInterp[] array = stars;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Interp();
			yield return new WaitForSeconds(0.5f);
		}
	}

	[TriggerableAction]
	public IEnumerator FadeStarsOut()
	{
		AutoInterp[] array = stars;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].ReverseInterp();
		}
		return null;
	}

	[TriggerableAction]
	public IEnumerator CrowsLookAtStars()
	{
		for (int i = 0; i < crows.Count; i++)
		{
			crows[i].StartHeadLook();
		}
		return null;
	}
}
