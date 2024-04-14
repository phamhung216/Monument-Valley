using System.Collections;
using Fabric;
using UnityEngine;

public class MoverAudio : MonoBehaviour
{
	public enum MotionType
	{
		Translate = 0,
		Rotate = 1,
		TranslateInteractive = 2,
		RotateInteractive = 3,
		Totem = 4,
		TranslateInteractiveSmooth = 5
	}

	public bool overrideEvents;

	public string audioEvent;

	public string secondaryEvent;

	public string stopEvent;

	public bool isLarge;

	public bool forceOnlyOneStopSound;

	public MotionType _motionType;

	private ValueSmoother _smoothedVelocity = new ValueSmoother();

	private float _currentValue;

	private bool _isMoving;

	private Vector3 _lastPos;

	private float _lastNotificationTime;

	private static ChordDefinitions _chordDefinitions;

	public string chordName;

	public string secondaryChord;

	public bool mute;

	public bool atonal;

	public int root;

	public static bool initialised;

	private float _lastAngle;

	private bool _cooldown = true;

	private float _lastNotch;

	private float _snapAngle = 90f;

	private Vector3 _lastDragNotch;

	private int _direction;

	private int _notches;

	private int _lastNote;

	private float _lastPlayedStopSound;

	private float _stopSoundCooldown = 0.2f;

	public MotionType motionType
	{
		get
		{
			return _motionType;
		}
		set
		{
			_motionType = value;
		}
	}

	private void Start()
	{
		_isMoving = false;
		switch (_motionType)
		{
		case MotionType.Rotate:
			_smoothedVelocity.clampDist = 5f;
			break;
		case MotionType.Translate:
		case MotionType.Totem:
			_smoothedVelocity.clampDist = 0.05f;
			break;
		}
		if (_motionType == MotionType.Totem)
		{
			_smoothedVelocity.easeUpTime = 0f;
			_smoothedVelocity.easeDownTime = 0.5f;
		}
		_lastPos = base.transform.position;
		Vector3 localPosition = base.transform.localPosition;
		_lastDragNotch = new Vector3(Mathf.Round(localPosition.x), Mathf.Round(localPosition.y), Mathf.Round(localPosition.z));
		if (!initialised)
		{
			_chordDefinitions = GameObject.Find("Audio").GetComponent<ChordDefinitions>();
		}
		if (chordName == null || chordName.Length < 1)
		{
			chordName = "Minor7";
		}
	}

	[TriggerableAction]
	public IEnumerator MuteAudio()
	{
		mute = true;
		return null;
	}

	[TriggerableAction]
	public IEnumerator UnMuteAudio()
	{
		mute = false;
		return null;
	}

	[TriggerableAction]
	public IEnumerator UnlockEventOverride()
	{
		overrideEvents = false;
		return null;
	}

	private void LateUpdate()
	{
		if (_lastNotificationTime != Time.time)
		{
			UpdateValue(_currentValue, isMoving: false);
			_lastNotificationTime = Time.time;
		}
	}

	public void NotifyRotation(float value, float snapAngle, bool snapping)
	{
		_lastNotificationTime = Time.time;
		_snapAngle = Mathf.Min(snapAngle, 90f);
		UpdateValue(value, isMoving: true, forcePlay: false, snapping);
	}

	public void NotifyBeginRotation(float value, float snapAngle)
	{
		_lastNotificationTime = Time.time;
		_snapAngle = snapAngle;
		UpdateValue(value, isMoving: true, forcePlay: true);
	}

	public void NotifyMove()
	{
		_lastNotificationTime = Time.time;
		float magnitude = (base.transform.position - _lastPos).magnitude;
		UpdateValue(_currentValue + magnitude, isMoving: true);
		_lastPos = base.transform.position;
	}

	public void NotifyMoveInteractive(float magnitude, int direction, int notches, bool forcePlay = false)
	{
		_lastNotificationTime = Time.time;
		_direction = direction;
		_notches = notches;
		UpdateValue(magnitude, isMoving: true, forcePlay);
	}

