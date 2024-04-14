using UnityEngine;

public class HatTrigger : ProximityTrigger
{
	private const string s_iconTexture = "IconQuestion";

	public AnniversaryDateChecker anniversaryDateChecker;

	private void Awake()
	{
		bool num = anniversaryDateChecker.CheckDateInRange();
		AnniversarySettings.LoadSettings();
		if (!num || !AnniversarySettings.partyMode)
		{
			base.gameObject.SetActive(value: false);
		}
	}

	public override void Trigger()
	{
		if (base.gameObject.activeInHierarchy)
		{
			base.Trigger();
		}
	}

	private void OnDrawGizmos()
	{
		Gizmos.DrawIcon(base.transform.position, "IconQuestion");
		if ((bool)targetBrush)
		{
			Gizmos.color = Color.blue;
			Gizmos.DrawLine(base.transform.position, targetBrush.transform.position);
			if (disableNavOnExit)
			{
				Gizmos.color = Color.red;
				Gizmos.DrawWireSphere(base.transform.position, 0.5f);
			}
		}
	}
}
