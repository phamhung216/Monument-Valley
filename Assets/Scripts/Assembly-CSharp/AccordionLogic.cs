using System.Collections;
using UnityEngine;

public class AccordionLogic : MonoBehaviour
{
	public AxisDragger dragger;

	private Draggable _draggable;

	private CharacterLocomotion _player;

	private bool _triggerEntered;

	[TriggerableAction]
	public IEnumerator EnterTriggerZone()
	{
		_triggerEntered = true;
		return null;
	}

	[TriggerableAction]
	public IEnumerator ExitTriggerZone()
	{
		_triggerEntered = false;
		return null;
	}

	private void Start()
	{
		_draggable = dragger.targetDraggable;
	}

	private void LateUpdate()
	{
		FindPlayer();
		if (_triggerEntered && _player.locoState == BaseLocomotion.LocomotionState.LocoIdle)
		{
			_draggable.dragEnabled = true;
			return;
		}
		if (dragger.dragging)
		{
			dragger.CancelDrag();
		}
		_draggable.dragEnabled = false;
	}

	private void FindPlayer()
	{
		if (_player == null)
		{
			_player = GameScene.player.GetComponent<CharacterLocomotion>();
		}
	}
}
