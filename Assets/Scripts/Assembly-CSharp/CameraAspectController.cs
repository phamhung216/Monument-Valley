using UnityCommon;
using UnityEngine;

public class CameraAspectController : MonoBehaviour
{
	public const float MaxVisibleAspectRatio = 2.3333333f;

	private void Awake()
	{
		GetComponent<Camera>().orthographicSize = CalculateReferenceOrthographicSize();
	}

	public static float CalculateReferenceOrthographicSize()
	{
		GameObject gameObject = GameObject.Find("UICamera");
		float num = 1f;
		if (gameObject != null)
		{
			gameObject.GetComponent<UICamera>().EnsureScreenArchetypeSelected();
			num = gameObject.GetComponent<UICamera>().layoutWidth / 768f;
		}
		float num2 = 9.225f;
		if (OrientationOverrideManager.IsLandscape())
		{
			float num3 = 1.7777778f;
			return 33f * Mathf.Sqrt(2f) / num3 * 0.5f;
		}
		return num * (num2 * (float)Screen.height / (float)Screen.width);
	}

	private void OnDrawGizmos()
	{
		if (!(Camera.main.aspect < 2.3333333f))
		{
			Gizmos.color = Color.red;
			Gizmos.matrix = base.gameObject.transform.localToWorldMatrix;
			Gizmos.DrawLine(Camera.main.orthographicSize * new Vector3(-2.3333333f, 1f, 0f), Camera.main.orthographicSize * new Vector3(-2.3333333f, -1f, 0f));
			Gizmos.DrawLine(Camera.main.orthographicSize * new Vector3(2.3333333f, 1f, 0f), Camera.main.orthographicSize * new Vector3(2.3333333f, -1f, 0f));
			Gizmos.matrix = Matrix4x4.identity;
		}
	}
}
