using System;

[Serializable]
public class IAP
{
	public string productIdentifier;

	public string title;

	public string description;

	public string price;

	public string currencyCode;

	public IAP(string _productIdentifier)
	{
		productIdentifier = _productIdentifier;
		price = "...";
	}

	public void productFromString(string productString)
	{
		string[] array = productString.Split(new string[1] { "|" }, StringSplitOptions.None);
		if (productIdentifier == array[0])
		{
			title = array[1];
			description = array[2];
			price = (array[3] + " " + array[4]).Trim();
			currencyCode = array[4];
		}
	}
}
