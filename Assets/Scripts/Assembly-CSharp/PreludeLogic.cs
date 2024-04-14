using System.Collections;
using UnityCommon;
using UnityEngine;

public class PreludeLogic : MonoBehaviour
{
	private bool _hasMoved;

	private float _lastTimeIdaMoved;

	private float _lastTimeRotatorMoved;

	private CharacterLocomotion ida;

	public ProximityTrigger rotatorInCorrectPosition;

	public ProximityTrigger bridgeCrossed;

	public float hintTime;

	public Rotatable rotator;

	public AutoInterp moveHintAlpha;

	public AutoInterp rotateHintAlpha;

	public UIText moveHintText;

	public string moveHintStringID_mobile;

	public string moveHintStringID_pc;

	private void Start()
	{
		_hasMoved = false;
		_lastTimeIdaMoved = Time.time;
		_lastTimeRotatorMoved = Time.time;
		if (GameScene.player != null)
		{
			ida = GameScene.player.GetComponent<CharacterLocomotion>();
		}
		moveHintText.SetText(OrientationOverrideManager.IsLandscape() ? moveHintStringID_pc : moveHintStringID_mobile);
	}

	[TriggerableAction]
	public IEnumerator IdaHasMoved()
	{
		_hasMoved = true;
		return null;
	}

	private void Update()
	{
		if (ida == null)
		{
			ida = GameScene.player.GetComponent<CharacterLocomotion>();
		}
		bool flag = ida.locoState != BaseLocomotion.LocomotionState.LocoIdle && ida.locoState != BaseLocomotion.LocomotionState.LocoStairsIdle;
		if (rotator.dragging && !rotatorInCorrectPosition.triggered)
		{
			_lastTimeRotatorMoved = Time.time;
		}
		if (flag)
		{
			_lastTimeIdaMoved = Time.time;
		}
		float num = ((rotatorInCorrectPosition.triggered && !rotator.dragging) ? (Time.time - _lastTimeRotatorMoved) : (-10000f));
		if (!_hasMoved || (!flag && num > hintTime))
		{
			if (Time.time > _lastTimeIdaMoved + hintTime && moveHintAlpha.interpAmount == 0f && !moveHintAlpha.snapping)
			{
				moveHintAlpha.Interp();
			}
			return;
		}
		if (moveHintAlpha.interpAmount == 1f && !moveHintAlpha.reverting)
		{
			moveHintAlpha.ReverseInterp();
		}
		if (!rotatorInCorrectPosition.triggered && !bridgeCrossed.triggered)
		{
			if (rotateHintAlpha.interpAmount == 0f && !moveHintAlpha.snapping)
			{
				rotateHintAlpha.Interp();
			}
		}
		else if (rotateHintAlpha.interpAmount == 1f && !rotateHintAlpha.reverting)
		{
			rotateHintAlpha.ReverseInterp();
		}
	}
}
