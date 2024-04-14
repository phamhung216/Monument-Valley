using UnityEngine;

public class Easer
{
	private float _startTime = -1f;

	private float _start;

	private float _end;

	private float _duration;

	public AnimationCurve curve;

	public float value => Mathf.Lerp(_start, _end, Mathf.Clamp((curve != null) ? curve.Evaluate(timeParam) : timeParam, 0f, 1f));

	public bool isRunning => _startTime != -1f;

	public float timeParam
	{
		get
		{
			if (isRunning)
			{
				if (!(_duration <= 0f))
				{
					return (Time.time - _startTime) / _duration;
				}
				return 1f;
			}
			return 0f;
		}
	}

	public void StartFromTo(float from, float to, float time)
	{
		_startTime = Time.time;
		_start = from;
		_end = to;
		_duration = time;
	}

	public void End()
	{
		_startTime = -1f;
	}
}
