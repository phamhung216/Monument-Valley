using System.Collections;
using UnityEngine;

public class CameraLookAtPoint : GameTouchable
{
	public CameraPanBounds panBounds;

	public float panToDuration = 2f;

	public CameraPanController.Mode cameraMode = CameraPanController.Mode.Pan;

	public CameraPanController panController;

	private GameObject _camPosTarget;

	private static string s_iconTexture = "GizmoLookAt";

	private static float s_camDistanceFromTarget = 30f;

	private AutoMover _panMover;

	private void Awake()
	{
		_camPosTarget = new GameObject("CamPosTarget");
		_camPosTarget.transform.parent = base.transform;
		_camPosTarget.AddComponent(typeof(MarkerGizmo));
		_camPosTarget.GetComponent<MarkerGizmo>().iconTexture = s_iconTexture;
	}

	private void Start()
	{
		Camera.main.GetComponent<TouchHandler>().RegisterNonPhysicalTouchable(this);
		if (panBounds == null)
		{
			panBounds = Camera.main.transform.parent.GetComponentInChildren<CameraPanBounds>();
		}
		SetTargetPos();
		_panMover = panBounds.GetComponent<AutoMover>();
		if (panController == null)
		{
			panController = panBounds.transform.parent.GetComponentInChildren<CameraPanController>();
		}
	}

	public override bool AcceptTouch(GameTouch touch)
	{
		if (panBounds.GetComponent<AutoMover>().snapping)
		{
			return true;
		}
		return false;
	}

	[TriggerableAction(true)]
	public IEnumerator LookAtWithZoomOut()
	{
		SetTargetPos();
		panBounds.transform.position = _camPosTarget.transform.position;
		panController.lookAtPoint = this;
		panController.mode = cameraMode;
		panController.StartPanWithZoomOut(_camPosTarget.transform.position, panToDuration);
		while (panController.IsPanning())
		{
			yield return null;
		}
	}

	[TriggerableAction(true)]
	public IEnumerator LookAtWithZoomToDefault()
	{
		SetTargetPos();
		panBounds.transform.position = _camPosTarget.transform.position;
		panController.lookAtPoint = this;
		panController.mode = cameraMode;
		panController.StartPanWithZoomToDefault(_camPosTarget.transform.position, panToDuration);
		while (panController.IsPanning())
		{
			yield return null;
		}
	}

	[TriggerableAction(true)]
	public IEnumerator LookAtWithZoomIn()
	{
		SetTargetPos();
		panBounds.transform.position = _camPosTarget.transform.position;
		panController.lookAtPoint = this;
		panController.mode = cameraMode;
		panController.StartPanWithZoomIn(_camPosTarget.transform.position, panToDuration);
		while (panController.IsPanning())
		{
			yield return null;
		}
	}

	public void LookAtInEditor()
	{
		Vector3 position = base.transform.position - s_camDistanceFromTarget * Camera.main.transform.forward;
		Camera.main.transform.position = position;
		Camera.main.transform.parent.GetComponentInChildren<CameraPanBounds>().transform.position = position;
	}

	[TriggerableAction]
	public IEnumerator LookAtWithZoomOutImmediate()
	{
		SetTargetPos();
		panBounds.transform.position = _camPosTarget.transform.position;
		panController.lookAtPoint = this;
		panController.mode = cameraMode;
		panController.StartPanWithZoomOut(_camPosTarget.transform.position, 0f);
		return null;
	}

	private IEnumerator WaitForZoomEnd()
	{
		while (_panMover.snapping)
		{
			yield return null;
		}
	}

	private void SetTargetPos()
	{
		_camPosTarget.transform.position = base.transform.position - s_camDistanceFromTarget * panBounds.transform.forward;
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.white;
		Gizmos.DrawIcon(base.transform.position, s_iconTexture);
		if ((bool)_camPosTarget)
		{
			Gizmos.DrawLine(base.transform.position, _camPosTarget.transform.position);
		}
	}
}
