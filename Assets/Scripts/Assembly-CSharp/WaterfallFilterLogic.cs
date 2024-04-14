using System.Collections;
using Fabric;
using UnityEngine;

public class WaterfallFilterLogic : MonoBehaviour
{
	public Rotatable rotatable;

	public string waterfallEvent;

	public float fadeInTime;

	public float fadeOutTime;

	public ValueSmoother _smoother;

	public AnimationCurveDefinition falloffCurve;

	private const string WaterfallFilterEvent = "WaterfallFilter";

	private const string WaterfallFadeEvent = "WaterfallFade";

	private void Start()
	{
		_smoother = new ValueSmoother();
		_smoother.easeUpTime = fadeInTime;
		_smoother.easeDownTime = fadeOutTime;
		_smoother.Reset(0f);
		if ((bool)EventManager.Instance)
		{
			EventManager.Instance.PostEvent(waterfallEvent, EventAction.PlaySound, base.gameObject);
		}
	}

	private void Update()
	{
		if ((bool)EventManager.Instance)
		{
			float value = falloffCurve.curve.Evaluate(Mathf.Abs(Mathf.DeltaAngle(rotatable.currentAngle, 0f)) / 180f);
			EventManager.Instance.SetParameter(waterfallEvent, "WaterfallFilter", value, base.gameObject);
			_smoother.Advance();
			EventManager.Instance.SetParameter(waterfallEvent, "WaterfallFade", _smoother.smoothedValue, base.gameObject);
		}
	}

	[TriggerableAction]
	public IEnumerator StartWaterfallAudio()
	{
		_smoother.Reset(0f);
		_smoother.target = 1f;
		return null;
	}

	[TriggerableAction]
	public IEnumerator StartWaterfallAudioFast()
	{
		_smoother.Reset(0f);
		_smoother.easeUpTime = 1f;
		_smoother.target = 1f;
		return null;
	}

	[TriggerableAction]
	public IEnumerator EndWaterfallAudio()
	{
		_smoother.target = 0f;
		return null;
	}

	[TriggerableAction]
	public IEnumerator KillWaterfallAudio()
	{
		if ((bool)EventManager.Instance)
		{
			EventManager.Instance.PostEvent(waterfallEvent, EventAction.StopSound, base.gameObject);
		}
		return null;
	}
}
