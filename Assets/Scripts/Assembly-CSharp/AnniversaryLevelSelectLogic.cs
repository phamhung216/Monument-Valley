using UnityEngine;

public class AnniversaryLevelSelectLogic : MonoBehaviour
{
	public AnniversaryDateChecker anniversaryDateChecker;

	public GameObject textAndFireWorks;

	public TriggerableActionSequence actionSequence;

	public GameObject[] candles;

	private void Start()
	{
		AnniversarySettings.LoadSettings();
		if ((anniversaryDateChecker != null && !anniversaryDateChecker.CheckDateInRange()) || !AnniversarySettings.partyMode)
		{
			SetAnniversaryGameObjetsActve(enable: false, showSequence: false);
		}
		else
		{
			StartCoroutine(actionSequence.RunSequence());
		}
	}

	public void SetAnniversaryGameObjetsActve(bool enable, bool showSequence)
	{
		textAndFireWorks.SetActive(enable);
		for (int i = 0; i < candles.Length; i++)
		{
			candles[i].SetActive(enable);
		}
		if (enable && showSequence)
		{
			StartCoroutine(actionSequence.RunSequence());
		}
	}
}
