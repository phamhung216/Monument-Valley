using System.Collections.Generic;

public class StorePlatform
{
	public virtual void Initialise(Dictionary<Iap_Type, IAP> products)
	{
	}

	public virtual void RequestProductData(string productIds)
	{
	}

	public virtual void PurchaseProduct(string productId)
	{
	}

	public virtual void RestorePurchase(string productId)
	{
	}

	public virtual bool CanMakePayment()
	{
		return false;
	}

	public virtual string GetAppOriginalPurchaseDate()
	{
		return "";
	}

	public virtual string GetAppOriginalPurchaseVersion()
	{
		return "";
	}

	public virtual void RefreshLocalReceipt()
	{
	}
}
