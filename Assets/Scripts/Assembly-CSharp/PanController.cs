using UnityEngine;

public class PanController : MonoBehaviour
{
	public Vector2 offset;

	private Vector3 camStartPosition;

	private Vector3 camDragStartPosition;

	public float dragSensitivity;

	private Vector2 mouseDragStartPosition;

	private void Start()
	{
		camStartPosition = base.transform.position;
	}

	private void Update()
	{
		if (Input.GetMouseButtonDown(0))
		{
			mouseDragStartPosition = Input.mousePosition;
			camDragStartPosition = base.transform.position;
		}
		if (Input.GetMouseButton(0))
		{
			Vector2 v = (Vector2)Input.mousePosition - mouseDragStartPosition;
			v /= 0f - dragSensitivity;
			base.transform.position = camDragStartPosition + Pan(v);
			offset = base.transform.position - camStartPosition;
		}
		if (Input.GetMouseButtonUp(0))
		{
			offset = new Vector2(Mathf.Round(offset.x * 2f) / 2f, Mathf.Round(offset.y * 2f) / 2f);
			base.transform.position = camStartPosition + Pan(offset);
		}
	}

	public static Vector3 Pan(Vector2 v2)
	{
		return new Vector3(v2.x, v2.y, 0f - v2.x);
	}
}
