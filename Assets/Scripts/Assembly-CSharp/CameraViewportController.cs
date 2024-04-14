using System.Collections;
using UnityEngine;

public class CameraViewportController : TriggerItem
{
	private Transform[] _roots;

	private Vector3 _offPosition = new Vector3(0f, 0f, -100f);

	private static Vector3 forwardWS = new Vector3(-1f, 0f, -1f);

	public bool viewportScrollingEnabled;

	public Camera[] sceneCameras;

	public Transform frontRoot;

	public Transform rearRoot;

	private Transform _roomContainingIda;

	public Transform roomContainingIda => _roomContainingIda;

	private void Start()
	{
		for (int i = 0; i < sceneCameras.Length; i++)
		{
			sceneCameras[i].gameObject.SetActive(value: true);
		}
		forwardWS.Normalize();
	}

	public void SetRoots(params Transform[] roots)
	{
		_roots = roots;
		UpdateStates();
	}

	[TriggerableAction]
	public IEnumerator DisableViewportScrolling()
	{
		viewportScrollingEnabled = false;
		return null;
	}

	[TriggerableAction]
	public IEnumerator KillMainCamera()
	{
		Camera.main.cullingMask = 0;
		return null;
	}

	[TriggerableAction]
	public IEnumerator EnableViewportScrolling()
	{
		viewportScrollingEnabled = true;
		Camera.main.cullingMask = LayerMask.GetMask("AlwaysShow");
		return null;
	}

	public void SetRearViewport(Transform rear)
	{
		if (!(rear == rearRoot) && !(rear == frontRoot))
		{
			rearRoot = rear;
			UpdateStates();
		}
	}

	public void UpdateStates()
	{
		for (int i = 0; i < _roots.Length; i++)
		{
			Transform transform = _roots[i];
			bool flag = transform == rearRoot || transform == frontRoot;
			transform.GetComponentInChildren<Camera>().enabled = flag;
			if (!flag)
			{
				transform.position = GetOffPosition(i);
			}
		}
	}

	private Vector3 GetOffPosition(int index)
	{
		return _offPosition * (index + 2);
	}

	private void LateUpdate()
	{
		Camera componentInChildren = rearRoot.GetComponentInChildren<Camera>();
		Camera componentInChildren2 = frontRoot.GetComponentInChildren<Camera>();
		if (!viewportScrollingEnabled)
		{
			componentInChildren2.enabled = true;
			componentInChildren.enabled = false;
			return;
		}
		componentInChildren2.enabled = true;
		componentInChildren.enabled = true;
		float num = Vector3.Dot(rearRoot.GetComponentInChildren<NSidedForwardDirection>().transform.forward, forwardWS);
		float num2 = Vector3.Dot(frontRoot.GetComponentInChildren<NSidedForwardDirection>().transform.forward, forwardWS);
		if (num > num2)
		{
			Transform transform = rearRoot;
			rearRoot = frontRoot;
			frontRoot = transform;
		}
		rearRoot.position = _offPosition;
		frontRoot.position = Vector3.zero;
		componentInChildren.clearFlags = CameraClearFlags.Depth;
		componentInChildren2.clearFlags = CameraClearFlags.Depth;
		componentInChildren.depth = -10f;
		componentInChildren2.depth = -5f;
	}

	[TriggerableAction]
	public IEnumerator ShowOnlyFinalRoom()
	{
		for (int i = 0; i < _roots.Length; i++)
		{
			Transform obj = _roots[i];
			bool flag = obj == frontRoot;
			obj.GetComponentInChildren<Camera>().enabled = flag;
			obj.position = (flag ? Vector3.zero : GetOffPosition(i));
		}
		frontRoot.GetComponentInChildren<Camera>().clearFlags = CameraClearFlags.Depth;
		return null;
	}

	[TriggerableAction]
	public IEnumerator UpdateIdaCurrentRoom()
	{
		for (int i = 0; i < _roots.Length; i++)
		{
			Transform parent = _roots[i];
			CharacterLocomotion component = GameScene.player.GetComponent<CharacterLocomotion>();
			if (component != null && component.lastValidBrush != null && component.lastValidBrush.transform.IsChildOf(parent))
			{
				_roomContainingIda = parent;
				break;
			}
		}
		return null;
	}
}
