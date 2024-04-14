using UnityEngine;

public class LogoSelector : MonoBehaviour
{
	public Transform[] logoPieces;

	private void Start()
	{
		LocalisationManager.Instance.onLanguageChanged.AddListener(OnLanguageChanged);
		OnLanguageChanged(LocalisationManager.Instance.currentLanguage);
	}

	private void OnLanguageChanged(DeviceLanguage language)
	{
		string text = language.ToString();
		if ((uint)(language - 5) > 1u)
		{
			text = "Default";
		}
		Transform[] array = logoPieces;
		foreach (Transform transform in array)
		{
			if (!transform)
			{
				continue;
			}
			Transform[] componentsInChildren = transform.GetComponentsInChildren<Transform>(includeInactive: true);
			foreach (Transform transform2 in componentsInChildren)
			{
				if (transform2.parent == transform)
				{
					if (transform2.gameObject.name == text)
					{
						transform2.gameObject.SetActive(value: true);
					}
					else
					{
						transform2.gameObject.SetActive(value: false);
					}
				}
			}
		}
	}
}
