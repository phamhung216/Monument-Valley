using UnityEngine;

public class RotatorSnapper : Rotator
{
	public float dragDampening = 0.7f;

	public Transform pivotTransform;

	private Vector3 lastPosition = Vector3.zero;

	private BoxCollider box;

	private float _currentTotalRotation;

	public float maxRotation;

	public float minRotation;

	public bool limitRotation;

	public bool softConstraint = true;

	public float softConstraintAmount = 20f;

	private Vector3 pivotPoint
	{
		get
		{
			if (pivotTransform != null)
			{
				return pivotTransform.position;
			}
			return base.transform.TransformPoint(box.center);
		}
	}

	private new void Start()
	{
		base.Start();
		box = GetComponent<Collider>() as BoxCollider;
	}

	public override bool AcceptTouch(GameTouch touch)
	{
		return true;
	}

	private bool AllowRotation()
	{
		return target.AllowRotation();
	}

	public override void StartDrag(Vector3 position)
	{
		base.StartDrag(position);
		_currentTotalRotation = base.currentAngle;
		lastPosition = position;
	}

	public override void Drag(Vector3 position, Vector3 delta)
	{
		if (!AllowRotation())
		{
			if (base.dragging)
			{
				Snap();
			}
		}
		else
		{
			if (!_dragging)
			{
				return;
			}
			if (lastPosition != Vector3.zero)
			{
				Vector3 vector = Camera.main.WorldToScreenPoint(pivotPoint);
				float num = Mathf.Atan2(lastPosition.y - vector.y, lastPosition.x - vector.x) * 57.29578f;
				if (num < 0f)
				{
					num = 360f + num;
				}
				if (num > 360f)
				{
					num -= 360f;
				}
				float num2 = Mathf.Atan2(position.y - vector.y, position.x - vector.x) * 57.29578f;
				if (num2 < 0f)
				{
					num2 = 360f + num2;
				}
				if (num2 > 360f)
				{
					num2 -= 360f;
				}
				float num3 = num - num2;
				if (Vector3.Dot(target.transform.TransformDirection(target.axis), Camera.main.transform.forward) > 0f)
				{
					num3 *= -1f;
				}
				if (limitRotation)
				{
					_currentTotalRotation = Mathf.Clamp(_currentTotalRotation, minRotation, maxRotation);
					if (softConstraint)
					{
						if (_currentTotalRotation + num3 > maxRotation + softConstraintAmount)
						{
							num3 = maxRotation + softConstraintAmount - _currentTotalRotation;
						}
						if (_currentTotalRotation + num3 < minRotation - softConstraintAmount)
						{
							num3 = minRotation - softConstraintAmount - _currentTotalRotation;
						}
						if (_currentTotalRotation + num3 <= maxRotation + softConstraintAmount && _currentTotalRotation + num3 >= minRotation - softConstraintAmount)
						{
							_currentTotalRotation += num3;
						}
					}
					else
					{
						if (_currentTotalRotation + num3 > maxRotation)
						{
							num3 = maxRotation + softConstraintAmount - _currentTotalRotation;
						}
						if (_currentTotalRotation + num3 < minRotation)
						{
							num3 = minRotation - softConstraintAmount - _currentTotalRotation;
						}
						if (_currentTotalRotation + num3 <= maxRotation && _currentTotalRotation + num3 >= minRotation)
						{
							_currentTotalRotation += num3;
						}
					}
				}
				else
				{
					_currentTotalRotation += num3;
				}
			}
			lastPosition = position;
		}
	}

	public override void Snap()
	{
		if (!base.snapping)
		{
			base.Snap();
			lastPosition = Vector3.zero;
		}
	}

	public override void EndSnapping()
	{
		base.EndSnapping();
	}

	private new void Update()
	{
		base.Update();
		if (!base.snapping && base.dragging)
		{
			target.currentAngle = _currentTotalRotation;
		}
	}

	private void OnDrawGizmos()
	{
		if (base.dragging)
		{
			Gizmos.color = Color.red;
			Gizmos.DrawLine(pivotPoint, Camera.main.ScreenToWorldPoint(lastPosition));
		}
	}
}
