using UnityEngine;

public class ContinuousRotator : MonoBehaviour
{
	public float speed = 5f;

	private void Update()
	{
		base.transform.Rotate(Time.deltaTime * speed, 0f, 0f);
	}
}
