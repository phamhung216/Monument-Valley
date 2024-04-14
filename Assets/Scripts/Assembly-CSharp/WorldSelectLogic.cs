using System.Collections;
using UnityEngine;

public class WorldSelectLogic : MonoBehaviour
{
	public TriggerableActionSequence go_To_Original;

	public TriggerableActionSequence go_To_Expansion;

	public TriggerableActionSequence go_To_RedSuits;

	public TriggerableActionSequence go_To_Blue;

	public TriggerableActionSequence current_World_Reselected;

	public TriggerableActionSequence hide_Purchase_Expansion;

	public TriggerableActionSequence hide_Purchase_RedSuits;

	public TriggerableActionSequence show_Purchase_Expansion;

	public TriggerableActionSequence show_Purchase_RedSuits;

	public TriggerableActionSequence show_Purchasing;

	public TriggerableActionSequence hide_Purchasing;

	public TriggerableActionSequence show_RedSuits;

	public TriggerableActionSequence hide_RedSuits;

	public TriggerableActionSequence show_Blue;

	public TriggerableActionSequence show_RedSuitsInfo;

	public TriggerableActionSequence hide_RedSuitsInfo;

	public TriggerableActionSequence popup_Purchase_Successful;

	public TriggerableActionSequence popup_Purchase_Failed;

	public TriggerableActionSequence popup_NoConnection;

	public TriggerableActionSequence popup_Hide;

	public TriggerableActionSequence play_Volcano;

	public bool expansion_unlocked;

	public UILayout worldSelectBody_UILayout;

	public int redSuitsHeight = 380;

	public UIText debugText_RedSuits_Available;

	public UIText debugText_Purchase_Success;

	public UIText expansionPriceText;

	public UIText redSuitsPriceText;

	public UIImage selectionIndicatorMV;

	public UIImage selectionIndicatorFS;

	public string selectionIndicatorSubTexture_On;

	public string selectionIndicatorSubTexture_Off;

	public UILayout crossSellLinkIcon;

	public UILayout crossSellLinkSpinner;

	private InAppPurchaseLogic _inAppPurchaseLogic;

	private RedSuitsLogic _redSuitsLogic;

	private InAppPurchaseManager _inAppPurchaseManager;

	private bool _testPurchase;

	private bool _isShowingCrossSellBusyIndicator;

	private void Start()
	{
		_inAppPurchaseLogic = Object.FindObjectOfType<InAppPurchaseLogic>();
		_inAppPurchaseManager = Object.FindObjectOfType<InAppPurchaseManager>();
		_redSuitsLogic = Object.FindObjectOfType<RedSuitsLogic>();
		if (!Debug.isDebugBuild)
		{
			debugText_RedSuits_Available.GetComponentInParent<UIButton>().gameObject.SetActive(value: false);
			debugText_Purchase_Success.GetComponentInParent<UIButton>().gameObject.SetActive(value: false);
		}
		UpdateDebugText();
		LevelName currentLevel = LevelManager.CurrentLevel;
		selectionIndicatorMV.SetSubTextureName((currentLevel == LevelName.LevelSelect) ? selectionIndicatorSubTexture_On : selectionIndicatorSubTexture_Off);
		selectionIndicatorFS.SetSubTextureName((currentLevel == LevelName.LevelSelectExpansion) ? selectionIndicatorSubTexture_On : selectionIndicatorSubTexture_Off);
		crossSellLinkSpinner.gameObject.SetActive(value: false);
	}

	[TriggerableAction]
	public IEnumerator Toggle_Expansion_Unlocked()
	{
		if ((bool)_inAppPurchaseLogic)
		{
			_inAppPurchaseLogic.PurchaseContentPack1();
		}
		return null;
	}

	[TriggerableAction]
	public IEnumerator Toggle_Purchase()
	{
		_testPurchase = !_testPurchase;
		UpdateDebugText();
		if ((bool)_inAppPurchaseLogic)
		{
			_inAppPurchaseLogic.simulatePurchaseSuccess = _testPurchase;
		}
		return null;
	}

	[TriggerableAction]
	public IEnumerator Toggle_RedSuits_Unlocked()
	{
		CheckRedSuits();
		CheckForPurchases();
		UpdateDebugText();
		return null;
	}

	[TriggerableAction]
	public IEnumerator Toggle_RedSuits_Available()
	{
		if ((bool)_redSuitsLogic)
		{
			_redSuitsLogic.available = !_redSuitsLogic.available;
		}
		UpdateDebugText();
		CheckRedSuits();
		return null;
	}

	private void UpdateDebugText()
	{
		if ((bool)_redSuitsLogic && debugText_RedSuits_Available.gameObject.activeInHierarchy)
		{
			debugText_RedSuits_Available.SetText("RedSuits Available: " + _redSuitsLogic.available);
		}
		if (debugText_Purchase_Success.gameObject.activeInHierarchy)
		{
			debugText_Purchase_Success.SetText("Test Purchase: " + _testPurchase);
		}
	}

