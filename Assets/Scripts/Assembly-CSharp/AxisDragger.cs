using UnityEngine;

[RequireComponent(typeof(Draggable))]
public class AxisDragger : Dragger
{
	private float dragCoefficient;

	public bool dragX;

	public float xMin;

	public float xMax;

	public bool dragY;

	public float yMin;

	public float yMax;

	public bool dragZ;

	public float zMin;

	public float zMax;

	public Transform[] points;

	public bool doCharacterCollision;

	public float maxSpeed = 40f;

	public DragHistory dragHistory = new DragHistory(1f, 1f);

	public NotchedSlider notchedSliderX = new NotchedSlider(wrapping: false, 0f, 0f, 1f);

	public NotchedSlider notchedSliderY = new NotchedSlider(wrapping: false, 0f, 0f, 1f);

	public NotchedSlider notchedSliderZ = new NotchedSlider(wrapping: false, 0f, 0f, 1f);

	public bool disableBounce;

	private Vector3 startPosition;

	private Vector3 dragPosition;

	private Vector3 targetPosition;

	private Vector3 offset;

	private BaseLocomotion[] _collidableEntities;

	private BoxCollider[] _childColliders;

	private bool _handlesActive = true;

	private MoverAudio _moverAudio;

	private float _lastParam = -1f;

	public override GameObject targetObject => base.gameObject;

	private void Awake()
	{
		dragCoefficient = 0.074f;
		if (Screen.height > 1024)
		{
			dragCoefficient *= 0.5f;
		}
		_moverAudio = GetComponent<MoverAudio>();
	}

	protected void Start()
	{
		startPosition = base.transform.localPosition;
		dragPosition = startPosition;
		targetPosition = startPosition;
		if (doCharacterCollision)
		{
			_childColliders = base.gameObject.GetComponentsInChildren<BoxCollider>();
			_collidableEntities = Object.FindObjectsOfType(typeof(BaseLocomotion)) as BaseLocomotion[];
		}
		if (points == null || points.Length == 0)
		{
			return;
		}
		xMin = float.MaxValue;
		xMax = float.MinValue;
		yMin = float.MaxValue;
		yMax = float.MinValue;
		zMin = float.MaxValue;
		zMax = float.MinValue;
		Vector3 zero = Vector3.zero;
		Transform[] array = points;
		foreach (Transform transform in array)
		{
			zero = base.transform.parent.InverseTransformPoint(transform.position);
			if (zero.x < xMin)
			{
				xMin = zero.x;
			}
			if (zero.x > xMax)
			{
				xMax = zero.x;
			}
			if (zero.y < yMin)
			{
				yMin = zero.y;
			}
			if (zero.y > yMax)
			{
				yMax = zero.y;
			}
			if (zero.z < zMin)
			{
				zMin = zero.z;
			}
			if (zero.z > zMax)
			{
				zMax = zero.z;
			}
		}
		float num = 0.25f;
		xMin = num * Mathf.Round(xMin / num);
		xMax = num * Mathf.Round(xMax / num);
		yMin = num * Mathf.Round(yMin / num);
		yMax = num * Mathf.Round(yMax / num);
		zMin = num * Mathf.Round(zMin / num);
		zMax = num * Mathf.Round(zMax / num);
	}

	public override void StartDrag(Vector3 position)
	{
		if (!targetDraggable || targetDraggable.AllowDrag())
		{
			bool num = !base.snapping && !base.dragging;
			base.StartDrag(position);
			if ((bool)_moverAudio && !_moverAudio.overrideEvents)
			{
				MoverAudioPresets.UpdateMoverWithPreset(_moverAudio, MoverAudioPresets.MoverAudioPresetType.InteractiveDragger);
			}
			startPosition = base.transform.localPosition;
			offset = Camera.main.WorldToScreenPoint(base.gameObject.transform.position) - position;
			dragPosition = startPosition;
			targetPosition = startPosition;
			dragHistory.ClearMomentum();
			if (num)
			{
				UpdateAudio(forcePlay: true);
			}
		}
	}

