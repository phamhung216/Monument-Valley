using System;
using System.Collections.Generic;
using UnityEngine;

public class InAppPurchaseManager : MonoBehaviour
{
	public delegate void ProductDataReceivedEventHandler();

	public delegate void ProductPurchasedEventHandler(string productIdentifier);

	public delegate void RestorePurchaseEventHandler(string param);

	public delegate void AppPurchasedDateEvent(string param);

	private static InAppPurchaseManager _instance;

	private static Dictionary<Iap_Type, IAP> _iaps = new Dictionary<Iap_Type, IAP>();

	private static StorePlatform _storePlatform = null;

	private bool _receivedProductData;

	private bool _purchasingInProgress;

	private bool _restoringPurchasesInProgress;

	private Iap_Type _currPurchasingIAP;

	public const string no_connection_key = "NoConnection";

	public float timeOut = 20f;

	public static InAppPurchaseManager Instance => _instance;

	public Dictionary<Iap_Type, IAP> IapInfo => _iaps;

	public static event ProductDataReceivedEventHandler productListReceived;

	public static event ProductPurchasedEventHandler onPurchaseSuccessful;

	public static event ProductPurchasedEventHandler onPurchaseFailed;

	public static event ProductPurchasedEventHandler onPurchaseCancelled;

	public static event RestorePurchaseEventHandler onRestoreFailed;

	public static event RestorePurchaseEventHandler onRestoreSuccess;

	public static event AppPurchasedDateEvent OnAppPurchasedDate;

	private void Awake()
	{
		_instance = this;
		if (_storePlatform == null && _storePlatform != null)
		{
			_storePlatform.Initialise(_iaps);
		}
	}

	private void Start()
	{
		RequestProductData();
	}

	private void OnDestroy()
	{
		_instance = null;
	}

	public void RequestProductData()
	{
		if (_storePlatform == null || _receivedProductData || !DeviceUtils.HasNetworkConnection())
		{
			return;
		}
		string text = "";
		foreach (KeyValuePair<Iap_Type, IAP> iap in _iaps)
		{
			text = text + iap.Value.productIdentifier + ",";
		}
		text = text.Remove(text.Length - 1);
		_storePlatform.RequestProductData(text);
	}

	public void ProductsReceived(string param)
	{
		StopPurchaseTimeout();
		string[] array = param.Split(new string[1] { "|||" }, StringSplitOptions.None);
		for (int i = 0; i < array.Length; i++)
		{
			foreach (KeyValuePair<Iap_Type, IAP> iap in _iaps)
			{
				iap.Value.productFromString(array[i]);
			}
		}
		_receivedProductData = true;
		if (InAppPurchaseManager.productListReceived != null)
		{
			InAppPurchaseManager.productListReceived();
		}
		if (_purchasingInProgress)
		{
			_purchasingInProgress = false;
			RequestPurchaseContent(_currPurchasingIAP);
		}
	}

	public void ProductsRequestDidFail(string param)
	{
		StopPurchaseTimeout();
		if (_restoringPurchasesInProgress)
		{
			_restoringPurchasesInProgress = false;
			if (InAppPurchaseManager.onRestoreFailed != null)
			{
				InAppPurchaseManager.onRestoreFailed(param);
			}
		}
		if (_purchasingInProgress)
		{
			ProductPurchaseFailed(param);
		}
	}

	public void ProductPurchased(string param)
	{
		_purchasingInProgress = false;
		StopPurchaseTimeout();
		if (InAppPurchaseManager.onPurchaseSuccessful != null)
		{
			InAppPurchaseManager.onPurchaseSuccessful(param);
		}
	}

	public void ProductPurchaseFailed(string param)
	{
		_purchasingInProgress = false;
		StopPurchaseTimeout();
		if (InAppPurchaseManager.onPurchaseFailed != null)
		{
			InAppPurchaseManager.onPurchaseFailed(param);
		}
	}

	public void ProductPurchaseCancelled(string param)
	{
		_purchasingInProgress = false;
		StopPurchaseTimeout();
		if (InAppPurchaseManager.onPurchaseCancelled != null)
		{
			InAppPurchaseManager.onPurchaseCancelled(param);
		}
	}

	public void RestoreCompletedTransactionFailed(string error)
	{
		_restoringPurchasesInProgress = false;
		StopPurchaseTimeout();
		if (InAppPurchaseManager.onRestoreFailed != null)
		{
			InAppPurchaseManager.onRestoreFailed(error);
		}
	}

	public void RestoreCompletedTransactions(string param)
	{
		_restoringPurchasesInProgress = false;
		StopPurchaseTimeout();
		if (InAppPurchaseManager.onRestoreSuccess != null)
		{
			InAppPurchaseManager.onRestoreSuccess(param);
		}
	}

	public void RequestPurchaseContent(Iap_Type _type)
	{
		if (_storePlatform == null || _purchasingInProgress)
		{
			return;
		}
		_currPurchasingIAP = _type;
		_purchasingInProgress = true;
		string productIdentifier = _iaps[_currPurchasingIAP].productIdentifier;
		if (!DeviceUtils.HasNetworkConnection())
		{
			ProductPurchaseFailed("NoConnection");
			return;
		}
		SetPurchaseTimeout();
		if (!_receivedProductData)
		{
			RequestProductData();
		}
		else
		{
			_storePlatform.PurchaseProduct(productIdentifier);
		}
	}

	public void RestorePurchases()
	{
		if (_storePlatform != null && !_restoringPurchasesInProgress)
		{
			_restoringPurchasesInProgress = true;
			if (_storePlatform != null)
			{
				SetPurchaseTimeout();
				string productIdentifier = _iaps[Iap_Type.ContentPack1].productIdentifier;
				_storePlatform.RestorePurchase(productIdentifier);
			}
		}
	}

	public void AppOriginalPurchaseDate(string _param)
	{
		if (InAppPurchaseManager.OnAppPurchasedDate != null)
		{
			InAppPurchaseManager.OnAppPurchasedDate(_param);
		}
	}

	public Iap_Type TypeFromString(string _id)
	{
		Iap_Type result = Iap_Type.ContentPack1;
		foreach (KeyValuePair<Iap_Type, IAP> iap in _iaps)
		{
			if (iap.Value.productIdentifier == _id)
			{
				result = iap.Key;
				break;
			}
		}
		return result;
	}

	public string GetAppOriginalPurchaseVersion()
	{
		if (_storePlatform != null)
		{
			return _storePlatform.GetAppOriginalPurchaseVersion();
		}
		return "";
	}

	private void SetPurchaseTimeout()
	{
		if (!IsInvoking("StopPurchase"))
		{
			Invoke("StopPurchase", timeOut);
		}
	}

	private void StopPurchaseTimeout()
	{
		CancelInvoke("StopPurchase");
	}

	private void StopPurchase()
	{
		if (_restoringPurchasesInProgress)
		{
			RestoreCompletedTransactionFailed("NoConnection");
		}
		if (_purchasingInProgress)
		{
			ProductPurchaseFailed("NoConnection");
		}
	}

	public void RefreshLocalReceipt()
	{
		if (_storePlatform != null)
		{
			_storePlatform.RefreshLocalReceipt();
		}
	}
}
