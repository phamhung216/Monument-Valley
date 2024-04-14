using UnityEngine;

public class CameraPanBounds : MonoBehaviour
{
	public Vector2 size = new Vector2(0f, 0f);

	private void Awake()
	{
		if (size.magnitude == 0f)
		{
			OnScreenSizeChanged();
		}
	}

	public void OnScreenSizeChanged()
	{
		float x = Mathf.Min(2.3333333f, Camera.main.aspect);
		size = 2f * CameraAspectController.CalculateReferenceOrthographicSize() * new Vector2(x, 1f);
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.yellow;
		Gizmos.matrix = base.gameObject.transform.localToWorldMatrix;
		Gizmos.DrawWireCube(new Vector3(0f, 0f, 0f), new Vector3(size.x, size.y, 0f));
		Gizmos.matrix = Matrix4x4.identity;
	}
}
