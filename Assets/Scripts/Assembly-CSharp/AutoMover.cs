using System.Collections;
using UnityEngine;

public class AutoMover : MonoBehaviour
{
	public enum MoveMode
	{
		To = 0,
		FromTo = 1
	}

	public Transform startPoint;

	public Transform endPoint;

	public AnimationCurveDefinition snapAnimation;

	public bool snapping;

	public bool reverting;

	public float snapDuration = 1f;

	public float reverseDuration = -1f;

	public Transform target;

	public MoveMode moveMode;

	protected float _snapStartTime;

	private Vector3 _relativeStartOffset = Vector3.zero;

	public bool refreshNavOnTrigger = true;

	private MoverAudio _moverAudio;

	private void Start()
	{
		if (target == null)
		{
			target = base.gameObject.transform;
		}
		_moverAudio = target.GetComponent<MoverAudio>();
		if (snapAnimation == null)
		{
			GameObject gameObject = GameObject.Find("RotateSnapCurve");
			if ((bool)gameObject && (bool)gameObject.GetComponent<AnimationCurveDefinition>())
			{
				snapAnimation = gameObject.GetComponent<AnimationCurveDefinition>();
			}
		}
		if (moveMode == MoveMode.To && startPoint == null)
		{
			startPoint = target.transform;
		}
		if (reverseDuration < 0f)
		{
			reverseDuration = snapDuration;
		}
	}

	[TriggerableAction]
	public IEnumerator DoTrigger()
	{
		StartMove();
		return null;
	}

	[TriggerableAction]
	public IEnumerator DoRevert()
	{
		if (TriggerAction.FastForward)
		{
			ApplyStartPosition();
		}
		else
		{
			StartReverseMove();
		}
		return null;
	}

	[TriggerableAction]
	public IEnumerator ApplyStartPosition()
	{
		GameScene.navManager.NotifyReconfigurationBegan(target.gameObject);
		target.position = startPoint.position;
		GameScene.navManager.NotifyReconfigurationEnded();
		return null;
	}

	[TriggerableAction]
	public IEnumerator ApplyEndPosition()
	{
		GameScene.navManager.NotifyReconfigurationBegan(target.gameObject);
		target.position = endPoint.position;
		GameScene.navManager.NotifyReconfigurationEnded();
		return null;
	}

	[TriggerableAction]
	public IEnumerator StartMove()
	{
		if (moveMode == MoveMode.To && target.position == endPoint.position)
		{
			snapping = false;
			reverting = false;
			return null;
		}
		if (TriggerAction.FastForward)
		{
			ApplyEndPosition();
			return null;
		}
		if ((bool)_moverAudio && !_moverAudio.overrideEvents)
		{
			MoverAudioPresets.UpdateMoverWithPreset(_moverAudio, MoverAudioPresets.MoverAudioPresetType.PhysicalDragger);
		}
		snapping = true;
		reverting = false;
		_snapStartTime = Time.time;
		if (refreshNavOnTrigger)
		{
			GameScene.navManager.NotifyReconfigurationBegan(target.gameObject);
		}
		if (moveMode == MoveMode.To)
		{
			_relativeStartOffset.Set(target.position.x, target.position.y, target.position.z);
		}
		if (snapDuration == 0f)
		{
			Update();
		}
		return null;
	}

	[TriggerableAction(true)]
	public IEnumerator Move()
	{
		StartMove();
		if (TriggerAction.FastForward)
		{
			yield return null;
		}
		while (snapping)
		{
			yield return null;
		}
	}

	[TriggerableAction(true)]
	public IEnumerator ReverseMove()
	{
		StartReverseMove();
		if (TriggerAction.FastForward)
		{
			yield return null;
		}
		while (reverting)
		{
			yield return null;
		}
	}

	[TriggerableAction]
	public IEnumerator StartReverseMove()
	{
		if (TriggerAction.FastForward)
		{
			ApplyStartPosition();
			return null;
		}
		snapping = false;
		reverting = true;
		if (refreshNavOnTrigger)
		{
			GameScene.navManager.NotifyReconfigurationBegan(target.gameObject);
		}
		if (moveMode == MoveMode.To)
		{
			_relativeStartOffset.Set(target.position.x, target.position.y, target.position.z);
		}
		_snapStartTime = Time.time;
		if (reverseDuration == 0f)
		{
			Update();
		}
		return null;
	}

