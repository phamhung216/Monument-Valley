using UnityEngine;

public class OffscreenDisabler : MonoBehaviour
{
	public Vector3 size;

	public Vector3 offset;

	public Transform target;

	private bool _isVisible;

	private Rect _panSpaceRect;

	private Vector3 _panSpaceOrigin;

	private CameraPanController _panController;

	private void Start()
	{
		if (!target)
		{
			target = base.transform;
		}
		_panController = Camera.main.GetComponent<CameraPanController>();
		_ = Camera.main;
		Vector3 position = base.transform.TransformPoint(offset + 0.5f * new Vector3(size.x, size.y, size.z));
		Vector3 position2 = base.transform.TransformPoint(offset + 0.5f * new Vector3(size.x, size.y, 0f - size.z));
		Vector3 position3 = base.transform.TransformPoint(offset + 0.5f * new Vector3(size.x, 0f - size.y, size.z));
		Vector3 position4 = base.transform.TransformPoint(offset + 0.5f * new Vector3(size.x, 0f - size.y, 0f - size.z));
		Vector3 position5 = base.transform.TransformPoint(offset + 0.5f * new Vector3(0f - size.x, size.y, size.z));
		Vector3 position6 = base.transform.TransformPoint(offset + 0.5f * new Vector3(0f - size.x, size.y, 0f - size.z));
		Vector3 position7 = base.transform.TransformPoint(offset + 0.5f * new Vector3(0f - size.x, 0f - size.y, size.z));
		Vector3 position8 = base.transform.TransformPoint(offset + 0.5f * new Vector3(0f - size.x, 0f - size.y, 0f - size.z));
		Vector3 vector = GameScene.WorldToPanPoint(position);
		Vector3 vector2 = GameScene.WorldToPanPoint(position2);
		Vector3 vector3 = GameScene.WorldToPanPoint(position3);
		Vector3 vector4 = GameScene.WorldToPanPoint(position4);
		Vector3 vector5 = GameScene.WorldToPanPoint(position5);
		Vector3 vector6 = GameScene.WorldToPanPoint(position6);
		Vector3 vector7 = GameScene.WorldToPanPoint(position7);
		Vector3 vector8 = GameScene.WorldToPanPoint(position8);
		float xmin = Mathf.Min(vector.x, vector2.x, vector3.x, vector4.x, vector5.x, vector6.x, vector7.x, vector8.x);
		float ymin = Mathf.Min(vector.y, vector2.y, vector3.y, vector4.y, vector5.y, vector6.y, vector7.y, vector8.y);
		float xmax = Mathf.Max(vector.x, vector2.x, vector3.x, vector4.x, vector5.x, vector6.x, vector7.x, vector8.x);
		float ymax = Mathf.Max(vector.y, vector2.y, vector3.y, vector4.y, vector5.y, vector6.y, vector7.y, vector8.y);
		_panSpaceRect = Rect.MinMaxRect(xmin, ymin, xmax, ymax);
		_panSpaceOrigin = GameScene.WorldToPanPoint(base.transform.position);
	}

	private void LateUpdate()
	{
		Rect rect = new Rect(Vector2.zero, _panController.bounds.size);
		if (_panController.zoom > 1f)
		{
			rect.center = GameScene.WorldToPanPoint(_panController.bounds.transform.position);
		}
		else
		{
			rect.center = GameScene.WorldToPanPoint(_panController.transform.position);
		}
		Vector3 panSpaceOrigin = _panSpaceOrigin;
		_panSpaceOrigin = GameScene.WorldToPanPoint(base.transform.position);
		Vector3 vector = _panSpaceOrigin - panSpaceOrigin;
		_panSpaceRect.center += new Vector2(vector.x, vector.y);
		_isVisible = false;
		if (rect.Overlaps(_panSpaceRect))
		{
			_isVisible = true;
		}
		if (target.gameObject.activeSelf != _isVisible)
		{
			target.gameObject.SetActive(_isVisible);
		}
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = (_isVisible ? Color.yellow : Color.grey);
		Gizmos.matrix = base.gameObject.transform.localToWorldMatrix;
		Gizmos.DrawWireCube(offset, size);
		Gizmos.matrix = Matrix4x4.identity;
	}
}
