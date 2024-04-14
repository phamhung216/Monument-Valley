using UnityEngine;

public class ConstellationLine : MonoBehaviour
{
	public LineRenderer lineRenderer;

	private LineRenderer[] allLineRenderers;

	public float pixelWidth = 1f;

	public Transform[] points;

	public Material lineRendererMaterial;

	private float lineRendererWidth;

	public bool loop;

	public bool useWorldSpace;

	public bool updateEveryFrame;

	public float panSpeedX;

	public TexturePanner texturePanner;

	public MaterialInstantiator matInst;

	private void Start()
	{
		lineRendererWidth = Camera.main.orthographicSize * 2f / (float)Screen.height * pixelWidth;
		if ((bool)matInst)
		{
			lineRendererMaterial = matInst.instantiatedMaterial;
		}
		else
		{
			lineRendererMaterial = Object.Instantiate(lineRendererMaterial);
		}
		if ((bool)texturePanner)
		{
			texturePanner.SetPanMat(lineRendererMaterial);
		}
		if (points.Length > 1)
		{
			for (int i = 0; i < points.Length - 1; i++)
			{
				CreateNewLineSegment(points[i], points[i + 1]);
			}
		}
		if (loop)
		{
			CreateNewLineSegment(points[points.Length - 1], points[0]);
		}
	}

	public LineRenderer CreateNewLineSegment(Transform pointA, Transform pointB)
	{
		GameObject gameObject = new GameObject();
		gameObject.name = "LineSegment";
		gameObject.transform.parent = base.transform.parent;
		gameObject.transform.localPosition = Vector3.zero;
		LineRenderer lineRenderer = (LineRenderer)gameObject.AddComponent(typeof(LineRenderer));
		lineRenderer.startWidth = lineRendererWidth;
		lineRenderer.endWidth = lineRendererWidth;
		lineRenderer.material = lineRendererMaterial;
		if (useWorldSpace)
		{
			lineRenderer.useWorldSpace = true;
			lineRenderer.SetPosition(0, pointA.position);
			lineRenderer.SetPosition(1, pointB.position);
			LineSegment obj = (LineSegment)gameObject.AddComponent(typeof(LineSegment));
			obj.lineRenderer = lineRenderer;
			obj.point1 = pointA;
			obj.point2 = pointB;
		}
		else
		{
			lineRenderer.useWorldSpace = false;
			lineRenderer.SetPosition(0, pointA.localPosition);
			lineRenderer.SetPosition(1, pointB.localPosition);
		}
		return lineRenderer;
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.blue;
		for (int i = 0; i < points.Length - 1; i++)
		{
			Gizmos.DrawLine(points[i].position, points[i + 1].position);
		}
	}
}
