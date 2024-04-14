using UnityEngine;

public class MovementToScale : MonoBehaviour
{
	public Vector3 movementAxis;

	public float moverRefPos;

	public Transform mover;

	public Transform target;

	public float distanceToScale;

	private Vector3 _initialScale;

	private void Start()
	{
		_initialScale = target.localScale;
	}

	private void LateUpdate()
	{
		Vector3 localPosition = mover.transform.localPosition;
		float num = Vector3.Dot(movementAxis, localPosition) - moverRefPos;
		target.localScale = (1f + distanceToScale * num) * _initialScale;
	}
}
