using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoMoverStaggered : MonoBehaviour
{
	public AutoMover mover;

	public List<Transform> followers;

	public float delay = 0.2f;

	private float[] _startTimes;

	private Vector3[] _diffs;

	private bool[] _hasStartedMoving;

	private bool[] _hasFinishedMoving;

	public bool snapping;

	public bool reverting;

	public bool pending;

	public bool reconfiguresNav = true;

	public bool applyStartPositionToMaster;

	public bool playCustomShotOnStart;

	public bool playCustomShotOnEnd;

	public bool muteAudio;

	private StaggeredMoverCustomAudio _customMoverAudio;

	private void Start()
	{
		_startTimes = new float[followers.Count];
		_diffs = new Vector3[followers.Count];
		_hasStartedMoving = new bool[followers.Count];
		_hasFinishedMoving = new bool[followers.Count];
		for (int i = 0; i < followers.Count; i++)
		{
			Transform transform = followers[i];
			_diffs[i] = transform.position - mover.target.position;
			transform.position = mover.startPoint.position + _diffs[i];
		}
		mover.refreshNavOnTrigger = false;
		if (applyStartPositionToMaster)
		{
			mover.ApplyStartPosition();
		}
		_customMoverAudio = GetComponent<StaggeredMoverCustomAudio>();
	}

	[TriggerableAction]
	public IEnumerator StartMove()
	{
		for (int i = 0; i < _startTimes.Length; i++)
		{
			_startTimes[i] = Time.time + _diffs[i].magnitude * delay;
			_hasStartedMoving[i] = false;
			_hasFinishedMoving[i] = false;
		}
		mover.StartMove();
		if (reconfiguresNav)
		{
			GameScene.navManager.NotifyReconfigurationBegan(mover.gameObject);
		}
		snapping = true;
		pending = true;
		return null;
	}

	[TriggerableAction]
	public IEnumerator StartReverseMove()
	{
		float num = 0f;
		for (int i = 0; i < _startTimes.Length; i++)
		{
			_startTimes[i] = _diffs[i].magnitude * delay;
			num = Mathf.Max(num, _startTimes[i]);
			_hasStartedMoving[i] = false;
			_hasFinishedMoving[i] = false;
		}
		for (int j = 0; j < _startTimes.Length; j++)
		{
			_startTimes[j] = Time.time + num - _startTimes[j];
		}
		mover.StartReverseMove();
		if (reconfiguresNav)
		{
			GameScene.navManager.NotifyReconfigurationBegan(mover.gameObject);
		}
		reverting = true;
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
			float normalizedTime = mover.GetNormalizedTime(startTime);
			Vector3 startPos = transform.position;
			bool flag2 = true;
			if (snapping)
			{
				if (normalizedTime >= 0.9f && !_hasFinishedMoving[i])
				{
					_hasFinishedMoving[i] = true;
					if (!muteAudio && playCustomShotOnEnd && (bool)_customMoverAudio)
					{
						_customMoverAudio.PlayNextNote();
					}
				}
				if (normalizedTime >= 1f)
				{
					transform.position = mover.GetTargetPosition(1f, forward: true, out startPos) + _diffs[i];
				}
				else
				{
					transform.position = mover.GetTargetPosition(normalizedTime, forward: true, out startPos) + _diffs[i];
					flag2 = false;
					if (!_hasStartedMoving[i] && normalizedTime != 0f)
					{
						if (!muteAudio && playCustomShotOnStart && (bool)_customMoverAudio)
						{
							_customMoverAudio.PlayNextNote();
						}
						_hasStartedMoving[i] = true;
						ParticleCollection component = transform.GetComponent<ParticleCollection>();
						if (component != null)
						{
							component.PlayStartParticle();
						}
					}
				}
			}
			else if (reverting)
			{
				if (normalizedTime >= 1f)
				{
					transform.position = mover.GetTargetPosition(1f, forward: false, out startPos) + _diffs[i];
				}
				else
				{
					transform.position = mover.GetTargetPosition(normalizedTime, forward: false, out startPos) + _diffs[i];
					flag2 = false;
					if (!_hasStartedMoving[i] && normalizedTime != 0f)
					{
						_hasStartedMoving[i] = true;
						ParticleCollection component2 = transform.GetComponent<ParticleCollection>();
						if (component2 != null)
						{
							component2.PlayEndParticle();
						}
					}
				}
			}
			flag = flag && flag2;
		}
		if (flag)
		{
			snapping = false;
			reverting = false;
			pending = false;
			if (reconfiguresNav)
			{
				GameScene.navManager.NotifyReconfigurationEnded();
			}
		}
	}
}
