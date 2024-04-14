using System.Collections;
using UnityEngine;

public class TimeControl : MonoBehaviour
{
	public enum TimePeriod
	{
		PAST = 0,
		FUTURE = 1
	}

	private Rect timeButtonRect = new Rect(0f, 0f, 100f, 50f);

	public TimePeriod currentTimePeriod;

	public Material[] blendMats;

	private float blendAmount;

	public Animation seaLevelAni;

	public GameObject fixedBridge;

	public GameObject crumbleAnimation;

	public ParticleSystem crumbleParticle;

	public bool animating;

	public GameObject fruits;

	public PlantTreeTrigger treeTrigger;

	public PlantBlockTrigger blockTrigger;

	public GameObject growingTree;

	public GameObject growingBlock;

	public GameObject roller;

	private Animation treeAnim;

	private Animation blockAnim;

	public GameObject topPath;

	private void Start()
	{
		MeshRenderer[] array = Object.FindObjectsOfType(typeof(MeshRenderer)) as MeshRenderer[];
		for (int i = 0; i < blendMats.Length; i++)
		{
			Material material = blendMats[i];
			Material material2 = Object.Instantiate(material);
			MeshRenderer[] array2 = array;
			foreach (MeshRenderer meshRenderer in array2)
			{
				if (meshRenderer.sharedMaterial.name == material.name)
				{
					meshRenderer.sharedMaterial = material2;
				}
			}
			blendMats[i] = material2;
			blendMats[i].SetFloat("_Blend", 0f);
		}
		treeAnim = growingTree.GetComponent<Animation>();
		blockAnim = growingBlock.GetComponent<Animation>();
		topPath.SetActive(value: false);
	}

	private void Update()
	{
	}

	public void ToggleTimePeriod()
	{
		currentTimePeriod = 1 - currentTimePeriod;
		StartCoroutine("ChangeToTimePeriod", currentTimePeriod);
	}

	public IEnumerator ChangeToTimePeriod(TimePeriod period)
	{
		topPath.SetActive(value: false);
		animating = true;
		if (period == TimePeriod.FUTURE)
		{
			if (treeTrigger.stageComplete)
			{
				growingTree.SetActive(value: true);
				treeAnim["TreeAnim"].speed = 1.5f;
				treeAnim["TreeAnim"].time = 0f;
				treeAnim.Play();
				if (blockTrigger.blockReady)
				{
					roller.SetActive(value: false);
					growingBlock.SetActive(value: true);
					blockAnim["TreeBlockAnim"].speed = 1.5f;
					blockAnim["TreeBlockAnim"].time = 0f;
					blockAnim.Play();
				}
			}
			seaLevelAni["SeaLevelDown"].speed = 0.5f;
			seaLevelAni["SeaLevelDown"].time = 0f;
			seaLevelAni.Play();
			yield return new WaitForSeconds(2f);
			fixedBridge.SetActive(value: false);
			crumbleAnimation.SetActive(value: true);
			crumbleAnimation.GetComponent<Animation>()["CrumbleAnimation"].speed = 1.5f;
			crumbleAnimation.GetComponent<Animation>()["CrumbleAnimation"].time = 0f;
			crumbleAnimation.GetComponent<Animation>().Play();
			fruits.SetActive(value: false);
			blendAmount = 0f;
			while (blendAmount < 1f)
			{
				blendAmount += 0.05f;
				Material[] array = blendMats;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].SetFloat("_Blend", blendAmount);
				}
				yield return new WaitForSeconds(0.05f);
			}
			while (crumbleAnimation.GetComponent<Animation>().isPlaying)
			{
				crumbleAnimation.GetComponent<Animation>()["CrumbleAnimation"].speed = 3f;
				yield return null;
			}
			if (growingBlock.activeSelf)
			{
				while (blockAnim.isPlaying)
				{
					yield return null;
				}
				topPath.SetActive(value: true);
			}
		}
		else
		{
			topPath.SetActive(value: false);
			if (growingTree.activeSelf)
			{
				treeAnim["TreeAnim"].speed = -2f;
				treeAnim["TreeAnim"].time = treeAnim["TreeAnim"].length;
				treeAnim.Play();
				if (growingBlock.activeSelf)
				{
					blockAnim["TreeBlockAnim"].speed = -2f;
					blockAnim["TreeBlockAnim"].time = blockAnim["TreeBlockAnim"].length;
					blockAnim.Play();
				}
			}
			crumbleAnimation.GetComponent<Animation>()["CrumbleAnimation"].speed = -3f;
			crumbleAnimation.GetComponent<Animation>()["CrumbleAnimation"].time = crumbleAnimation.GetComponent<Animation>()["CrumbleAnimation"].length;
			crumbleAnimation.GetComponent<Animation>().Play();
			yield return new WaitForSeconds(1f);
			crumbleAnimation.GetComponent<Animation>()["CrumbleAnimation"].speed = -1.5f;
			yield return new WaitForSeconds(2f);
			seaLevelAni["SeaLevelDown"].speed = -1f;
			seaLevelAni["SeaLevelDown"].time = seaLevelAni["SeaLevelDown"].length;
			seaLevelAni.Play();
			blendAmount = 1f;
			while (blendAmount > 0f)
			{
				blendAmount -= 0.05f;
				Material[] array = blendMats;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].SetFloat("_Blend", blendAmount);
				}
				yield return new WaitForSeconds(0.05f);
			}
			fruits.SetActive(value: true);
			while (seaLevelAni.isPlaying)
			{
				yield return null;
			}
			while (crumbleAnimation.GetComponent<Animation>().isPlaying)
			{
				yield return null;
			}
			if (growingTree.activeSelf)
			{
				while (treeAnim.isPlaying)
				{
					yield return null;
				}
				growingTree.SetActive(value: false);
			}
			if (growingBlock.activeSelf)
			{
				while (blockAnim.isPlaying)
				{
					yield return null;
				}
				growingBlock.SetActive(value: false);
				roller.SetActive(value: true);
			}
			fixedBridge.SetActive(value: true);
			crumbleAnimation.SetActive(value: false);
		}
		yield return null;
		GameScene.navManager.ScanAllConnections();
		animating = false;
		yield return null;
	}
}
