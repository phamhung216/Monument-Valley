using UnityEngine;

public class MovementToRotation : MonoBehaviour
{
	public AxisDragger source;

	public Rotatable target;

	public float ratio;

	private float _startAngle;

	private Vector3 _startPos;

	private void Start()
	{
		_startAngle = target.currentAngle;
		_startPos = source.transform.localPosition;
	}

	private void Update()
	{
		int index = ((!source.dragX) ? (source.dragY ? 1 : 2) : 0);
		float num = (source.transform.localPosition - _startPos)[index] * ratio;
		target.currentAngle = _startAngle + num;
	}
}
