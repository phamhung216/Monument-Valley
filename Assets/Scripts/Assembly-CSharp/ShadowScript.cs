using UnityEngine;

public class ShadowScript : MonoBehaviour
{
	private void LateUpdate()
	{
		base.transform.position = new Vector3(base.transform.parent.position.x, 0f, base.transform.parent.position.z);
	}
}
