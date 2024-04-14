using UnityEngine;

public class PlantBlockTrigger : MonoBehaviour
{
	public Dragger dragger;

	public bool blockReady;

	private void Update()
	{
		blockReady = !dragger.snapping && !dragger.dragging && dragger.transform.position == base.transform.position;
	}
}