	public override void Drag(Vector3 position, Vector3 delta)
	{
		if ((bool)targetDraggable && !targetDraggable.AllowDrag())
		{
			targetPosition = base.transform.localPosition;
			return;
		}
		_snapping = false;
		_dragging = true;
		Ray ray = Camera.main.ScreenPointToRay(position + offset);
		float enter = 0f;
		Vector3 direction = Vector3.forward;
		if (dragZ)
		{
			direction = Vector3.down;
		}
		if (dragZ && dragY)
		{
			direction = Vector3.right;
		}
		if (new Plane(base.transform.TransformDirection(direction), base.transform.position).Raycast(ray, out enter))
		{
			Vector3 point = ray.GetPoint(enter);
			Vector3 vector = (base.transform.parent ? base.transform.parent.InverseTransformPoint(point) : point);
			float x = base.transform.localPosition.x;
			float y = base.transform.localPosition.y;
			float z = base.transform.localPosition.z;
			if (dragX)
			{
				x = Mathf.Clamp(vector.x, xMin, xMax);
			}
			if (dragY)
			{
				y = Mathf.Clamp(vector.y, yMin, yMax);
			}
			if (dragZ)
			{
				z = Mathf.Clamp(vector.z, zMin, zMax);
			}
			Vector3 vector2 = (base.transform.parent ? base.transform.parent.TransformPoint(new Vector3(x, y, z)) : new Vector3(x, y, z));
			if (doCharacterCollision)
			{
				if (!DoCollisionCheck(vector2 - base.transform.position))
				{
					dragPosition = vector2;
				}
			}
			else
			{
				dragPosition = vector2;
			}
		}
		dragPosition = base.transform.parent.InverseTransformPoint(dragPosition);
		targetPosition = new Vector3(Mathf.Round(dragPosition.x), Mathf.Round(dragPosition.y), Mathf.Round(dragPosition.z));
	}

	protected bool DoCollisionCheck(Vector3 deltaPos)
	{
		float num = 1f;
		Vector3 vector = deltaPos;
		vector.Normalize();
		for (int i = -1; i < _childColliders.Length; i++)
		{
			Vector3 position = base.transform.position;
			position = ((i >= 0) ? _childColliders[i].transform.position : base.transform.position);
			float num2 = Vector3.Dot(vector, position);
			float num3 = Vector3.Dot(vector, position + deltaPos);
			BaseLocomotion[] collidableEntities = _collidableEntities;
			foreach (BaseLocomotion baseLocomotion in collidableEntities)
			{
				float num4 = Vector3.Dot(vector, baseLocomotion.transform.position);
				float num5 = 1f;
				if ((num4 > num2 - num5 && num4 < num3 + num5) || (num4 < num2 + num5 && num4 > num3 - num5))
				{
					Vector3 vector2 = baseLocomotion.transform.position - vector * num4;
					Vector3 vector3 = position - vector * num2;
					if ((vector2 - vector3).sqrMagnitude < num)
					{
						return true;
					}
				}
			}
		}
		return false;
	}

	public void OnDrawGizmos()
	{
	}

	public override void Snap()
	{
		base.Snap();
		_snapping = true;
		_dragging = false;
		dragHistory.DecayMomentum(0f);
		if (dragX)
		{
			StartSlider(notchedSliderX, targetObject.transform.localPosition.x, dragHistory.momentum.x, xMin, xMax);
			if (disableBounce)
			{
				notchedSliderX.DisableEdgeBounce();
			}
		}
		if (dragY)
		{
			StartSlider(notchedSliderY, targetObject.transform.localPosition.y, dragHistory.momentum.y, yMin, yMax);
			if (disableBounce)
			{
				notchedSliderY.DisableEdgeBounce();
			}
		}
		if (dragZ)
		{
			StartSlider(notchedSliderZ, targetObject.transform.localPosition.z, dragHistory.momentum.z, zMin, zMax);
			if (disableBounce)
			{
				notchedSliderZ.DisableEdgeBounce();
			}
		}
	}

	private void StartSlider(NotchedSlider notchedSlider, float pos, float speed, float min, float max)
	{
		notchedSlider.pos = pos;
		notchedSlider.vel = speed;
		notchedSlider.min = min;
		notchedSlider.max = max;
		notchedSlider.Start();
	}

