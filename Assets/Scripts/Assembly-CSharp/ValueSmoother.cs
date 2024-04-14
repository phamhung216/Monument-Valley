using UnityEngine;

public class ValueSmoother
{
	public float target;

	public float easeUpTime = 0.1f;

	public float easeDownTime = 0.1f;

	public float maxTarget = 360f;

	public float minTarget = -360f;

	public float clampDist = 0.1f;

	private float _currValue;

	private float _velocity;

	public float smoothedValue => _currValue;

	public void Reset(float value)
	{
		target = value;
		_currValue = value;
		_velocity = 0f;
	}

	public float Advance()
	{
		float num = Mathf.Clamp(target, minTarget, maxTarget);
		float num2 = _currValue - num;
		float num3 = ((num2 > 0f) ? easeDownTime : easeUpTime);
		if (num3 <= 0f || Mathf.Abs(num2) < clampDist)
		{
			_currValue = num;
			_velocity = 0f;
		}
		else
		{
			float deltaTime = Time.deltaTime;
			float num4 = 2f / num3;
			float num5 = num4 * deltaTime;
			float num6 = 1f / (1f + num5 + 0.48f * num5 * num5 * 0.235f * num5 * num5 * num5);
			float num7 = (_velocity + num4 * num2) * deltaTime;
			_velocity = (_velocity - num4 * num7) * num6;
			_currValue = num + (num2 + num7) * num6;
		}
		return _currValue;
	}
}
