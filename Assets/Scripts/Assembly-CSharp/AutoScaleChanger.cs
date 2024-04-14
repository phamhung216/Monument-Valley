using System.Collections;
using UnityEngine;

public class AutoScaleChanger : MonoBehaviour
{
	public Transform startPoint;

	public Transform endPoint;

	public AnimationCurveDefinition snapAnimation;

	public bool snapping;

	public bool reverting;

	public float snapDuration = 1f;

	public Transform target;

	protected float _snapStartTime;

	private Vector3 _relativeStartOffset;

	public bool refreshNavOnTrigger = true;

	private void Start()
	{
		if (target == null)
		{
			target = base.gameObject.transform;
		}
		if (snapAnimation == null)
		{
			GameObject gameObject = GameObject.Find("RotateSnapCurve");
			if ((bool)gameObject && (bool)gameObject.GetComponent<AnimationCurveDefinition>())
			{
				snapAnimation = gameObject.GetComponent<AnimationCurveDefinition>();
			}
		}
	}

	[TriggerableAction]
	public IEnumerator DoRevert()
	{
		StartReverseScale();
		return null;
	}

	[TriggerableAction]
	public IEnumerator ApplyStartScale()
	{
		target.localScale = startPoint.localScale;
		return null;
	}

	[TriggerableAction]
	public IEnumerator StartScale()
	{
		if (target.localScale == endPoint.localScale)
		{
			return null;
		}
		snapping = true;
		_snapStartTime = Time.time;
		if (refreshNavOnTrigger)
		{
			GameScene.navManager.NotifyReconfigurationBegan(target.gameObject);
		}
		return null;
	}

	[TriggerableAction]
	public IEnumerator StartReverseScale()
	{
		reverting = true;
		if (refreshNavOnTrigger)
		{
			GameScene.navManager.NotifyReconfigurationBegan(target.gameObject);
		}
		_snapStartTime = Time.time;
		return null;
	}

	private void Update()
	{
		if (snapping)
		{
			float normalizedTime = GetNormalizedTime(_snapStartTime);
			if (normalizedTime >= 1f)
			{
				target.localScale = GetTargetPosition(1f);
				snapping = false;
				if (refreshNavOnTrigger)
				{
					GameScene.navManager.NotifyReconfigurationEnded();
				}
			}
			else
			{
				target.localScale = GetTargetPosition(normalizedTime);
			}
		}
		else
		{
			if (!reverting)
			{
				return;
			}
			float normalizedTime2 = GetNormalizedTime(_snapStartTime);
			if (normalizedTime2 >= 1f)
			{
				target.localScale = GetTargetPosition(1f, forward: false);
				reverting = false;
				if (refreshNavOnTrigger)
				{
					GameScene.navManager.NotifyReconfigurationEnded();
				}
			}
			else
			{
				target.localScale = GetTargetPosition(normalizedTime2, forward: false);
			}
		}
	}

	public Vector3 GetTargetPosition(float timeParam, bool forward = true)
	{
		float num = snapAnimation.curve.Evaluate(timeParam);
		Vector3 result = (forward ? Vector3.Lerp(startPoint.localScale, endPoint.localScale, num) : Vector3.Lerp(endPoint.localScale, startPoint.localScale, num));
		if (num > 1f)
		{
			Vector3 vector = (forward ? Vector3.Lerp(startPoint.localScale, endPoint.localScale, num - 1f) : Vector3.Lerp(endPoint.localScale, startPoint.localScale, num - 1f));
			result += vector - startPoint.localScale;
		}
		return result;
	}

	public float GetNormalizedTime(float startTime)
	{
		return Mathf.Clamp((Time.time - startTime) / snapDuration, 0f, 1f);
	}

	private void OnDrawGizmos()
	{
		if (!target)
		{
			Gizmos.color = Color.red;
			if ((bool)startPoint && (bool)endPoint)
			{
				Gizmos.DrawLine(startPoint.position, endPoint.position);
			}
			if ((bool)startPoint)
			{
				Gizmos.DrawWireSphere(startPoint.position, 1f);
			}
			if ((bool)endPoint)
			{
				Gizmos.DrawWireSphere(endPoint.position, 1f);
			}
		}
	}
}
