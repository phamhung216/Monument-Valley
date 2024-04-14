using Fabric;
using UnityEngine;

public class RotatorHandle : Rotator, IHoverable
{
	private Vector3 _initialDragPos;

	private Vector3 _currDragPos;

	private Vector3 _localInitialDragPos;

	private Vector3 _localCurrentDragPos;

	private int _lastIntersectChoice = -1;

	private float _initialAngle;

	private float _abortDragRadius = 1f;

	private Vector3[,] _debugRays = new Vector3[4, 2];

	private bool _handlesActive = true;

	private bool _isOnAxis;

	private float _dragSpeed;

	private Collider _collider;

	public override bool showTouchIndicator
	{
		get
		{
			if (base.showTouchIndicator && _handlesActive)
			{
				return base.dragging;
			}
			return false;
		}
	}

	public override void StartDrag(Vector3 position)
	{
		if (!target.AllowRotation())
		{
			return;
		}
		base.StartDrag(position);
		_currDragPos = position;
		Ray ray = Camera.main.ScreenPointToRay(position);
		Vector3 vector = target.transform.TransformDirection(target.axis);
		vector.Normalize();
		Vector3 rhs = base.transform.position - target.transform.position;
		Vector3 vector2 = vector;
		Vector3 vector3 = target.transform.position;
		bool flag = false;
		_isOnAxis = Vector3.Cross(vector, rhs).sqrMagnitude < 0.1f;
		if (_isOnAxis)
		{
			flag = true;
			_abortDragRadius = 0.4f;
		}
		else
		{
			vector2 = Vector3.Cross(vector, rhs);
			vector3 = target.transform.position;
			if (Mathf.Abs(Vector3.Dot(vector2, ray.direction)) < 0.01f)
			{
				flag = true;
			}
			_abortDragRadius = Mathf.Clamp(0.25f * rhs.magnitude, 0.4f, 2f);
		}
		if (flag)
		{
			vector2 = vector;
			vector3 = target.transform.position;
			if (_collider == null)
			{
				_collider = base.gameObject.GetComponent<Collider>();
				if (_collider == null && base.ProxyAccessor != null)
				{
					_collider = base.ProxyAccessor.GetColliderOfTouchableProxyAtIndex(0);
				}
			}
			if (_collider != null)
			{
				RaycastHit hitInfo;
				if (_collider is SphereCollider)
				{
					vector3 = base.transform.TransformPoint(((SphereCollider)_collider).center);
				}
				else if (base.ProxyAccessor == null)
				{
					if (_collider.Raycast(ray, out hitInfo, float.MaxValue))
					{
						vector3 = ray.GetPoint(hitInfo.distance);
					}
				}
				else
				{
					for (int i = 0; i < base.ProxyAccessor.GetNumProxies(); i++)
					{
						_collider = base.ProxyAccessor.GetColliderOfTouchableProxyAtIndex(i);
						if (_collider.Raycast(ray, out hitInfo, float.MaxValue))
						{
							vector3 = ray.GetPoint(hitInfo.distance);
							break;
						}
					}
				}
			}
		}
		vector2.Normalize();
		Plane plane = new Plane(vector2, vector3);
		if (Vector3.Angle(vector, Camera.main.transform.up) < 5f)
		{
			_initialDragPos = vector3;
			_localInitialDragPos = base.transform.InverseTransformPoint(_initialDragPos);
			_initialAngle = target.currentAngle;
		}
		else
		{
			float enter = 0f;
			if (plane.Raycast(ray, out enter))
			{
				Vector3 initialDragPos = ray.origin + enter * ray.direction;
				_initialDragPos = initialDragPos;
				_localInitialDragPos = base.transform.InverseTransformPoint(_initialDragPos);
			}
			_initialAngle = target.currentAngle + CalcInitialPushAngle(ray, plane);
		}
		_lastIntersectChoice = -1;
	}

