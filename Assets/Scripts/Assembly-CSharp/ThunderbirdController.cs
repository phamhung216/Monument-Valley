using System.Collections;
using UnityEngine;

public class ThunderbirdController : MonoBehaviour
{
	public enum AnimState
	{
		None = 0,
		Idle = 1,
		Born = 2,
		PickUpIda = 3,
		TakeOff = 4,
		Fly = 5
	}

	public Animation animSystem;

	private SkinnedMeshRenderer _thunderbirdRenderer;

	private AnimState _prevState;

	private AnimState _animState;

	private void Start()
	{
		_prevState = AnimState.None;
		_animState = AnimState.None;
		_thunderbirdRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
		_thunderbirdRenderer.gameObject.SetActive(value: false);
		animSystem["Idle"].wrapMode = WrapMode.Loop;
	}

	private void LateUpdate()
	{
		UpdateStates();
		animSystem.Sample();
	}

	private void UpdateStates()
	{
		switch (_animState)
		{
		case AnimState.Idle:
			UpdateIdleState();
			break;
		case AnimState.Born:
			UpdateBornState();
			break;
		case AnimState.PickUpIda:
			UpdatePickUpIdaState();
			break;
		case AnimState.TakeOff:
			UpdateTakeOffState();
			break;
		case AnimState.None:
			break;
		}
	}

	[TriggerableAction]
	public IEnumerator Born()
	{
		_animState = AnimState.Born;
		return null;
	}

	[TriggerableAction]
	public IEnumerator PickUpIda()
	{
		_animState = AnimState.PickUpIda;
		return null;
	}

	private void UpdateIdleState()
	{
		UpdateAnimation("Idle");
	}

	private void UpdateBornState()
	{
		_thunderbirdRenderer.gameObject.SetActive(value: true);
		UpdateAnimation("Born");
		if (!animSystem.isPlaying)
		{
			_animState = AnimState.Idle;
		}
	}

	private void UpdatePickUpIdaState()
	{
		UpdateAnimation("PickUpIda");
		if (!animSystem.isPlaying)
		{
			_animState = AnimState.TakeOff;
		}
	}

	private void UpdateTakeOffState()
	{
		UpdateAnimation("TakeOff");
		if (!animSystem.isPlaying)
		{
			_animState = AnimState.None;
		}
	}

	private void UpdateAnimation(string animationName)
	{
		if (_prevState != _animState)
		{
			_prevState = _animState;
			animSystem.Play(animationName);
		}
	}
}
