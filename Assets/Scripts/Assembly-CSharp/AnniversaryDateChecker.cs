using UnityEngine;

public class AnniversaryDateChecker : MonoBehaviour
{
	public enum TestDateCheat
	{
		Default = 0,
		AlwaysPass = 1,
		AlwaysFail = 2
	}

	[Tooltip("dd/MM/yyyy")]
	public string fromDate = "11/03/2019";

	[Tooltip("dd/MM/yyyy")]
	public string toDate = "22/03/2019";

	private string DATE_FORMAT = "dd/MM/yyyy";

	public TestDateCheat test;

	public bool CheckDateInRange()
	{
		return false;
	}
}