	private float CalcInitialPushAngle(Ray ray, Plane plane)
	{
		float enter = 0f;
		if (plane.Raycast(ray, out enter))
		{
			float f = Vector3.Dot(ray.direction, plane.normal);
			Vector3 vector = ray.direction - Mathf.Abs(f) * ray.direction;
			Vector3 point = ray.GetPoint(enter);
			Vector3 vector2 = point + 1f * vector;
			float a = Vector3.Angle(point - target.transform.position, vector2 - target.transform.position);
			Vector3 rhs = target.transform.TransformDirection(target.axis);
			rhs.Normalize();
			float f2 = Vector3.Dot(Vector3.Cross(point - target.transform.position, vector2 - point), rhs);
			return Mathf.Min(a, 5f) * Mathf.Sign(f2);
		}
		return 0f;
	}

	public override void Drag(Vector3 position, Vector3 delta)
	{
		base.Drag(position, delta);
		if (_dragging)
		{
			_currDragPos = position;
		}
	}

	public override void CancelDrag()
	{
		target.CancelDrag();
		base.CancelDrag();
	}

	public override void Snap()
	{
		base.Snap();
	}

	private void DragLever(Vector3 position)
	{
		Vector3 vector = target.transform.TransformDirection(target.axis);
		vector.Normalize();
		Vector3 initialDragPos = _initialDragPos;
		Vector3 axisPointWS = GetAxisPointWS();
		Vector3 rhs = Vector3.Cross(vector, initialDragPos - axisPointWS);
		rhs.Normalize();
		Plane plane = new Plane(vector, initialDragPos);
		Ray ray = Camera.main.ScreenPointToRay(position);
		Vector3 vector2 = initialDragPos;
		if (Mathf.Abs(Vector3.Angle(ray.direction, vector) - 90f) < 5f)
		{
			GetRaySphereIntersections(axisPointWS, (axisPointWS - initialDragPos).magnitude + 0.1f, ray, out var intersectionNear, out var _);
			vector2 = intersectionNear;
		}
		else
		{
			float enter = 0f;
			if (plane.Raycast(ray, out enter))
			{
				vector2 = ray.origin + enter * ray.direction;
			}
		}
		_localCurrentDragPos = base.transform.InverseTransformPoint(vector2);
		Vector3 lhs = initialDragPos - axisPointWS;
		lhs.Normalize();
		Vector3 rhs2 = vector2 - axisPointWS;
		rhs2.Normalize();
		RenderDebug.DrawLine(axisPointWS, initialDragPos, Color.magenta);
		RenderDebug.DrawLine(axisPointWS, vector2, Color.cyan);
		float f = Mathf.Clamp(Vector3.Dot(lhs, rhs2), -1f, 1f);
		float num = 57.29578f * Mathf.Acos(f);
		if (Vector3.Dot(vector2 - axisPointWS, rhs) < 0f)
		{
			num *= -1f;
		}
		float num2 = Mathf.DeltaAngle(target.currentAngle, num + _initialAngle);
		ApplyIntendedAngle(target.currentAngle + num2);
	}

	private void ApplyIntendedAngle(float angle)
	{
		if (target.limitRotation)
		{
			if (target.softConstraint)
			{
				if (angle < target.minRotation)
				{
					float softConstraintAmount = target.softConstraintAmount;
					float num = Mathf.Abs(angle - target.minRotation);
					float num2 = softConstraintAmount * (1f - 1f / (num / softConstraintAmount + 1f));
					angle = target.minRotation - num2;
				}
				if (angle > target.maxRotation)
				{
					float softConstraintAmount2 = target.softConstraintAmount;
					float num3 = Mathf.Abs(angle - target.maxRotation);
					float num4 = softConstraintAmount2 * (1f - softConstraintAmount2 / (num3 + softConstraintAmount2));
					angle = target.maxRotation + num4;
				}
			}
			else
			{
				angle = Mathf.Clamp(angle, target.minRotation, target.maxRotation);
			}
		}
		if (target.maxRotationSpeed > 0f)
		{
			float f = Mathf.DeltaAngle(target.currentAngle, angle);
			float num5 = target.maxRotationSpeed * Time.deltaTime;
			if (Mathf.Abs(f) > num5)
			{
				f = Mathf.Sign(f) * num5;
				angle = target.currentAngle + f;
			}
		}
		target.currentAngle = angle;
	}

