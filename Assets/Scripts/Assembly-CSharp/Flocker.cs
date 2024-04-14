using System;
using UnityEngine;

public class Flocker : MonoBehaviour
{
	public Flocker leader;

	public float followDist = 5f;

	public float acceleration = 2f;

	public float maxSpeed = 10f;

	public float maxTurnRate = 90f;

	public float maxRollRate = 720f;

	public float sidewaysMovementPeriod = 4f;

	public float sidewaysMovementAmplitude = 0.5f;

	private Vector3 _heading;

	private float _speed;

	private Vector3 _right;

	private float _roll;

	private Vector3 _startPos;

	private float _flightTime;

	private bool _gotInitialRoll;

	private Vector3 GetRight()
	{
		if (!leader)
		{
			return base.transform.right;
		}
		return leader.GetRight();
	}

	private void Start()
	{
		_heading = base.transform.forward;
		_right = GetRight();
		_speed = 0f;
		_roll = 0f;
		_gotInitialRoll = false;
		_startPos = base.transform.position;
		_flightTime = 0f;
		sidewaysMovementPeriod *= UnityEngine.Random.Range(0.8f, 1.2f);
	}

	private void Update()
	{
		if ((bool)leader)
		{
			Vector3 lhs = leader.transform.position - base.transform.position;
			lhs -= Vector3.Dot(lhs, _right) * _right;
			Vector3 normalized = lhs.normalized;
			float magnitude = lhs.magnitude;
			float num = 2f;
			float num2 = Mathf.Clamp((magnitude - followDist) / num, -1f, 1f);
			_speed += acceleration * num2 * Time.deltaTime;
			_speed = Mathf.Clamp(_speed, 0f, maxSpeed);
			_heading = Vector3.RotateTowards(_heading, normalized, maxTurnRate * Time.deltaTime * ((float)Math.PI / 180f), float.MaxValue);
			_heading.Normalize();
			Vector3 vector = _heading * _speed * Time.deltaTime;
			Vector3 vector2 = base.transform.position + vector;
			_flightTime += Time.deltaTime;
			float num3 = Vector3.Dot(_startPos, _right);
			float num4 = sidewaysMovementAmplitude * Mathf.Sin((float)Math.PI * 2f * _flightTime / sidewaysMovementPeriod);
			vector2 -= Vector3.Project(vector2, _right);
			vector2 += (num3 + num4) * _right;
			base.transform.position = vector2;
			if (!_gotInitialRoll)
			{
				_roll = 0f - Vector3.Angle(base.transform.right, _right);
				_gotInitialRoll = true;
			}
			Vector3 vector3 = Vector3.Cross(_heading, _right);
			vector3 = Quaternion.AngleAxis(_roll, _heading) * vector3;
			base.transform.rotation = Quaternion.LookRotation(_heading, vector3);
			UpdateRoll(Time.deltaTime);
		}
	}

	private void UpdateRoll(float timeStep)
	{
		Vector3 to = Vector3.up - Vector3.Dot(base.transform.forward, Vector3.up) * base.transform.forward;
		to.Normalize();
		float num = Mathf.Clamp(Vector3.Angle(base.transform.up, to), (0f - maxRollRate) * timeStep, maxRollRate * timeStep);
		_roll += num;
	}

	private void OnDrawGizmos()
	{
		if ((bool)leader)
		{
			Gizmos.color = Color.magenta;
			Gizmos.DrawLine(base.transform.position, leader.transform.position);
			Gizmos.color = Color.cyan;
			Gizmos.DrawLine(base.transform.position, base.transform.position + _heading);
		}
	}
}
