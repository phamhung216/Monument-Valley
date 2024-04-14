using System.Collections;
using UnityEngine;

public class BistablePerceptionController : MonoBehaviour
{
	public Camera invertCam;

	public CameraLookAtPoint refLookAtPoint;

	private Camera _camera;

	[TriggerableAction]
	public IEnumerator UseInvertCamForInput()
	{
		GameScene.player.GetComponent<PlayerInput>().currentCamera = invertCam;
		return null;
	}

	[TriggerableAction]
	public IEnumerator UseNormalCamForInput()
	{
		GameScene.player.GetComponent<PlayerInput>().currentCamera = Camera.main;
		return null;
	}

	private void Start()
	{
		_camera = GetComponent<Camera>();
	}

	private void LateUpdate()
	{
		_camera.orthographicSize = Camera.main.orthographicSize;
		Vector3 localPosition = Camera.main.transform.InverseTransformPoint(refLookAtPoint.transform.position);
		localPosition.x = -1f * localPosition.x;
		localPosition.y = -1f * localPosition.y;
		localPosition.z = 0f;
		invertCam.transform.localPosition = localPosition;
	}
}