	private void UpdateValue(float value, bool isMoving, bool forcePlay = false, bool snapping = false)
	{
		if (mute || !base.enabled || !base.gameObject.activeInHierarchy || TriggerAction.FastForward)
		{
			return;
		}
		if (_motionType == MotionType.RotateInteractive)
		{
			_currentValue = value;
			float num = (360f + value % 360f) % 360f;
			float num2 = (snapping ? 2 : 15);
			if (!_cooldown || forcePlay)
			{
				float num3 = Mathf.Round(num / _snapAngle);
				if (num3 >= 360f / _snapAngle)
				{
					num3 = 0f;
				}
				int num4 = root + _chordDefinitions.GetSemiTone(chordName, root + (int)num3);
				int num5 = num4;
				if (secondaryChord != null && secondaryChord.Length > 0)
				{
					num5 = root + _chordDefinitions.GetSemiTone(secondaryChord, root + (int)num3);
				}
				num3 *= _snapAngle;
				float num6 = Sign(Mathf.DeltaAngle(_lastAngle, num3));
				float num7 = Sign(Mathf.DeltaAngle(num, num3));
				if (num6 != num7 || forcePlay)
				{
					_cooldown = true;
					_lastNotch = num3;
					if ((bool)EventManager.Instance)
					{
						string text = audioEvent;
						if (!atonal)
						{
							text = text + "/" + num4;
						}
						if (IsEventValid(audioEvent))
						{
							EventManager.Instance.PostEvent(text, EventAction.PlaySound, base.gameObject);
						}
						text = secondaryEvent;
						if (!atonal)
						{
							text = text + "/" + num5;
						}
						if (IsEventValid(secondaryEvent))
						{
							EventManager.Instance.PostEvent(text, EventAction.PlaySound, base.gameObject);
						}
					}
				}
			}
			else if (Mathf.Abs(Mathf.DeltaAngle(num, _lastNotch)) > num2)
			{
				_cooldown = false;
			}
			_lastAngle = num;
			return;
		}
		if (_motionType == MotionType.TranslateInteractive)
		{
			if (!_cooldown || forcePlay)
			{
				Vector3 localPosition = base.transform.localPosition;
				Vector3 vector = new Vector3(Mathf.Round(localPosition.x), Mathf.Round(localPosition.y), Mathf.Round(localPosition.z));
				float num8 = Vector3.Dot((localPosition - vector).normalized, (_lastPos - vector).normalized);
				float num9 = Mathf.Round(value * (float)_notches);
				num9 = (atonal ? 0f : (num9 % (float)_chordDefinitions.GetNumTones(chordName)));
				if (num8 < 1f || forcePlay)
				{
					_cooldown = true;
					_lastDragNotch = vector;
					int num10 = root + _chordDefinitions.GetSemiTone(chordName, root + (int)num9);
					int num11 = num10;
					if (secondaryChord != null && secondaryChord.Length > 0)
					{
						num11 = root + _chordDefinitions.GetSemiTone(secondaryChord, root + (int)num9);
					}
					if ((bool)EventManager.Instance)
					{
						string text2 = audioEvent;
						if (!atonal)
						{
							text2 = text2 + "/" + num10;
						}
						if (IsEventValid(audioEvent))
						{
							EventManager.Instance.PostEvent(text2, EventAction.PlaySound, base.gameObject);
						}
						text2 = secondaryEvent;
						if (!atonal)
						{
							text2 = text2 + "/" + num11;
						}
						if (IsEventValid(secondaryEvent))
						{
							EventManager.Instance.PostEvent(text2, EventAction.PlaySound, base.gameObject);
						}
					}
				}
			}
			else if (Vector3.Distance(_lastDragNotch, base.transform.localPosition) > 0.15f)
			{
				_cooldown = false;
			}
			_lastPos = base.transform.localPosition;
			return;
		}
		if (_motionType == MotionType.TranslateInteractiveSmooth)
		{
			float num12 = value - _currentValue;
			_currentValue = value;
			_ = num12 / Time.deltaTime;
			isMoving = isMoving || num12 != 0f;
			if (isMoving && !_isMoving)
			{
				if ((bool)EventManager.Instance)
				{
					EventManager.Instance.PostEvent(audioEvent, EventAction.PlaySound, base.gameObject);
				}
				_isMoving = true;
			}
			if (isMoving && (bool)EventManager.Instance)
			{
				EventManager.Instance.SetParameter(audioEvent, "magnitude", value, base.gameObject);
			}
			return;
		}
		float num13 = value - _currentValue;
		_currentValue = value;
		float num14 = num13 / Time.deltaTime;
		isMoving = isMoving || num13 != 0f;
		if (isMoving && !_isMoving)
		{
			if ((bool)EventManager.Instance)
			{
				if (IsEventValid(audioEvent))
				{
					EventManager.Instance.PostEvent(audioEvent, EventAction.PlaySound, base.gameObject);
				}
				if (IsEventValid(secondaryEvent))
				{
					EventManager.Instance.PostEvent(secondaryEvent, EventAction.PlaySound, base.gameObject);
				}
			}
			_isMoving = true;
			num14 = 0f;
			_smoothedVelocity.Reset(num14);
		}
		_smoothedVelocity.target = num14;
		_smoothedVelocity.Advance();
		if (isMoving)
		{
			float num15 = ((_motionType == MotionType.Rotate) ? 720f : 20f);
			float value2 = Mathf.Clamp(Mathf.Abs(_smoothedVelocity.smoothedValue), 0f, 0.99f * num15);
			if (!EventManager.Instance)
			{
				return;
			}
			if (IsEventValid(audioEvent))
			{
				EventManager.Instance.SetParameter(audioEvent, "dragSpeed", value2, base.gameObject);
			}
			if (IsEventValid(secondaryEvent))
			{
				EventManager.Instance.SetParameter(secondaryEvent, "dragSpeed", value2, base.gameObject);
			}
			if (IsEventValid(audioEvent))
			{
				if (MotionType.Rotate == _motionType)
				{
					EventManager.Instance.SetParameter(audioEvent, "angle", (360f + value % 360f) % 360f, base.gameObject);
				}
				else if (MotionType.TranslateInteractive == _motionType)
				{
					EventManager.Instance.SetParameter(audioEvent, "angle", value, base.gameObject);
				}
			}
		}
		else
		{
			if (isMoving || !_isMoving)
			{
				return;
			}
			_isMoving = false;
			if ((bool)EventManager.Instance)
			{
				if (IsEventValid(audioEvent))
				{
					EventManager.Instance.PostEvent(audioEvent, EventAction.StopSound, base.gameObject);
				}
				if (IsEventValid(secondaryEvent))
				{
					EventManager.Instance.PostEvent(secondaryEvent, EventAction.StopSound, base.gameObject);
				}
			}
		}
	}

