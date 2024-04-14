using UnityEngine;

public class LSPortal : MonoBehaviour
{
	public Rotatable sourceRotator;

	public float visibleAngle;

	public float visibleRange = 90f;

	private Vector3 inactivePosition = new Vector3(0f, 200f, 0f);

	private void Update()
	{
		if (Mathf.Abs(sourceRotator.currentAngle - visibleAngle) < visibleRange)
		{
			base.transform.localPosition = Vector3.zero;
		}
		else
		{
			base.transform.localPosition = inactivePosition;
		}
	}
}
