using System.Collections;
using UnityCommon;
using UnityEngine;

public class VolcanoHintLogic : MonoBehaviour
{
	private bool _idaHasMoved;

	private float _lastTimeIdaMoved;

	private bool _draggerHasMoved;

	private float _lastTimeDraggerMoved;

	private CharacterLocomotion _ida;

	public UILayout levelPage;

	public float hintTime;

	public AutoInterp moveHintAlpha;

	public AutoInterp dragHintAlpha;

	private bool _walkHintModePending;

	private bool _walkHintMode;

	private bool _dragHintMode;

	public UIText moveHintText;

	public string moveHintStringID_mobile;

	public string moveHintStringID_pc;

	private void Start()
	{
		_idaHasMoved = false;
		_lastTimeIdaMoved = Time.time;
		_lastTimeDraggerMoved = Time.time;
		_ida = GameScene.player.GetComponent<CharacterLocomotion>();
		moveHintText.SetText(OrientationOverrideManager.IsLandscape() ? moveHintStringID_pc : moveHintStringID_mobile);
	}

	[TriggerableAction]
	public IEnumerator StartWalkHintMode()
	{
		_walkHintModePending = true;
		return null;
	}

	[TriggerableAction]
	public IEnumerator EndWalkHintMode()
	{
		_walkHintModePending = false;
		_walkHintMode = false;
		return null;
	}

	[TriggerableAction]
	public IEnumerator IdaHasMoved()
	{
		_idaHasMoved = true;
		return null;
	}

	[TriggerableAction]
	public IEnumerator StartDragHintMode()
	{
		_dragHintMode = true;
		_lastTimeDraggerMoved = Time.time;
		return null;
	}

	[TriggerableAction]
	public IEnumerator EndDragHintMode()
	{
		_dragHintMode = false;
		return null;
	}

	[TriggerableAction]
	public IEnumerator DraggerHasMoved()
	{
		_draggerHasMoved = true;
		return null;
	}

	private void Update()
	{
		if (_walkHintModePending && levelPage.opacity > 0.98f)
		{
			_walkHintModePending = false;
			_walkHintMode = true;
			_lastTimeIdaMoved = Time.time;
		}
		if (_walkHintMode)
		{
			if (_ida.locoState != BaseLocomotion.LocomotionState.LocoIdle && _ida.locoState != BaseLocomotion.LocomotionState.LocoStairsIdle)
			{
				_lastTimeIdaMoved = Time.time;
			}
			if (!_idaHasMoved)
			{
				if (Time.time > _lastTimeIdaMoved + hintTime && moveHintAlpha.interpAmount == 0f && !moveHintAlpha.snapping)
				{
					moveHintAlpha.Interp();
				}
			}
			else if (moveHintAlpha.interpAmount == 1f && !moveHintAlpha.reverting && !moveHintAlpha.snapping)
			{
				moveHintAlpha.ReverseInterp();
			}
		}
		if (!_dragHintMode)
		{
			return;
		}
		if (!_draggerHasMoved)
		{
			if (Time.time > _lastTimeDraggerMoved + hintTime && dragHintAlpha.interpAmount == 0f && !dragHintAlpha.snapping)
			{
				dragHintAlpha.Interp();
			}
		}
		else if (dragHintAlpha.interpAmount == 1f && !dragHintAlpha.reverting && !dragHintAlpha.snapping)
		{
			dragHintAlpha.ReverseInterp();
		}
	}
}
