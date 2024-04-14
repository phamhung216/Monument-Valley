using UnityEngine;

public class ConstantRotate : MonoBehaviour
{
	public Transform target;

	public Vector3 speed;

	public bool useRotateAround;

	public Vector3 axis;

	public float angle;

	private void Start()
	{
		if (!target)
		{
			target = base.transform;
		}
	}

	private void LateUpdate()
	{
		float num = Time.deltaTime * 60f;
		if (useRotateAround)
		{
			target.RotateAround(target.position, axis, angle * num);
		}
		else
		{
			target.Rotate(speed * num);
		}
	}
}
