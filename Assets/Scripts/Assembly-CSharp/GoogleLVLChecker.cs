using UnityEngine;

public class GoogleLVLChecker : MonoBehaviour
{
	public string publicKey_Base64 = "Your google play public key here";

	public string publicKey_Modulus_Base64 = "Modulus_Base64";

	public string publicKey_Exponent_Base64 = "Exponent_Base64";

	private const int MAX_CACHED_DAYS_ALLOWED = 30;
}