	private void Update()
	{
		if (base.dragging)
		{
			Vector3 vector = dragPosition - base.transform.localPosition;
			if (!dragX)
			{
				vector.x = 0f;
			}
			if (!dragY)
			{
				vector.y = 0f;
			}
			if (!dragZ)
			{
				vector.z = 0f;
			}
			float num = Mathf.Abs(vector.magnitude);
			float num2 = maxSpeed * Time.deltaTime;
			if (num > num2)
			{
				vector *= num2 / num;
			}
			base.transform.localPosition += vector;
			if (Vector3.Distance(base.transform.localPosition, targetPosition) < 0.1f)
			{
				base.transform.localPosition = targetPosition;
			}
			dragHistory.AddDatum(Time.time, base.transform.localPosition);
			GlowFull();
		}
		else
		{
			GlowDecrease();
		}
		if (base.snapping)
		{
			Vector3 localPosition = base.transform.localPosition;
			if (dragX)
			{
				localPosition.x = notchedSliderX.Advance(base.transform.parent, new Vector3(1f, 0f, 0f));
			}
			if (dragY)
			{
				localPosition.y = notchedSliderY.Advance(base.transform.parent, new Vector3(0f, 1f, 0f));
			}
			if (dragZ)
			{
				localPosition.z = notchedSliderZ.Advance(base.transform.parent, new Vector3(0f, 0f, 1f));
			}
			base.transform.localPosition = localPosition;
			if (notchedSliderX.stationary && notchedSliderY.stationary && notchedSliderZ.stationary)
			{
				_lastParam = -1f;
				EndSnapping();
				if ((bool)_moverAudio)
				{
					_moverAudio.NotifyStopMoveInteractive();
				}
			}
		}
		if ((bool)targetDraggable && !targetDraggable.AllowDrag())
		{
			if (base.dragging)
			{
				CancelDrag();
			}
			if (_handlesActive)
			{
				_handlesActive = false;
				PlayAnimationInChildren("Close");
			}
		}
		else if (!_handlesActive)
		{
			_handlesActive = true;
			PlayAnimationInChildren("Open");
		}
		if (base.snapping || base.dragging)
		{
			UpdateAudio(forcePlay: false);
		}
	}

	private void UpdateAudio(bool forcePlay)
	{
		if ((bool)_moverAudio)
		{
			float num = 0f;
			float num2 = 0f;
			if (dragX)
			{
				num = (base.transform.localPosition.x - xMin) / (xMax - xMin);
				num2 = xMax - xMin;
			}
			else if (dragY)
			{
				num = (base.transform.localPosition.y - yMin) / (yMax - yMin);
				num2 = yMax - yMin;
			}
			else if (dragZ)
			{
				num = (base.transform.localPosition.z - zMin) / (zMax - zMin);
				num2 = zMax - zMin;
			}
			if (_lastParam < 0f)
			{
				_lastParam = num;
			}
			int direction = ((num > _lastParam) ? 1 : ((num < _lastParam) ? (-1) : 0));
			_moverAudio.NotifyMoveInteractive(num, direction, (int)num2, forcePlay);
			_lastParam = num;
		}
	}

	public bool AtMinimum()
	{
		if (dragX)
		{
			if (base.transform.localPosition.x == xMin)
			{
				return true;
			}
		}
		else if (dragY)
		{
			if (base.transform.localPosition.y == yMin)
			{
				return true;
			}
		}
		else if (base.transform.localPosition.z == zMin)
		{
			return true;
		}
		return false;
	}

	public override void CancelDrag()
	{
		Snap();
	}

	private void PlayAnimationInChildren(string animationName)
	{
		Animation[] componentsInChildren = GetComponentsInChildren<Animation>();
		foreach (Animation animation in componentsInChildren)
		{
			AnimationState animationState = animation[animationName];
			if ((bool)animationState)
			{
				animation.Stop();
				animationState.time = 0f;
				animation.Play(animationName);
			}
		}
	}
}
