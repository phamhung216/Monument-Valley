using UnityEngine;

public class HandleHint : MonoBehaviour
{
	public RotatorHandle handle;

	private float _amount;

	private Vector3 initialSize;

	private void Awake()
	{
		initialSize = base.transform.localScale;
	}

	private void Update()
	{
		if (handle.dragging)
		{
			_amount = Mathf.MoveTowards(_amount, 1f, 5f * Time.deltaTime);
		}
		else
		{
			_amount = Mathf.MoveTowards(_amount, 0f, 5f * Time.deltaTime);
		}
		base.transform.localScale = new Vector3(initialSize.x, initialSize.y * _amount, initialSize.z);
	}
}
