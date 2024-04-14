using UnityEngine;

public class NavCameraFollower : MonoBehaviour
{
	public Camera navCamera;

	private PlayerInput _playerInput;

	private void FindPlayerInput()
	{
		if (_playerInput == null)
		{
			_playerInput = GameScene.player.GetComponent<PlayerInput>();
		}
	}

	private void Update()
	{
		FindPlayerInput();
		if (_playerInput != null)
		{
			navCamera.transform.position = _playerInput.currentCamera.transform.position;
			navCamera.transform.rotation = _playerInput.currentCamera.transform.rotation;
			navCamera.orthographicSize = _playerInput.currentCamera.orthographicSize;
		}
	}
}
