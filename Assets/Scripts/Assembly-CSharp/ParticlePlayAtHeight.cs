using UnityEngine;

public class ParticlePlayAtHeight : MonoBehaviour
{
	private bool newState;

	[SerializeField]
	private bool above;

	public float targetHeight;

	public ParticleSystem ps;

	private void Start()
	{
		above = base.transform.position.y > targetHeight;
	}

	private void Update()
	{
		newState = base.transform.position.y > targetHeight;
		if (newState != above)
		{
			ps.Play();
		}
		above = newState;
	}
}
