using UnityEngine;

public class Stabilize : MonoBehaviour
{
	private void LateUpdate()
	{
		base.transform.rotation = Quaternion.identity;
	}
}