	public void NotifyStopMoveInteractive()
	{
		if (_motionType == MotionType.TranslateInteractiveSmooth)
		{
			_isMoving = false;
			if ((bool)EventManager.Instance && IsEventValid(audioEvent))
			{
				EventManager.Instance.PostEvent(audioEvent, EventAction.StopSound, base.gameObject);
			}
		}
	}

	public void PlayStopSound(float timeParam, float timeRemaining)
	{
		if (!mute && base.enabled && base.gameObject.activeInHierarchy && !TriggerAction.FastForward && _lastPlayedStopSound + _stopSoundCooldown < Time.time && (bool)EventManager.Instance)
		{
			if (stopEvent != null)
			{
				EventManager.Instance.PostEvent(stopEvent, EventAction.PlaySound, base.gameObject);
				EventManager.Instance.SetParameter(stopEvent, "timeParam", timeParam, base.gameObject);
			}
			_lastPlayedStopSound = Time.time;
			_stopSoundCooldown = Mathf.Max(forceOnlyOneStopSound ? timeRemaining : 0f, 0f) + 0.2f;
		}
	}

	private bool IsEventValid(string eventName)
	{
		if (eventName != null)
		{
			return eventName.Length > 0;
		}
		return false;
	}

	private float Sign(float input)
	{
		if (input == 0f || Mathf.Abs(input) >= 180f)
		{
			return 0f;
		}
		return Mathf.Sign(input);
	}
}
