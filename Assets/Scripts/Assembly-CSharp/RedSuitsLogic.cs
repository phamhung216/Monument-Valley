using System;
using System.Collections;
using UnityEngine;

public class RedSuitsLogic : MonoBehaviour
{
	public bool available;

	private const string PurchasedRedSuitsKey = "PurchasedRedSuits";

	private string promotionEndDate = "2014-12-08";

	private string redSuitsBuildMin = "2.1";

	private string redSuitsBuildMax = "2.1.41";

	private bool _doRedSuitsCheck;

	private bool _redSuitsCheckInProgress;

	private InAppPurchaseLogic _inAppPurchaseLogic;

	private WorldSelectLogic _worldSelectLogic;

	private bool _showPrice;

	public bool showPrice => _showPrice;

	public bool redSuitsCheckInProgress
	{
		get
		{
			return _redSuitsCheckInProgress;
		}
		set
		{
			_redSuitsCheckInProgress = value;
		}
	}

	private void Start()
	{
	}

	private void OnDisable()
	{
		InAppPurchaseManager.onPurchaseSuccessful -= ProductPurchasedSuccess;
		InAppPurchaseManager.onRestoreFailed -= RestorePurchaseFailed;
		InAppPurchaseManager.onRestoreSuccess -= RestorePurchaseSuccess;
	}

	public void SetUp()
	{
		_showPrice = false;
		if (_worldSelectLogic == null)
		{
			_worldSelectLogic = UnityEngine.Object.FindObjectOfType<WorldSelectLogic>();
		}
	}

	public bool HasPurchasedRedSuits()
	{
		return false;
	}

	public void SetPurchased()
	{
		SharedPlayerPrefs.SetInt("PurchasedRedSuits", 1);
	}

	public bool IsAvailable()
	{
		return false;
	}

	[TriggerableAction]
	public IEnumerator PurchaseRedSuits()
	{
		return null;
	}

	private void SimulatePurchaseRedSuitsSuccess()
	{
		ProductPurchasedSuccess(InAppPurchaseManager.Instance.IapInfo[Iap_Type.RedSuits].productIdentifier);
	}

	private void SimulatePurchaseRedSuitsFailed()
	{
		ProductPurchasedFailed(InAppPurchaseManager.Instance.IapInfo[Iap_Type.RedSuits].productIdentifier);
	}

	private void ProductPurchasedSuccess(string productIdentifier)
	{
		if (InAppPurchaseManager.Instance.TypeFromString(productIdentifier) == Iap_Type.RedSuits)
		{
			SetPurchased();
			_worldSelectLogic.Purchase_RedSuits_Successful();
			UserDataController.Instance.Save();
		}
	}

	private void ProductPurchasedFailed(string productIdentifier)
	{
		if (productIdentifier == "NoConnection")
		{
			_worldSelectLogic.PurchaseNoConnection();
		}
		else
		{
			_worldSelectLogic.PurchaseFailed();
		}
	}

	public bool IsInPromotionPeriod(DateTime time)
	{
		string format = "yyyy-MM-dd";
		bool result = false;
		DateTime t = DateTime.ParseExact(promotionEndDate, format, null);
		if (DateTime.Compare(time, t) <= 0)
		{
			result = true;
		}
		return result;
	}

	public bool HasPurchasedRedSuitsPromotionBuild()
	{
		bool result = false;
		string appOriginalPurchaseVersion = InAppPurchaseManager.Instance.GetAppOriginalPurchaseVersion();
		if (appOriginalPurchaseVersion != "")
		{
			result = CompareVersion(redSuitsBuildMin, appOriginalPurchaseVersion) <= 0 && CompareVersion(redSuitsBuildMax, appOriginalPurchaseVersion) >= 0;
		}
		return result;
	}

	private int CompareVersion(string a, string b)
	{
		string[] array = a.Split('.');
		string[] array2 = b.Split('.');
		for (int i = 0; i < array.Length; i++)
		{
			if (array2.Length <= i)
			{
				return 1;
			}
			int num = int.Parse(array[i]);
			int num2 = int.Parse(array2[i]);
			if (num > num2)
			{
				return 1;
			}
			if (num < num2)
			{
				return -1;
			}
			if (i == array.Length - 1 && array.Length == array2.Length && num == num2)
			{
				return 0;
			}
		}
		return -1;
	}

	public void PerformRedSuitCheck(string param)
	{
		if (param == "yes")
		{
			_redSuitsCheckInProgress = true;
			if (!DeviceUtils.HasNetworkConnection())
			{
				ProductPurchasedFailed("NoConnection");
			}
			else
			{
				InAppPurchaseManager.Instance.RestorePurchases();
			}
		}
		else
		{
			_redSuitsCheckInProgress = false;
			_worldSelectLogic.HidePopUp();
		}
	}

	public void RestorePurchaseSuccess(string productIds)
	{
		_showPrice = false;
		string[] array = productIds.Split(new string[1] { "," }, StringSplitOptions.None);
		for (int i = 0; i < array.Length; i++)
		{
			if (InAppPurchaseManager.Instance.TypeFromString(array[i]) == Iap_Type.RedSuits)
			{
				SetPurchased();
			}
		}
		if (HasPurchasedRedSuitsPromotionBuild())
		{
			SetPurchased();
		}
		else
		{
			_showPrice = true;
		}
		if (_redSuitsCheckInProgress)
		{
			_redSuitsCheckInProgress = false;
			_worldSelectLogic.RefreshRedSuitsInfoButtonText();
			_worldSelectLogic.HidePopUp();
		}
	}

	public void RestorePurchaseFailed(string error)
	{
		if (_redSuitsCheckInProgress)
		{
			_showPrice = false;
			_redSuitsCheckInProgress = false;
			_worldSelectLogic.HidePopUp();
			if (HasPurchasedRedSuitsPromotionBuild())
			{
				SetPurchased();
			}
			else if (error.Equals("no_transactions"))
			{
				_showPrice = true;
			}
			else if (error.Equals("NoConnection"))
			{
				ProductPurchasedFailed("NoConnection");
			}
			_worldSelectLogic.RefreshRedSuitsInfoButtonText();
		}
		else if (error.Equals("no_transactions") && HasPurchasedRedSuitsPromotionBuild())
		{
			SetPurchased();
			InAppPurchaseLogic component = GetComponent<InAppPurchaseLogic>();
			if ((bool)component)
			{
				component.SetRestoreText(_success: true);
			}
		}
	}
}
