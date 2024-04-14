using System.Collections;
using UnityEngine;

public class AutoRotator : MonoBehaviour
{
	public enum Mode
	{
		FromTo = 0,
		To = 1,
		Relative = 2
	}

	public Rotatable target;

	public float startAngle;

	public float endAngle;

	public float duration = 1.2f;

	public AnimationCurveDefinition animationCurve;

	public bool useShortestRotation;

	public Mode mode = Mode.To;

	public bool snapping;

	public bool reverting;

	public bool useNavReconfig = true;

	private float _startTime;

	private float _startAngle;

	private float _endAngle;

	private bool _lastMoveForward;

	private MoverAudio _moverAudio;

	private void Start()
	{
		_moverAudio = target.GetComponent<MoverAudio>();
		if (animationCurve == null)
		{
			GameObject gameObject = GameObject.Find("EaseInOutCurve");
			if ((bool)gameObject && (bool)gameObject.GetComponent<AnimationCurveDefinition>())
			{
				animationCurve = gameObject.GetComponent<AnimationCurveDefinition>();
			}
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
		StartReverseMove();
		return null;
	}

	[TriggerableAction(true)]
	public IEnumerator Toggle()
	{
		if (_lastMoveForward)
		{
			return ReverseMove();
		}
		return Move();
	}

	[TriggerableAction]
	public IEnumerator ApplyEndRotation()
	{
		InitMoveAngles();
		if (useNavReconfig)
		{
			GameScene.navManager.NotifyReconfigurationBegan(base.gameObject);
		}
		target.currentAngle = _endAngle;
		if (useNavReconfig)
		{
			GameScene.navManager.NotifyReconfigurationEnded();
		}
		return null;
	}

	[TriggerableAction]
	public IEnumerator ApplyStartRotation()
	{
		InitMoveAngles();
		if (useNavReconfig)
		{
			GameScene.navManager.NotifyReconfigurationBegan(base.gameObject);
		}
		target.currentAngle = _startAngle;
		if (useNavReconfig)
		{
			GameScene.navManager.NotifyReconfigurationEnded();
		}
		return null;
	}

	private void InitMoveAngles()
	{
		if (mode == Mode.To || mode == Mode.Relative)
		{
			_startAngle = target.currentAngle;
		}
		else
		{
			_startAngle = startAngle;
		}
		if (mode == Mode.Relative)
		{
			_endAngle = target.currentAngle + endAngle;
		}
		else
		{
			_endAngle = endAngle;
		}
	}

	[TriggerableAction]
	public IEnumerator StartMove()
	{
		if (TriggerAction.FastForward)
		{
			ApplyEndRotation();
			return null;
		}
		if ((bool)_moverAudio && !_moverAudio.overrideEvents)
		{
			MoverAudioPresets.UpdateMoverWithPreset(_moverAudio, MoverAudioPresets.MoverAudioPresetType.PhysicalRotator);
		}
		_lastMoveForward = true;
		snapping = true;
		InitMoveAngles();
		_startTime = Time.time;
		if (useNavReconfig)
		{
			GameScene.navManager.NotifyReconfigurationBegan(base.gameObject);
		}
		bool flag = _startAngle == _endAngle;
		if (useShortestRotation)
		{
			flag = Mathf.DeltaAngle(_startAngle, _endAngle) == 0f;
		}
		if (flag)
		{
			EndRotator();
		}
		if (duration == 0f)
		{
			Update();
		}
		return null;
	}

	[TriggerableAction(true)]
	public IEnumerator Move()
	{
		StartMove();
		if (snapping)
		{
			return WaitForMoveEnd();
		}
		return null;
	}

	public IEnumerator WaitForMoveEnd()
	{
		if (TriggerAction.FastForward)
		{
			yield return null;
		}
		while (snapping || reverting)
		{
			yield return null;
		}
	}

	[TriggerableAction(true)]
	public IEnumerator ReverseMove()
	{
		StartReverseMove();
		if (reverting)
		{
			return WaitForMoveEnd();
		}
		return null;
	}

	[TriggerableAction]
	public IEnumerator StartReverseMove()
	{
		if (TriggerAction.FastForward)
		{
			ApplyStartRotation();
			return null;
		}
		_lastMoveForward = false;
		reverting = true;
		if (mode == Mode.To)
		{
			_startAngle = target.currentAngle;
		}
		else
		{
			_startAngle = endAngle;
		}
		_endAngle = startAngle;
		if (useNavReconfig)
		{
			GameScene.navManager.NotifyReconfigurationBegan(base.gameObject);
		}
		_startTime = Time.time;
		return null;
	}

	private void Update()
	{
		if (snapping || reverting)
		{
			float num = ((duration <= 0f) ? 1f : ((Time.time - _startTime) / duration));
			float num2 = animationCurve.curve.Evaluate(num);
			if (num >= 1f)
			{
				num2 = 1f;
			}
			if (_moverAudio != null && num2 > 0.985f)
			{
				_moverAudio.PlayStopSound(num, (1f - num) * duration);
			}
			if (useShortestRotation)
			{
				target.currentAngle = Mathf.LerpAngle(_startAngle, _endAngle, num2);
			}
			else
			{
				target.currentAngle = Mathf.Lerp(_startAngle, _endAngle, num2);
			}
			if (num >= 1f)
			{
				EndRotator();
			}
		}
	}

	private void EndRotator()
	{
		snapping = false;
		reverting = false;
		if (useNavReconfig)
		{
			GameScene.navManager.NotifyReconfigurationEnded();
		}
	}
}
