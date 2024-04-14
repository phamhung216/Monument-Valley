using UnityEngine;

public class PhysicsIceCube : MonoBehaviour
{
	public float lowerTheshold = -10f;

	public Vector3 respawnPosition;

	private Rigidbody _rigidbody;

	private void Start()
	{
		respawnPosition = new Vector3(-1f, 9.5f, -0.5f);
		_rigidbody = GetComponent<Rigidbody>();
	}

	private void Update()
	{
		if (base.transform.position.y < lowerTheshold)
		{
			base.transform.position = respawnPosition + new Vector3(Random.Range(-2, 2), 0f, Random.Range(-2, 2));
			_rigidbody.velocity = Vector3.zero;
		}
	}
}
