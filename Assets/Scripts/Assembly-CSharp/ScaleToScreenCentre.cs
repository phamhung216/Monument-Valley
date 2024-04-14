using UnityEngine;

public class ScaleToScreenCentre : MonoBehaviour
{
	public Transform target;

	public float distanceA;

	public float distanceB;

	public float scaleA;

	public float scaleB;

	private void Update()
	{
		float t = Mathf.Clamp01((Mathf.Abs(Camera.main.WorldToScreenPoint(target.position).y - (float)(Screen.height / 2)) - distanceA) / (distanceB - distanceA));
		float num = Mathf.Lerp(scaleA, scaleB, t);
		target.localScale = new Vector3(num, num, num);
	}
}
