using UnityEngine;

public class ParallaxShifter : MonoBehaviour
{
	private Vector3 initialLocalPosition;

	private Vector3 initialWorldPosition;

	public float depth;

	private float depthX;

	private float depthY;

	private Vector3 offset;

	public bool useOffsetX = true;

	public bool useOffsetY = true;

	private void Awake()
	{
		initialLocalPosition = base.transform.localPosition;
		initialWorldPosition = base.transform.position;
		depth *= 0.001f;
		depthX = (useOffsetX ? depth : 0f);
		depthY = (useOffsetY ? depth : 0f);
	}

	private void LateUpdate()
	{
		offset = (GameScene.WorldToPanPoint(base.transform.parent.position) - GameScene.WorldToPanPoint(Camera.main.transform.position)) * 41f;
		offset = new Vector3(offset.x * depthX, offset.y * depthY, 0f);
		base.transform.localPosition = initialLocalPosition + offset;
	}
}
