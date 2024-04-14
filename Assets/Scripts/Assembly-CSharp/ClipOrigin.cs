using UnityEngine;

public class ClipOrigin : MonoBehaviour
{
	public Material clipMaterial;

	public string clipPlanePropertyName;

	private void LateUpdate()
	{
		Vector3 forward = base.transform.forward;
		forward.Normalize();
		Vector4 value = new Vector4(forward.x, forward.y, forward.z, 0f);
		value.w = Vector3.Dot(forward, base.transform.position);
		clipMaterial.SetVector(clipPlanePropertyName, value);
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawRay(new Ray(base.transform.position, base.transform.forward));
		Color color = Gizmos.color;
		color.a = 0.25f;
		Gizmos.color = color;
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.DrawCube(Vector3.zero, new Vector3(10f, 10f, 0f));
		Gizmos.matrix = Matrix4x4.identity;
	}
}
