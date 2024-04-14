using UnityEngine;

public class NSidedMaskController : MonoBehaviour
{
	public NSidedMaskController otherMask;

	public Camera cam;

	private Vector3 _startPosition;

	private Vector3 _forward;

	public Vector3 startPosition => _startPosition;

	private void Start()
	{
		_startPosition = base.transform.localPosition;
		_forward = new Vector3(-1f, 0f, -1f).normalized;
	}

	private void LateUpdate()
	{
		base.transform.forward = _forward;
		base.transform.localPosition = _startPosition;
		float num = Vector3.Distance(cam.transform.position, base.transform.parent.TransformPoint(_startPosition));
		float num2 = Vector3.Distance(cam.transform.position, base.transform.parent.TransformPoint(otherMask.startPosition));
		if (num <= num2)
		{
			base.transform.Translate(_forward * 8f, Space.World);
		}
	}
}
