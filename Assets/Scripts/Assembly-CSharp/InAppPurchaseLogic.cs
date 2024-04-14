using System;
using System.Collections;
using UnityEngine;

public class InAppPurchaseLogic : MonoBehaviour
{
	public enum SimulatedPurchasedState
	{
		None = 0,
		Purchased = 1,
		NotPurchased = 2
	}

	private WorldSelectLogic _worldSelectLogic;

	private InAppPurchaseManager _inAppPurchaseManager;

	public const string PurchasedContentPack1Key = "PurchasedContentPack1";

	public UIText restorePurchaseText;

	public bool simulatePurchaseSuccess;

	public bool simulatePurchaseFailed;

	public bool simulateRestorePurchasesSuccess;

	public bool simulateRestorePurchasesFailed;

	public SimulatedPurchasedState contentPack1SimulatedState;

	private RedSuitsLogic _redSuitsLogic;

	private void Start()
	{
		_inAppPurchaseManager = InAppPurchaseManager.Instance;
		_redSuitsLogic = UnityEngine.Object.FindObjectOfType<RedSuitsLogic>();
		InAppPurchaseManager.onPurchaseSuccessful += ProductPurchasedSuccess;
		InAppPurchaseManager.onPurchaseFailed += ProductPurchasedFailed;
		InAppPurchaseManager.onPurchaseCancelled += ProductPurchasedCancelled;
		InAppPurchaseManager.onRestoreFailed += RestorePurchaseFailed;
		InAppPurchaseManager.onRestoreSuccess += RestorePurchaseSuccess;
		SetUp();
	}

	private void OnDisable()
	{
		InAppPurchaseManager.onPurchaseSuccessful -= ProductPurchasedSuccess;
		InAppPurchaseManager.onPurchaseFailed -= ProductPurchasedFailed;
		InAppPurchaseManager.onPurchaseCancelled -= ProductPurchasedCancelled;
		InAppPurchaseManager.onRestoreFailed -= RestorePurchaseFailed;
		InAppPurchaseManager.onRestoreSuccess -= RestorePurchaseSuccess;
	}

	private void SetUp()
	{
		if (_worldSelectLogic == null)
		{
			_worldSelectLogic = UnityEngine.Object.FindObjectOfType<WorldSelectLogic>();
		}
	}

	public bool HasPurchasedContentPack1()
	{
		return true;
	}

	[TriggerableAction]
	public IEnumerator PurchaseContentPack1()
	{
		if (!HasPurchasedContentPack1())
		{
			if (simulatePurchaseSuccess)
			{
				Invoke("SimulatePurchaseContentPackSuccess", 1f);
			}
			else if (simulatePurchaseFailed)
			{
				Invoke("SimulatePurchaseContentPackFailed", 1f);
			}
			else
			{
				_inAppPurchaseManager.RequestPurchaseContent(Iap_Type.ContentPack1);
			}
		}
		return null;
	}

	[TriggerableAction]
	public IEnumerator RestorePurchases()
	{
		if ((bool)restorePurchaseText)
		{
			restorePurchaseText.text = LocalisationManager.Instance.LocaliseString("$(iap_restoring)");
		}
		if (simulateRestorePurchasesSuccess)
		{
			Invoke("SimulateRestorePurchasesSuccess", 1f);
		}
		else if (simulateRestorePurchasesFailed)
		{
			Invoke("SimulateRestorePurchasesFailed", 1f);
		}
		else
		{
			_inAppPurchaseManager.RestorePurchases();
		}
		return null;
	}

	private void SimulatePurchaseContentPackSuccess()
	{
		ProductPurchasedSuccess(_inAppPurchaseManager.IapInfo[Iap_Type.ContentPack1].productIdentifier);
	}

	private void SimulatePurchaseContentPackFailed()
	{
		ProductPurchasedFailed(_inAppPurchaseManager.IapInfo[Iap_Type.ContentPack1].productIdentifier);
	}

	private void SimulateRestorePurchasesSuccess()
	{
		RestorePurchaseSuccess("ContentPack01");
	}

	private void SimulateRestorePurchasesFailed()
	{
		RestorePurchaseFailed(_inAppPurchaseManager.IapInfo[Iap_Type.ContentPack1].productIdentifier);
	}

	private void ProductPurchasedSuccess(string productIdentifier)
	{
		SetUp();
		switch (_inAppPurchaseManager.TypeFromString(productIdentifier))
		{
		case Iap_Type.ContentPack1:
			SetContentPack1Purchased();
			_worldSelectLogic.Purchase_Expansion_Successful();
			Analytics.LogEvent("Purchased_ForgottenShores");
			break;
		case Iap_Type.RedSuits:
			_redSuitsLogic.SetPurchased();
			_worldSelectLogic.Purchase_RedSuits_Successful();
			break;
		}
		UserDataController.Instance.Save();
	}

	public static void SetContentPack1Purchased()
	{
		SharedPlayerPrefs.SetInt("PurchasedContentPack1", 1);
	}

	private void ProductPurchasedFailed(string productIdentifier)
	{
		SetUp();
		if (productIdentifier == "NoConnection")
		{
			_worldSelectLogic.PurchaseNoConnection();
		}
		else
		{
			_worldSelectLogic.PurchaseFailed();
		}
	}

	private void ProductPurchasedCancelled(string productIdentifier)
	{
		SetUp();
		_worldSelectLogic.PurchaseFailed();
	}

	private void RestorePurchaseSuccess(string productIds)
	{
		SetRestoreText(_success: true);
		string[] array = productIds.Split(new string[1] { "," }, StringSplitOptions.None);
		for (int i = 0; i < array.Length; i++)
		{
			if (_inAppPurchaseManager.TypeFromString(array[i]) == Iap_Type.ContentPack1)
			{
				SharedPlayerPrefs.SetInt("PurchasedContentPack1", 1);
			}
		}
		GameScene.instance.eventHandlers[SceneEvent.EnableInput].Send();
	}

	public void SetRestoreText(bool _success)
	{
		if ((bool)restorePurchaseText)
		{
			if (_success)
			{
				restorePurchaseText.text = LocalisationManager.Instance.LocaliseString("$(iap_success)");
			}
			else
			{
				restorePurchaseText.text = LocalisationManager.Instance.LocaliseString("$(iap_fail)");
			}
		}
	}

	private void RestorePurchaseFailed(string error)
	{
		SetRestoreText(_success: false);
		GameScene.instance.eventHandlers[SceneEvent.EnableInput].Send();
	}
}
