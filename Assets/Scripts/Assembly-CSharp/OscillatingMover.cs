using System;
using UnityEngine;

public class OscillatingMover : MonoBehaviour
{
	public Vector3 deltaPosition;

	public float duration = 2f;

	public bool startWithRandomOffset;

	public float startParam;

	public bool centered;

	private float _time;

	private Vector3 _startPosition;

	private void Start()
	{
		_startPosition = base.transform.localPosition;
		if (startWithRandomOffset)
		{
			startParam = UnityEngine.Random.Range(0f, 1f);
		}
	}

	private void Update()
	{
		float f = (float)Math.PI * 2f * (_time / duration + startParam);
		float num = 0.5f * Mathf.Cos(f);
		if (!centered)
		{
			num = 0.5f - num;
		}
		base.transform.localPosition = _startPosition + deltaPosition * num;
		_time += Time.deltaTime;
	}

	public void MoveCentreOfMotion(Vector3 worldDeltaPos)
	{
		_startPosition += base.transform.parent.InverseTransformVector(worldDeltaPos);
	}
}