	private void DragPole(Vector3 position)
	{
		Vector3 axisPointWS = GetAxisPointWS();
		Vector3 vector = target.transform.TransformDirection(target.axis);
		vector.Normalize();
		Vector3 initialDragPos = _initialDragPos;
		Vector3 rhs = Vector3.Cross(vector, initialDragPos - axisPointWS);
		rhs.Normalize();
		Plane plane = new Plane(vector, initialDragPos);
		Ray ray = Camera.main.ScreenPointToRay(position);
		float enter = 0f;
		if (!plane.Raycast(ray, out enter))
		{
			return;
		}
		Vector3 vector2 = ray.origin + enter * ray.direction;
		Vector3 rhs2 = vector2 - axisPointWS;
		if (rhs2.magnitude < _abortDragRadius)
		{
			Snap();
			return;
		}
		rhs2.Normalize();
		Vector3 vector3 = Vector3.Cross(new Plane(Vector3.Cross(ray.direction, vector), vector2).normal, plane.normal);
		vector3.Normalize();
		Ray ray2 = new Ray(vector2, vector3);
		float magnitude = (initialDragPos - axisPointWS).magnitude;
		float num = (vector2 - axisPointWS).magnitude;
		if (num < magnitude)
		{
			num = magnitude;
		}
		GetRaySphereIntersections(axisPointWS, num, ray2, out var intersectionNear, out var intersectionFar);
		_debugRays[0, 0] = axisPointWS;
		_debugRays[0, 1] = vector2;
		_debugRays[1, 0] = vector2;
		_debugRays[1, 1] = vector2 + vector3 * 10f;
		_debugRays[2, 0] = axisPointWS;
		_debugRays[2, 1] = intersectionNear;
		_debugRays[3, 0] = axisPointWS;
		_debugRays[3, 1] = intersectionFar;
		Vector3 lhs = intersectionNear - axisPointWS;
		lhs.Normalize();
		float num2 = Mathf.Clamp(Vector3.Dot(lhs, rhs2), -1f, 1f);
		Vector3 vector4 = intersectionFar - axisPointWS;
		vector4.Normalize();
		float num3 = Mathf.Clamp(Vector3.Dot(vector4, rhs2), -1f, 1f);
		bool flag = num2 > num3;
		if (_lastIntersectChoice < 0)
		{
			vector2 = (flag ? intersectionNear : intersectionFar);
		}
		else
		{
			float num4 = 57.29578f * Mathf.Acos(Mathf.Clamp(Vector3.Dot(lhs, vector4), -1f, 1f));
			float num5 = 180f;
			if (num4 > num5)
			{
				flag = _lastIntersectChoice == 1;
			}
			if (!(num2 > 1f))
			{
				_ = 1f;
			}
			float num6 = 57.29578f * Mathf.Acos(Mathf.Max(num2, num3));
			float num7 = Mathf.Clamp(2f * (num6 / num4), 0f, 1f);
			vector2 = num7 * vector2 + (1f - num7) * (flag ? intersectionNear : intersectionFar);
		}
		if (flag)
		{
			_lastIntersectChoice = 1;
		}
		else
		{
			_lastIntersectChoice = 2;
		}
		_localCurrentDragPos = base.transform.InverseTransformPoint(vector2);
		Vector3 lhs2 = initialDragPos - axisPointWS;
		lhs2.Normalize();
		Vector3 rhs3 = vector2 - axisPointWS;
		rhs3.Normalize();
		float num8 = 57.29578f * Mathf.Acos(Mathf.Clamp(Vector3.Dot(lhs2, rhs3), -1f, 1f));
		if (Vector3.Dot(vector2 - axisPointWS, rhs) < 0f)
		{
			num8 *= -1f;
		}
		float num9 = Mathf.DeltaAngle(target.currentAngle, num8 + _initialAngle);
		ApplyIntendedAngle(target.currentAngle + num9);
	}

