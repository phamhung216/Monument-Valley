using UnityEngine;

public class DepthTrigger : MonoBehaviour
{
	public static Vector3 awayFromCamera = new Vector3(1f, -1f, 1f);

	public float depthChange;

	private void OnCollisionEnter(Collision collision)
	{
	}

	private void OnTriggerEnter(Collider other)
	{
		Dragger component = other.GetComponent<Dragger>();
		if (component != null)
		{
			component.EndSnapping();
			other.transform.Translate(GetOffset(depthChange));
		}
	}

	public static Vector3 GetOffset(float depthOffset)
	{
		return awayFromCamera * depthOffset;
	}
}
