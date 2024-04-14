using UnityEngine;

public class BerryPositionTrigger : MonoBehaviour
{
	public Rigidbody fruit;

	public Dragger dragger;

	public bool berryCaught;

	private void Update()
	{
		if (!berryCaught && fruit.useGravity && Mathf.Abs(fruit.velocity.y) == 0f && Vector3.Distance(fruit.transform.position, base.transform.position) < 1f)
		{
			berryCaught = true;
			dragger.enabled = false;
		}
	}
}
