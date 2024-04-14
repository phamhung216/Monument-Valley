using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoScaleChangerStaggered : MonoBehaviour
{
	public AutoScaleChanger scaleChanger;

	public List<Transform> followers;

	public float delay = 0.2f;

	private float[] _startTimes;

	private float[] _diffs;

	public bool autoInitialise = true;

	public bool snapping;

	public bool reverting;

	public bool pending;

	private void Start()
	{
		_startTimes = new float[followers.Count];
		_diffs = new float[followers.Count];
		for (int i = 0; i < followers.Count; i++)
		{
			Transform transform = followers[i];
			_diffs[i] = (transform.position - scaleChanger.target.position).magnitude;
			if (autoInitialise)
			{
				transform.localScale = scaleChanger.startPoint.localScale;
			}
		}
		scaleChanger.refreshNavOnTrigger = false;
	}

	[TriggerableAction]
	public IEnumerator StartScale()
	{
		for (int i = 0; i < _startTimes.Length; i++)
		{
			_startTimes[i] = Time.time + _diffs[i] * delay;
		}
		scaleChanger.StartScale();
		GameScene.navManager.NotifyReconfigurationBegan(scaleChanger.gameObject);
		snapping = true;
		pending = true;
		return null;
	}

	private void Update()
	{
		if (!pending)
		{
			return;
		}
		bool flag = true;
		for (int i = 0; i < followers.Count; i++)
		{
			Transform transform = followers[i];
			float startTime = _startTimes[i];
			if (snapping)
			{
				float normalizedTime = scaleChanger.GetNormalizedTime(startTime);
				if (normalizedTime >= 1f)
				{
					transform.localScale = scaleChanger.GetTargetPosition(1f);
					continue;
				}
				transform.localScale = scaleChanger.GetTargetPosition(normalizedTime);
				flag = false;
			}
			else if (reverting)
			{
				float normalizedTime2 = scaleChanger.GetNormalizedTime(startTime);
				if (normalizedTime2 >= 1f)
				{
					transform.localScale = scaleChanger.GetTargetPosition(1f, forward: false);
					continue;
				}
				transform.localScale = scaleChanger.GetTargetPosition(normalizedTime2, forward: false);
				flag = false;
			}
		}
		if (flag)
		{
			snapping = false;
			reverting = false;
			pending = false;
			GameScene.navManager.NotifyReconfigurationEnded();
		}
	}
}
