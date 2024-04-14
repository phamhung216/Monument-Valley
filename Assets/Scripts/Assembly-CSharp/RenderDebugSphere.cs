using UnityEngine;

public class RenderDebugSphere : RenderDebugItem
{
	public Vector3 position;

	public float radius;

	public Color color;

	public RenderDebugSphere(Vector3 position, Color color, float radius)
	{
		this.position = position;
		this.color = color;
		this.radius = radius;
	}

	public override void Draw()
	{
		Gizmos.color = color;
		Gizmos.DrawSphere(position, radius);
	}

	public override void UpdateLineRenderer()
	{
		Vector3 up = Camera.main.transform.up;
		Vector3 forward = Camera.main.transform.forward;
		int num = 12;
		lineRenderer.positionCount = num;
		lineRenderer.startWidth = 0.1f;
		lineRenderer.endWidth = 0.1f;
		lineRenderer.startColor = color;
		lineRenderer.endColor = color;
		lineRenderer.transform.position = position;
		for (int i = 0; i < num; i++)
		{
			Vector3 vector = Quaternion.AngleAxis((float)(i * 360) / (float)(num - 1), forward) * up;
			Vector3 vector2 = position + radius * vector;
			lineRenderer.SetPosition(i, vector2);
		}
	}
}
