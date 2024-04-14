using System.Collections;
using UnityEngine;

public class AnniversarySettings : MonoBehaviour
{
	public static bool partyMode;

	public UILayout partyButtonLayout;

	public UILayout pcPartyLayout;

	public UIDropdown pcPartyDropdown;

	private UIToggleButton _partyButton;

	private const string partyModeKey = "partyModeKey";

	public AnniversaryDateChecker anniversaryDateChecker;

	private AnniversaryLevelSelectLogic _anniversaryLevelSelectUILogic;

	public void Init()
	{
		_partyButton = partyButtonLayout.GetComponentInChildren<UIToggleButton>();
		_anniversaryLevelSelectUILogic = Object.FindObjectOfType<AnniversaryLevelSelectLogic>();
		LoadSettings();
		if (anniversaryDateChecker != null && !anniversaryDateChecker.CheckDateInRange())
		{
			partyButtonLayout.gameObject.SetActive(value: false);
			pcPartyLayout.gameObject.SetActive(value: false);
			partyButtonLayout.transform.parent.GetComponent<UILinearLayout>().enabled = false;
		}
		RefreshButtons();
		pcPartyDropdown.onValueChanged.AddListener(OnPartyDropdownValueChanged);
	}

	[TriggerableAction]
	public IEnumerator RefreshButtons()
	{
		if (_partyButton.isActiveAndEnabled)
		{
			_partyButton.isSelected = partyMode;
		}
		pcPartyDropdown.SetValue((!partyMode) ? 1 : 0);
		return null;
	}

	private void OnPartyDropdownValueChanged(int value)
	{
		if (partyMode != (value == 0))
		{
			ToggleParty();
		}
	}

	[TriggerableAction]
	public IEnumerator ToggleParty()
	{
		partyMode = !partyMode;
		SaveSettings();
		if (_anniversaryLevelSelectUILogic != null)
		{
			_anniversaryLevelSelectUILogic.SetAnniversaryGameObjetsActve(partyMode, showSequence: false);
		}
		return null;
	}

	public static void LoadSettings()
	{
		partyMode = UserDataController.GetUserLocalPrefsInt("partyModeKey", 1) == 1;
	}

	private void SaveSettings()
	{
		UserDataController.SetUserLocalPrefsInt("partyModeKey", partyMode ? 1 : 0);
	}
}
