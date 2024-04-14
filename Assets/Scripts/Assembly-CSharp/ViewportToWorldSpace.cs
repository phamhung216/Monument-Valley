using UnityCommon;
using UnityEngine;

public class ViewportToWorldSpace : MonoBehaviour
{
	public bool matchX;

	public float xPos;

	public bool matchY;

	public float yPos;

	public EditorButton updatePosition = new EditorButton("UpdatePosition");

	public void Start()
	{
		UpdatePosition();
	}

	public void UpdatePosition()
	{
		Vector3 position = Camera.main.WorldToViewportPoint(base.transform.position);
		if (matchX)
		{
			position.x = xPos;
		}
		if (matchY)
		{
			position.y = yPos;
		}
		base.transform.position = Camera.main.ViewportToWorldPoint(position);
	}
}
