using UnityEngine;

public class Rotatable : Draggable
{
	public Vector3 axis = new Vector3(0f, 1f, 0f);

	public AnimationCurveDefinition snapAnimation;

	public float snapAngle = 90f;

	public bool limitRotation;

	public float maxRotation;

	public float minRotation;

	public bool softConstraint = true;

	public float softConstraintAmount = 20f;

	public float[] customSnapPositions;

	public float maxRotationSpeed = 1080f;

	public DragHistory dragHistory;

	public NotchedSlider notchedSlider = new NotchedSlider(wrapping: true, 0f, 360f, 90f);

	protected Quaternion _initialRotation;

	protected float _currentAngle;

	private float _snapStartAngle;

	private float _snapEndAngle;

	private float _snapStartTime;

	private MoverAudio _moverAudio;

	public float currentAngle
	{
		get
		{
			return _currentAngle;
		}
		set
		{
			ApplyAngle(value);
		}
	}

	private Rotatable()
	{
	}

	protected new void Start()
	{
		base.Start();
		_moverAudio = GetComponent<MoverAudio>();
		_initialRotation = base.transform.localRotation;
		if (snapAnimation == null)
		{
			GameObject gameObject = GameObject.Find("RotateSnapCurve");
			if ((bool)gameObject && (bool)gameObject.GetComponent<AnimationCurveDefinition>())
			{
				snapAnimation = gameObject.GetComponent<AnimationCurveDefinition>();
			}
		}
	}

	protected void Update()
	{
		if (base.snapping)
		{
			ApplyAngle(notchedSlider.Advance(base.transform.parent, axis));
			if (notchedSlider.stationary)
			{
				_snapEndAngle = notchedSlider.pos;
				EndSnapping();
			}
		}
		if (base.dragging)
		{
			dragHistory.AddDatum(Time.time, new Vector3(_currentAngle, 0f, 0f));
		}
	}

	public override void StartDrag()
	{
		bool flag = base.isStationary;
		base.StartDrag();
		if ((bool)_moverAudio)
		{
			if (!_moverAudio.overrideEvents)
			{
				MoverAudioPresets.UpdateMoverWithPreset(_moverAudio, MoverAudioPresets.MoverAudioPresetType.InteractiveRotator);
			}
			if (flag)
			{
				_moverAudio.NotifyBeginRotation(_currentAngle, snapAngle);
			}
		}
	}

	public virtual void ApplyAngle(float angle)
	{
		float num = _currentAngle;
		if (angle != _currentAngle || num < minRotation || num > maxRotation)
		{
			if (limitRotation)
			{
				float num2 = (softConstraint ? softConstraintAmount : 0f);
				_currentAngle = Mathf.Clamp(angle, minRotation - num2, maxRotation + num2);
			}
			else
			{
				_currentAngle = angle;
			}
			Quaternion quaternion = Quaternion.AngleAxis(_currentAngle, axis);
			Quaternion localRotation = _initialRotation * quaternion;
			base.transform.localRotation = localRotation;
		}
		if ((_currentAngle != num || base.dragging) && (bool)_moverAudio)
		{
			_moverAudio.NotifyRotation(_currentAngle, snapAngle, base.snapping);
		}
	}

	public override void Snap()
	{
		if (!base.snapping)
		{
			base.Snap();
			dragHistory.DecayMomentum(0f);
			notchedSlider.pos = _currentAngle;
			notchedSlider.vel = dragHistory.momentum.x;
			if (limitRotation)
			{
				float num = (softConstraint ? softConstraintAmount : 0f);
				notchedSlider.origin = minRotation;
				notchedSlider.min = minRotation - num;
				notchedSlider.max = maxRotation + num;
				notchedSlider.wrapping = false;
			}
			else
			{
				notchedSlider.min = 0f;
				notchedSlider.max = 360f;
				notchedSlider.wrapping = true;
			}
			notchedSlider.notchSeparation = snapAngle;
			notchedSlider.Start();
		}
	}

	public override void EndSnapping()
	{
		if (_snapping)
		{
			base.EndSnapping();
			ApplyAngle(_snapEndAngle);
		}
	}

	protected void SnapFromTo(float start, float end)
	{
		_snapStartAngle = start;
		_snapEndAngle = end;
		_snapStartTime = Time.time;
	}

	public void CancelDrag()
	{
		dragHistory.ClearMomentum();
		notchedSlider.pos = currentAngle;
		notchedSlider.vel = 0f;
	}

	public bool AllowRotation()
	{
		return AllowDrag();
	}

	protected static float LerpAngleNoClamp(float start, float end, float param)
	{
		if (Mathf.Abs(start - end) > 180f)
		{
			end = ((!(end > start)) ? (end + 360f) : (end - 360f));
		}
		return start + param * (end - start);
	}

	public static float GetRotationComponentForAxis(Quaternion rotation, Vector3 axis)
	{
		Vector3 vector = Vector3.Cross(axis, new Vector3(1f, 0f, 0f));
		if (vector.magnitude == 0f)
		{
			vector = Vector3.Cross(axis, new Vector3(0f, 0f, 1f));
		}
		Vector3 vector2 = rotation * vector;
		Vector3 vector3 = vector2 - Vector3.Dot(vector2, axis) * axis;
		vector3.Normalize();
		float num = Vector3.Angle(vector, vector3);
		if (Vector3.Dot(Vector3.Cross(vector, vector3), axis) < 0f)
		{
			num *= -1f;
		}
		return num;
	}

	public bool AtMinimum()
	{
		if (Mathf.Abs(_currentAngle - minRotation) < 0.2f)
		{
			return true;
		}
		return false;
	}

	public bool AtMaximum()
	{
		if (Mathf.Abs(_currentAngle - maxRotation) < 0.2f)
		{
			return true;
		}
		return false;
	}
}
