using UnityEngine;

public class LevelFlower : MonoBehaviour
{
	public Transform target;

	public float distanceA;

	public float distanceB;

	public float min;

	public float max;

	public Transform[] petals;

	private void Update()
	{
		float t = Mathf.Clamp01((Mathf.Abs(Camera.main.WorldToScreenPoint(target.position).x - (float)(Screen.width / 2)) - distanceA) / (distanceB - distanceA));
		Transform[] array = petals;
		foreach (Transform transform in array)
		{
			transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, Mathf.Lerp(min, max, t));
		}
	}
}
