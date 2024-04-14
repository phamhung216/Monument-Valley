using UnityEngine;

public class FruitDropper : InteractiveItem
{
	private Vector3 startPosition;

	private GameObject seaPlane;

	private SphereCollider sphere;

	private void Awake()
	{
		seaPlane = GameObject.Find("SeaPlane");
		sphere = GetComponent<SphereCollider>();
		startPosition = base.transform.position;
		ResetPosition();
	}

	private void ResetPosition()
	{
		sphere.radius = 4f;
		GetComponent<Rigidbody>().useGravity = false;
		GetComponent<Rigidbody>().velocity = Vector3.zero;
		base.transform.position = startPosition;
	}

	public override void Trigger()
	{
		base.Trigger();
		GetComponent<Rigidbody>().useGravity = true;
		sphere.radius = 1f;
	}

	public void Respawn()
	{
		Invoke("ResetPosition", 4f);
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other == seaPlane.GetComponent<Collider>())
		{
			seaPlane.GetComponent<SeaTap>().Splash(new Vector3(base.transform.position.x, 1f, base.transform.position.z));
			Respawn();
		}
	}
}
