using UnityEngine;

public class NavBlockerPlane : MonoBehaviour
{
	public float width = 1f;

	public float height = 1f;

	private void Start()
	{
		if (base.transform.localScale != Vector3.one)
		{
			D.Error("NavBlockerPlane must have scale == 1", base.gameObject);
		}
	}

	private void Update()
	{
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.matrix = base.gameObject.transform.localToWorldMatrix;
		Gizmos.DrawWireCube(new Vector3(0f, 0f, 0f), new Vector3(width, height, 0f));
		Gizmos.matrix = Matrix4x4.identity;
	}

	public bool DoesBlockConnection(NavBrushComponent brushA, NavBrushComponent brushB)
	{
		Vector3 vector = base.transform.InverseTransformPoint(brushA.transform.position);
		Vector3 vector2 = base.transform.InverseTransformPoint(brushB.transform.position);
		if (Mathf.Sign(vector.z) != Mathf.Sign(vector2.z))
		{
			if (Mathf.Abs(vector.z) < 0.01f || Mathf.Abs(vector2.z) < 0.01f)
			{
				return false;
			}
			Vector3 vector3 = vector2 - vector;
			Vector3 vector4 = vector - vector3 * (vector.z / vector3.z);
			if ((0f - width) / 2f < vector4.x && vector4.x < width / 2f && (0f - height) / 2f < vector4.y && vector4.y < height / 2f)
			{
				return true;
			}
		}
		return false;
	}
}
