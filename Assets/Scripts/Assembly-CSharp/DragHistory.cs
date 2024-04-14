using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DragHistory
{
	private struct Datum
	{
		public float time;

		public Vector3 point;

		public Datum(float time, Vector3 point)
		{
			this.time = time;
			this.point = point;
		}
	}

	private List<Datum> _history = new List<Datum>();

	private float _historyDuration = 0.2f;

	private Vector3 _momentum = new Vector3(0f, 0f, 0f);

	public float decayRate = 3f;

	public float decayRefSpeed = 80f;

	public bool debugMe;

	public Vector3 momentum => _momentum;

	public DragHistory()
	{
	}

	public DragHistory(float decayRate, float decayRefSpeed)
	{
		this.decayRate = decayRate;
		this.decayRefSpeed = decayRefSpeed;
	}

	public void AddDatum(float time, Vector3 point)
	{
		_ = debugMe;
		_history.Add(new Datum(time, point));
		while (_history.Count > 0 && time - _history[0].time > _historyDuration)
		{
			_history.RemoveAt(0);
		}
		_ = debugMe;
	}

	public void DecayMomentum(float timestep)
	{
		_ = debugMe;
		if (_history.Count > 0)
		{
			AddDatum(Time.time, _history[_history.Count - 1].point);
			if (debugMe)
			{
				foreach (Datum item in _history)
				{
					_ = item;
				}
			}
			Vector3 vector = _history[_history.Count - 1].point - _history[0].point;
			float num = _history[_history.Count - 1].time - _history[0].time;
			num = _historyDuration;
			_momentum = ((num > 0f) ? (vector / num) : Vector3.zero);
			_ = debugMe;
			_history.Clear();
			float num2 = _momentum.magnitude / decayRefSpeed;
			if (num2 > 0f)
			{
				decayRate = _momentum.magnitude / num2;
			}
		}
		else
		{
			float magnitude = _momentum.magnitude;
			if (magnitude > 0f)
			{
				float a = magnitude - decayRate * timestep;
				a = Mathf.Max(a, 0f);
				_momentum = a / magnitude * _momentum;
			}
		}
	}

	public void ClearMomentum()
	{
		_ = debugMe;
		_momentum = Vector3.zero;
		_history.Clear();
	}
}
