using UnityEngine;

public class UpdateVersionText : MonoBehaviour
{
	public string prefix;

	private void Start()
	{
		GetComponent<UIText>().text = prefix + ApplicationInfo.bundleVersion;
	}
}
