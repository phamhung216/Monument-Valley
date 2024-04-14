using UnityEngine;

public class WorldUILanguageChangeUpdate : MonoBehaviour
{
	[SerializeField]
	private LanguageSelectPage _languageSelectPage;

	private UIText _ui;

	private void Start()
	{
		_ui = GetComponent<UIText>();
		if ((bool)_languageSelectPage && (bool)_ui)
		{
			_languageSelectPage.onLanguageChanged.AddListener(OnLanguageChanged);
		}
	}

	private void OnDestroy()
	{
		if ((bool)_languageSelectPage && (bool)_ui)
		{
			_languageSelectPage.onLanguageChanged.RemoveListener(OnLanguageChanged);
		}
	}

	private void OnLanguageChanged(string id)
	{
		_ui.OnLanguageChanged();
	}
}
