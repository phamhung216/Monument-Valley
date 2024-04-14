using UnityEngine;

public class CopyCameraProperties : MonoBehaviour
{
	public Camera sourceCamera;

	public Transform targetPositionTransform;

	private Camera _localCamera;

	private void Start()
	{
		_localCamera = GetComponent<Camera>();
		_localCamera.farClipPlane = sourceCamera.farClipPlane;
		int cullingMask = _localCamera.cullingMask;
		cullingMask &= ~(1 << LayerMask.NameToLayer("GameUI"));
		_localCamera.cullingMask = cullingMask;
		if (!targetPositionTransform)
		{
			targetPositionTransform = base.transform;
		}
	}

	private void Update()
	{
		_localCamera.orthographicSize = sourceCamera.orthographicSize;
		targetPositionTransform.localPosition = sourceCamera.transform.localPosition;
	}
}
