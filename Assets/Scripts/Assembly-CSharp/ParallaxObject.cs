using UnityEngine;

public class ParallaxObject : MonoBehaviour
{
	private Vector3 startPosition;

	public PanController panController;

	public float offsetMultiplier;

	private void Start()
	{
		startPosition = base.transform.position;
	}

	private void Update()
	{
		base.transform.position = startPosition + offsetMultiplier * PanController.Pan(panController.offset);
	}
}