	public int GetRaySphereIntersections(Vector3 circleOrigin, float radius, Ray ray, out Vector3 intersectionNear, out Vector3 intersectionFar)
	{
		ray.direction.Normalize();
		Vector3 vector = ray.origin - circleOrigin;
		Vector3 vector2 = vector - ray.direction * Vector3.Dot(ray.direction, vector);
		float magnitude = vector2.magnitude;
		if (magnitude >= radius)
		{
			vector2.Normalize();
			intersectionFar = circleOrigin + radius * vector2;
			intersectionNear = intersectionFar;
			if (magnitude != radius)
			{
				return 0;
			}
			return 1;
		}
		float num = Mathf.Sqrt(radius * radius - magnitude * magnitude);
		intersectionFar = circleOrigin + vector2 + num * ray.direction;
		intersectionNear = circleOrigin + vector2 - num * ray.direction;
		return 2;
	}

	public void OnDrawGizmos()
	{
		if (base.dragging)
		{
			Gizmos.color = Color.green;
			Gizmos.DrawSphere(base.transform.TransformPoint(_localInitialDragPos), 0.2f);
			Gizmos.DrawLine(GetAxisPointWS(), base.transform.TransformPoint(_localInitialDragPos));
			Gizmos.DrawLine(_initialDragPos, base.transform.TransformPoint(_localInitialDragPos));
			Gizmos.color = Color.white;
			Gizmos.DrawLine(GetAxisPointWS(), target.transform.position);
			Gizmos.color = Color.red;
			Gizmos.DrawSphere(base.transform.TransformPoint(_localCurrentDragPos), 0.2f);
			Gizmos.DrawLine(GetAxisPointWS(), base.transform.TransformPoint(_localCurrentDragPos));
			Gizmos.color = Color.magenta;
			for (int i = 0; i < 4; i++)
			{
				Gizmos.DrawLine(_debugRays[i, 0], _debugRays[i, 1]);
			}
		}
	}

	public void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.yellow;
		if ((bool)target)
		{
			Gizmos.DrawRay(target.transform.position, 20f * target.transform.TransformDirection(target.axis));
		}
	}

	private Vector3 GetAxisPointWS()
	{
		Vector3 vector = target.transform.TransformDirection(target.axis);
		vector.Normalize();
		Vector3 initialDragPos = _initialDragPos;
		return target.transform.position + vector * Vector3.Dot(vector, initialDragPos - target.transform.position);
	}

	public override Vector3 GetTouchIndicatorPosition(GameTouch touch)
	{
		return base.transform.TransformPoint(_localInitialDragPos);
	}

	protected override void Update()
	{
		base.Update();
		if (target.AllowRotation())
		{
			if (!_handlesActive)
			{
				_handlesActive = true;
				PlayAnimationInChildren("Open");
				if ((bool)EventManager.Instance)
				{
					EventManager.Instance.PostEvent("World/Movers/Global/Rotor_HandleShrink", EventAction.PlaySound);
				}
			}
		}
		else
		{
			if (base.dragging)
			{
				CancelDrag();
			}
			if (_handlesActive)
			{
				_handlesActive = false;
				PlayAnimationInChildren("Close");
				if ((bool)EventManager.Instance)
				{
					EventManager.Instance.PostEvent("World/Movers/Global/Rotor_HandleGrow", EventAction.PlaySound);
				}
			}
		}
		if (_dragging)
		{
			if (_isOnAxis)
			{
				DragLever(_currDragPos);
			}
			else
			{
				DragPole(_currDragPos);
			}
			GlowFull();
		}
		else
		{
			GlowDecrease();
		}
	}

	private void PlayAnimationInChildren(string animationName)
	{
		Animation[] componentsInChildren = GetComponentsInChildren<Animation>();
		foreach (Animation animation in componentsInChildren)
		{
			AnimationState animationState = animation[animationName];
			if ((bool)animationState)
			{
				animation.Stop();
				animationState.time = 0f;
				animation.Play(animationName);
			}
		}
	}
}
