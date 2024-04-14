using UnityEngine;

public class FishPondWall : MonoBehaviour
{
	public Vector2 size = new Vector2(1f, 1f);

	public bool hasConvexCorners;

	private Plane _plane;

	public Plane plane => _plane;

	private void Start()
	{
		UpdatePlane();
	}

	private void Update()
	{
	}

	public void UpdatePlane()
	{
		_plane = new Plane(base.transform.forward, base.transform.position);
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = (hasConvexCorners ? Color.Lerp(Color.yellow, Color.red, 0.5f) : Color.red);
		Gizmos.matrix = base.gameObject.transform.localToWorldMatrix;
		Gizmos.DrawWireCube(new Vector3(0f, 0f, 0f), new Vector3(size.x, size.y, 0f));
		Gizmos.matrix = Matrix4x4.identity;
		Gizmos.color = Color.red;
		Gizmos.DrawRay(base.transform.position, 1f * base.transform.forward);
	}
}
