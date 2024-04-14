using UnityEngine;

public class RenderDebugItem
{
	public float timeout = -1f;

	public float timestamp;

	public bool onScreen;

	public LineRenderer lineRenderer;

	public RenderDebugItem()
	{
		timestamp = Time.time;
	}

	public virtual void Draw()
	{
	}

	public virtual void UpdateLineRenderer()
	{
	}
}
