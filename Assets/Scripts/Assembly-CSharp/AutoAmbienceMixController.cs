using System.Collections.Generic;
using Fabric;
using UnityEngine;

public class AutoAmbienceMixController : MonoBehaviour
{
	public float duckingCooldown = 2.5f;

	private string _ambienceEvent;

	private List<string> _stingEvents;

	private float _startTime;

	private bool _stingActive;

	private ValueSmoother _smoother;

	private const float _target = 0.7f;

	private void Start()
	{
		EventListener[] componentsInChildren = GetComponentsInChildren<EventListener>();
		_smoother = new ValueSmoother();
		_smoother.easeUpTime = 0.25f;
		_smoother.easeDownTime = 1f;
		_stingEvents = new List<string>();
		EventListener[] array = componentsInChildren;
		foreach (EventListener eventListener in array)
		{
			if (eventListener._eventName.ToLower().IndexOf("/amb/") >= 0)
			{
				_ambienceEvent = eventListener._eventName;
			}
			else if (eventListener._eventName.ToLower().IndexOf("sting/") >= 0)
			{
				_stingEvents.Add(eventListener._eventName);
			}
		}
		EventManager.Instance.SetParameter(_ambienceEvent, "Ducking", 0f, GameScene.instance.gameObject);
	}

	private void Update()
	{
		if (!EventManager.Instance)
		{
			return;
		}
		bool flag = false;
		foreach (string stingEvent in _stingEvents)
		{
			if (EventManager.Instance.IsEventActive(stingEvent, GameScene.instance.gameObject))
			{
				flag = true;
				break;
			}
		}
		if (!_stingActive && flag)
		{
			_smoother.target = 0.7f;
			_startTime = Time.time;
		}
		if (Time.time - _startTime > duckingCooldown)
		{
			_smoother.target = 0f;
		}
		_stingActive = flag;
		float smoothedValue = _smoother.smoothedValue;
		_smoother.Advance();
		if (_smoother.smoothedValue != smoothedValue)
		{
			EventManager.Instance.SetParameter(_ambienceEvent, "Ducking", _smoother.smoothedValue, GameScene.instance.gameObject);
		}
	}
}
