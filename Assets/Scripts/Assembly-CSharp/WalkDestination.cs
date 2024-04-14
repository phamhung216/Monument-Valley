using System.Collections;
using UnityEngine;

public class WalkDestination : MonoBehaviour
{
	public NavBrushComponent destinationBrush;

	private PlayerInput _playerInput;

	private bool _pendingArrival;

	private static string s_iconTexture = "GizmoAutoWalk";

	private void Start()
	{
		if (null == destinationBrush)
		{
			destinationBrush = GetComponent<NavBrushComponent>();
		}
		GameScene.instance.EnsurePlayer();
		_playerInput = GameScene.player.GetComponent<PlayerInput>();
	}

	private void Update()
	{
		if (_pendingArrival)
		{
			if (_playerInput.GetComponent<CharacterLocomotion>().lastValidBrush == destinationBrush)
			{
				_playerInput.SetControlSource(PlayerInput.ControlSource.User);
				_pendingArrival = false;
			}
			else if (_playerInput.navRequest.status == NavRequest.RequestStatus.Complete && (_playerInput.navRequest.route == null || _playerInput.navRequest.route.Count == 0))
			{
				_playerInput.MoveTo(destinationBrush);
			}
		}
	}

	[TriggerableAction]
	public IEnumerator WalkHere()
	{
		_playerInput.SetControlSource(PlayerInput.ControlSource.Game);
		_playerInput.MoveTo(destinationBrush);
		_pendingArrival = true;
		return null;
	}

	private void OnDrawGizmos()
	{
		Gizmos.DrawIcon(base.transform.position, s_iconTexture);
		if ((bool)destinationBrush)
		{
			Gizmos.color = Color.blue;
			Gizmos.DrawLine(base.transform.position, destinationBrush.transform.position);
		}
	}
}
