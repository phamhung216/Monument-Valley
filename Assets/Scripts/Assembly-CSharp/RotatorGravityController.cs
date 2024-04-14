using UnityEngine;

public class RotatorGravityController : MonoBehaviour
{
	public GameObject gravityProvider;

	private static Vector3 diff = new Vector3(0f, 1f, -1f);

	private void Update()
	{
		_ = gravityProvider.transform.eulerAngles;
		float num = Vector3.Angle(gravityProvider.transform.up, diff);
		gravityProvider.SetActive(num <= 50f);
	}
}
