using UnityEngine;

public class LineSegment : MonoBehaviour
{
	public LineRenderer lineRenderer;

	public Transform point1;

	public Transform point2;

	private void LateUpdate()
	{
		if ((bool)lineRenderer)
		{
			lineRenderer.SetPosition(0, point1.position);
			lineRenderer.SetPosition(1, point2.position);
		}
	}
}
