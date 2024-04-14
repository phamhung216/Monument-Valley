using System.Collections;
using UnityEngine;

public class CrowFinale : MonoBehaviour
{
	public enum AnimState
	{
		None = 0,
		Idle = 1,
		IdleLooking = 2,
		WalkFlat = 3,
		WalkUp = 4,
		WalkDown = 5,
		Melt = 6
	}

	public Animation animSystem;

	public float characterVel = 1f;

	public int walkUnits = 2;

	public int numSteps = 1;

	private AnimState _prevState;

	private AnimState _animState;

	private float _interp;

	private Vector3 _fromPos;

	private Vector3 _toPos;

	public Transform HeadBone;

	private Quaternion prevLookDir;

	public Transform itemToLookAt;

	public float headRotationSpeed = 0.3f;

	public ParticleSystem curseLiftingLong;

	public ParticleSystem curseLiftingShort;

	public ParticleSystem curseLifted;

	public Animation realCrow;

	public AnimationCurveDefinition animationCurve;

	public float headTurnAngle = 135f;

	public float headTurnSpeed = 0.05f;

	public ActionSequence curseSequence;

	private bool _moveIntoCentre;

	private float _startTime;

	private float _duration = 4f;

	private void Start()
	{
		_fromPos = base.transform.position;
		_toPos = base.transform.position + base.transform.forward;
		animSystem["Walk"].wrapMode = WrapMode.Loop;
		animSystem["Idle"].wrapMode = WrapMode.Once;
		animSystem["WalkDown"].wrapMode = WrapMode.Once;
		animSystem["WalkUp"].wrapMode = WrapMode.Once;
		animSystem["Melt"].wrapMode = WrapMode.Once;
		_prevState = AnimState.None;
		_animState = AnimState.Idle;
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
	public IEnumerator StartMoving()
	{
		_animState = AnimState.WalkFlat;
		return null;
	}

	[TriggerableAction]
	public IEnumerator TriggerMelt()
	{
		_animState = AnimState.None;
		StartCoroutine(curseSequence.DoSequence());
		return null;
	}

	[TriggerableAction]
	public IEnumerator MoveIntoCentre()
	{
		_moveIntoCentre = true;
		_fromPos = base.transform.localPosition;
		_toPos = Vector3.zero;
		_startTime = Time.time;
		return null;
	}

	[TriggerableAction]
	public IEnumerator StartHeadLook()
	{
		_animState = AnimState.IdleLooking;
		return null;
	}

	private void Update()
	{
		if (_moveIntoCentre)
		{
			float num = ((_duration <= 0f) ? 1f : ((Time.time - _startTime) / _duration));
			float t = animationCurve.curve.Evaluate(num);
			if (num >= 1f)
			{
				t = 1f;
				_moveIntoCentre = false;
			}
			base.transform.localPosition = Vector3.Lerp(_fromPos, _toPos, t);
		}
	}

	private void LateUpdate()
	{
		switch (_animState)
		{
		case AnimState.Idle:
			UpdateIdleState();
			break;
		case AnimState.IdleLooking:
			UpdateIdleLookingState();
			break;
		case AnimState.WalkFlat:
			UpdateWalkState();
			break;
		case AnimState.WalkDown:
			UpdateWalkDownState();
			break;
		case AnimState.WalkUp:
			UpdateWalkUpState();
			break;
		case AnimState.Melt:
			UpdateMeltState();
			break;
		}
		prevLookDir = HeadBone.transform.rotation;
	}

	private void UpdateIdleState()
	{
		if (_prevState != AnimState.Idle)
		{
			AutoPositionAfterAnimation();
			_prevState = AnimState.Idle;
			animSystem["Idle"].speed = Random.Range(0.75f, 1.25f);
			animSystem.Play("Idle");
		}
		animSystem.Sample();
		if (!animSystem.isPlaying)
		{
			animSystem.Play("Idle");
		}
	}

	private void UpdateIdleLookingState()
	{
		if (itemToLookAt != null)
		{
			Vector3 vector = itemToLookAt.transform.position - base.transform.position;
			Quaternion b = HeadBone.transform.rotation;
			Vector3 forward = HeadBone.transform.forward;
			Vector3 forward2 = base.transform.forward;
			Vector3 normalized = vector.normalized;
			if (Vector3.Dot(forward2, normalized) > 1f - headTurnAngle / 180f && Vector3.Dot(forward, forward2) > 0.25f)
			{
				b = Quaternion.LookRotation(normalized, HeadBone.transform.up);
			}
			HeadBone.transform.rotation = Quaternion.Lerp(prevLookDir, b, headTurnSpeed);
			prevLookDir = HeadBone.transform.rotation;
		}
	}

	private void UpdateWalkState()
	{
		if (_prevState != AnimState.WalkFlat)
		{
			AutoPositionAfterAnimation();
			_prevState = _animState;
			animSystem.Play("Walk");
		}
		animSystem["Walk"].speed = 0.5f * characterVel;
		animSystem.Sample();
		if (_interp < 1f)
		{
			base.transform.position = _fromPos + (_toPos - _fromPos) * _interp;
		}
		_interp += Time.deltaTime * characterVel;
		if (_interp > 1f)
		{
			walkUnits--;
			if (walkUnits <= 0)
			{
				base.transform.position = _toPos;
				_animState = AnimState.WalkDown;
			}
			else
			{
				_interp -= 1f;
				_fromPos = _toPos;
				_toPos += base.transform.forward;
			}
		}
	}

	private void UpdateWalkDownState()
	{
		if (_prevState != AnimState.WalkDown)
		{
			AutoPositionAfterAnimation();
			_prevState = AnimState.WalkDown;
			animSystem.Play("WalkDown");
		}
		animSystem.Sample();
		if (!animSystem.isPlaying)
		{
			numSteps--;
			if (numSteps <= 0)
			{
				_animState = AnimState.Idle;
			}
			else
			{
				_animState = AnimState.WalkUp;
			}
		}
	}

	private void UpdateWalkUpState()
	{
		if (_prevState != AnimState.WalkUp)
		{
			AutoPositionAfterAnimation();
			_prevState = AnimState.WalkUp;
			animSystem.Play("WalkUp");
		}
		animSystem.Sample();
		if (!animSystem.isPlaying)
		{
			_animState = AnimState.WalkDown;
		}
	}

	private void UpdateMeltState()
	{
		if (_prevState != AnimState.Melt)
		{
			_prevState = AnimState.Melt;
			animSystem.Play("Melt");
		}
		animSystem.Sample();
	}

	private void AutoPositionAfterAnimation()
	{
		if (_prevState == AnimState.WalkDown)
		{
			base.transform.position += (base.transform.forward - base.transform.up) * 0.5f;
			base.transform.Rotate(base.transform.right, 90f, Space.World);
		}
		if (_prevState == AnimState.WalkUp)
		{
			base.transform.position += (base.transform.forward + base.transform.up) * 0.5f;
			base.transform.Rotate(base.transform.right, -90f, Space.World);
		}
	}

	private void OnDrawGizmos()
	{
		if (_animState == AnimState.None)
		{
			Gizmos.color = Color.green;
			Vector3 position = base.transform.position;
			for (int i = 0; i < walkUnits; i++)
			{
				Gizmos.DrawLine(position, position + base.transform.forward);
				position += base.transform.forward;
			}
			Gizmos.DrawLine(position, position + base.transform.forward * 0.5f);
			Gizmos.DrawLine(position + base.transform.forward * 0.5f, position + (base.transform.forward - base.transform.up) * 0.5f);
			position += ((float)numSteps - 0.5f) * (base.transform.forward - base.transform.up);
			Gizmos.DrawSphere(position, 0.25f);
		}
	}
}
