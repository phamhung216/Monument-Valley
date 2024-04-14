using UnityEngine;

public class RenderDebugLine : RenderDebugItem
{
	public Vector3 from;

	public Vector3 to;

	public Color color;

	public RenderDebugLine(Vector3 from, Vector3 to, Color color)
	{
		this.from = from;
		this.to = to;
		this.color = color;
	}

	public override void Draw()
	{
		Gizmos.color = color;
		Gizmos.DrawLine(from, to);
	}
}