	[TriggerableAction]
	public IEnumerator StopMove()
	{
		snapping = false;
		reverting = false;
		return null;
	}

	private void Update()
	{
		Vector3 startPos = startPoint.position;
		if (snapping)
		{
			float normalizedTime = GetNormalizedTime(_snapStartTime);
			if (normalizedTime >= 1f)
			{
				target.position = GetTargetPosition(1f, forward: true, out startPos);
				snapping = false;
				if (refreshNavOnTrigger)
				{
					GameScene.navManager.NotifyReconfigurationEnded();
				}
			}
			else
			{
				target.position = GetTargetPosition(normalizedTime, forward: true, out startPos);
			}
		}
		else if (reverting)
		{
			float normalizedTime2 = GetNormalizedTime(_snapStartTime);
			if (normalizedTime2 >= 1f)
			{
				target.position = GetTargetPosition(1f, forward: false, out startPos);
				reverting = false;
				if (refreshNavOnTrigger)
				{
					GameScene.navManager.NotifyReconfigurationEnded();
				}
			}
			else
			{
				target.position = GetTargetPosition(normalizedTime2, forward: false, out startPos);
			}
		}
		if ((!snapping && !reverting) || !_moverAudio)
		{
			return;
		}
		if (_moverAudio.motionType == MoverAudio.MotionType.Translate)
		{
			_moverAudio.NotifyMove();
		}
		else if (_moverAudio.motionType == MoverAudio.MotionType.TranslateInteractive)
		{
			float magnitude = (target.position - startPoint.position).magnitude / (endPoint.position - startPoint.position).magnitude;
			int direction = 0;
			if (snapping)
			{
				direction = 1;
			}
			if (reverting)
			{
				direction = -1;
			}
			_moverAudio.NotifyMoveInteractive(magnitude, direction, (int)(endPoint.position - startPoint.position).magnitude);
		}
	}

	public Vector3 GetTargetPosition(float timeParam, bool forward, out Vector3 startPos)
	{
		if (null == snapAnimation)
		{
			Start();
		}
		float num = snapAnimation.curve.Evaluate(timeParam);
		startPos = ((moveMode == MoveMode.To) ? _relativeStartOffset : (forward ? startPoint.position : endPoint.position));
		Vector3 b = (forward ? endPoint.position : startPoint.position);
		Vector3 result = Vector3.Lerp(startPos, b, num);
		if (num > 1f)
		{
			Vector3 vector = Vector3.Lerp(startPos, b, num - 1f);
			result += vector - startPos;
		}
		if ((bool)_moverAudio && num > 0.985f && num <= 1f)
		{
			_moverAudio.PlayStopSound(timeParam, (1f - timeParam) * (forward ? snapDuration : reverseDuration));
		}
		return result;
	}

	public float GetNormalizedTime(float startTime)
	{
		float num = snapDuration;
		if (reverting)
		{
			num = reverseDuration;
		}
		if (num == 0f)
		{
			return 1f;
		}
		return Mathf.Clamp((Time.time - startTime) / num, 0f, 1f);
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
				Gizmos.DrawIcon(startPoint.position, "GizmoMoveStart");
			}
			if ((bool)endPoint)
			{
				Gizmos.DrawIcon(endPoint.position, "GizmoMoveEnd");
			}
			return;
		}
		Vector3 position = target.position;
		if (moveMode == MoveMode.FromTo)
		{
			if (!startPoint)
			{
				return;
			}
			position = startPoint.position;
			Color cyan = Color.cyan;
			cyan.a = 0.5f;
			Gizmos.color = cyan;
			Gizmos.DrawLine(target.position, position);
		}
		if ((bool)endPoint)
		{
			Gizmos.color = Color.cyan;
			Gizmos.DrawLine(position, endPoint.position);
		}
	}
}
