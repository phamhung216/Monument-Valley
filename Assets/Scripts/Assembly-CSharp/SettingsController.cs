using UnityEngine;

public class SettingsController : MonoBehaviour
{
	private void Start()
	{
		if (!Debug.isDebugBuild)
		{
			base.gameObject.transform.parent.gameObject.SetActive(value: false);
		}
	}
}