	[TriggerableAction]
	public IEnumerator CheckRedSuits()
	{
		bool flag = false;
		if ((bool)_inAppPurchaseLogic)
		{
			flag = _redSuitsLogic.IsAvailable();
			_redSuitsLogic.HasPurchasedRedSuits();
		}
		if (flag)
		{
			show_RedSuits.TriggerActions();
		}
		else
		{
			show_Blue.TriggerActions();
		}
		worldSelectBody_UILayout.Layout();
		return null;
	}

	[TriggerableAction]
	public IEnumerator CheckForPurchases()
	{
		if ((bool)_inAppPurchaseLogic)
		{
			expansion_unlocked = _inAppPurchaseLogic.HasPurchasedContentPack1();
		}
		if (expansion_unlocked)
		{
			hide_Purchase_Expansion.TriggerActions();
		}
		else
		{
			show_Purchase_Expansion.TriggerActions();
			if ((bool)_inAppPurchaseManager)
			{
				expansionPriceText.text = _inAppPurchaseManager.IapInfo[Iap_Type.ContentPack1].price;
			}
		}
		return null;
	}

	[TriggerableAction]
	public IEnumerator Purchase_Expansion_Successful()
	{
		expansion_unlocked = true;
		CheckForPurchases();
		popup_Purchase_Successful.TriggerActions();
		return null;
	}

	[TriggerableAction]
	public IEnumerator Purchase_RedSuits_Successful()
	{
		RefreshRedSuitsInfoButtonText();
		popup_Purchase_Successful.TriggerActions();
		return null;
	}

	[TriggerableAction]
	public IEnumerator PurchaseFailed()
	{
		popup_Purchase_Failed.TriggerActions();
		return null;
	}

	[TriggerableAction]
	public IEnumerator PurchaseNoConnection()
	{
		popup_NoConnection.TriggerActions();
		return null;
	}

	[TriggerableAction]
	public IEnumerator HidePopUp()
	{
		popup_Hide.TriggerActions();
		return null;
	}

	[TriggerableAction]
	public IEnumerator TouchedOriginal()
	{
		if (LevelManager.CurrentLevel == LevelName.LevelSelect)
		{
			current_World_Reselected.TriggerActions();
		}
		else
		{
			go_To_Original.TriggerActions();
		}
		return null;
	}

	[TriggerableAction]
	public IEnumerator TouchedExpansion()
	{
		if (!expansion_unlocked)
		{
			show_Purchasing.TriggerActions();
		}
		else if (LevelManager.CurrentLevel == LevelName.LevelSelectExpansion)
		{
			current_World_Reselected.TriggerActions();
		}
		else if (!LevelManager.Instance.HasShownLevelUnlock(LevelName.Volcano))
		{
			play_Volcano.TriggerActions();
		}
		else
		{
			go_To_Expansion.TriggerActions();
		}
		return null;
	}

	[TriggerableAction]
	public IEnumerator TouchedRedSuitsInfo()
	{
		if (_redSuitsLogic.HasPurchasedRedSuits() || _testPurchase)
		{
			go_To_RedSuits.TriggerActions();
		}
		else
		{
			go_To_Blue.TriggerActions();
		}
		return null;
	}

	public void RefreshRedSuitsInfoButtonText()
	{
		if (_redSuitsLogic.HasPurchasedRedSuits())
		{
			redSuitsPriceText.text = LocalisationManager.Instance.LocaliseString("$(play_redsuits)");
		}
		else if (_redSuitsLogic.showPrice)
		{
			redSuitsPriceText.text = _inAppPurchaseManager.IapInfo[Iap_Type.RedSuits].price;
		}
		else
		{
			redSuitsPriceText.text = LocalisationManager.Instance.LocaliseString("$(check_availability)");
		}
	}

	[TriggerableAction]
	public IEnumerator TouchedRedSuitsBuy()
	{
		if (_redSuitsLogic.HasPurchasedRedSuits())
		{
			go_To_RedSuits.TriggerActions();
		}
		else
		{
			show_Purchasing.TriggerActions();
		}
		return null;
	}

	[TriggerableAction]
	public IEnumerator ShowCrossSellBusyIndicator()
	{
		_isShowingCrossSellBusyIndicator = true;
		crossSellLinkIcon.gameObject.SetActive(value: false);
		crossSellLinkSpinner.gameObject.SetActive(value: true);
		crossSellLinkIcon.GetComponentInParent<UIButton>().isEnabled = false;
		return null;
	}

	public void OnApplicationFocus(bool hasFocus)
	{
		if (hasFocus && _isShowingCrossSellBusyIndicator)
		{
			_isShowingCrossSellBusyIndicator = false;
			crossSellLinkIcon.gameObject.SetActive(value: true);
			crossSellLinkSpinner.gameObject.SetActive(value: false);
			crossSellLinkIcon.GetComponentInParent<UIButton>().isEnabled = true;
		}
	}
}
