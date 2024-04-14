using UnityEngine;

public class SpringPuller : MonoBehaviour
{
	public AxisDragger dragger;

	public AutoMover mover;

	public float waitTime = 2f;

	private bool _triggered;

	private float _triggerStartTime;

	private Collider _collider;

	private void Start()
	{
		_collider = GetComponent<Collider>();
	}

	private void LateUpdate()
	{
		bool flag = _collider.bounds.Contains(dragger.transform.position);
		if (!_triggered && !dragger.dragging && flag)
		{
			_triggered = true;
			_triggerStartTime = Time.time;
		}
		if (!flag)
		{
			_triggered = false;
		}
		if (_triggered && Time.time > _triggerStartTime + waitTime)
		{
			_triggered = false;
			mover.StartMove();
		}
	}
}
