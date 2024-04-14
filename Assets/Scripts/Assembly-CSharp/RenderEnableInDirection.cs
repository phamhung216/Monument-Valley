using UnityEngine;

public class RenderEnableInDirection : MonoBehaviour
{
	public Vector3 direction = new Vector3(-1f, 0f, -1f);

	private Renderer[] _renderers;

	private void Start()
	{
		_renderers = GetComponentsInChildren<Renderer>();
	}

	private void LateUpdate()
	{
		float num = Vector3.Dot(base.transform.forward, direction);
		for (int i = 0; i < _renderers.Length; i++)
		{
			_renderers[i].enabled = num > 0f;
		}
	}
}
