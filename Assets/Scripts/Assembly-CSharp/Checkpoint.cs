using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
	public List<Checkpoint> priorCheckpoints = new List<Checkpoint>();

	public ActionSequence startupActions = new ActionSequence();

	public CameraLookAtPoint startupLookAtPoint;

	public bool showLevelIntroSequence = true;

	private static string s_iconTexture = "GizmoCheckpoint";

	private bool _triggered;

	private bool _isReached;

	private float _triggerTime;

	public bool IsReached => _isReached;

	public float triggerTime
	{
		get
		{
			return _triggerTime;
		}
		set
		{
			_triggerTime = value;
		}
	}

	private void Awake()
	{
		_isReached = false;
		_triggered = false;
		_triggerTime = 0f;
		if (!startupLookAtPoint)
		{
			D.Error("Missing look at point for " + base.name, base.gameObject);
		}
	}

	public void FastForwardToHere()
	{
		Trigger();
	}

	private void Trigger()
	{
		if (!_triggered)
		{
			_isReached = true;
			_triggered = true;
			TriggerAction.FastForward = true;
			StartCoroutine(startupActions.DoSequence());
			TriggerAction.FastForward = false;
		}
	}

	private void OnDrawGizmos()
	{
		Vector3 vector = base.transform.position + 0.5f * base.transform.up;
		Gizmos.DrawIcon(vector, s_iconTexture);
		Gizmos.color = Color.green;
		Gizmos.DrawLine(vector, vector + base.transform.forward * 2f);
	}

	[TriggerableAction]
	public IEnumerator NotifyPassedCheckpoint()
	{
		if (!_isReached)
		{
			_isReached = true;
			LevelProgress.SaveCheckpoint(this);
		}
		return null;
	}
}
