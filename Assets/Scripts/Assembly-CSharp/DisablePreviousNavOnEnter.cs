using System.Collections;
using UnityEngine;

public class DisablePreviousNavOnEnter : MonoBehaviour
{
	public NavBrushComponent targetBrush;

	private CharacterLocomotion _player;

	private bool _navDisabled;

	[TriggerableAction]
	public IEnumerator FastForward()
	{
		_navDisabled = true;
		return null;
	}

	private void Update()
	{
		if (!_player)
		{
			_player = GameScene.player.GetComponent<CharacterLocomotion>();
		}
		if (!_navDisabled && _player.targetBrush == targetBrush)
		{
			_player.lastValidBrush.touchable = false;
			_player.lastValidBrush.discarded = true;
			_navDisabled = true;
		}
	}

	private void OnDrawGizmos()
	{
		if ((bool)targetBrush)
		{
			Gizmos.color = Color.red;
			Gizmos.DrawLine(base.transform.position, targetBrush.transform.position);
			Gizmos.DrawWireSphere(base.transform.position, 0.4f);
		}
	}
}
