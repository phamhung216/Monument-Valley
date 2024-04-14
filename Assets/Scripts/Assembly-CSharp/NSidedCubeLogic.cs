using System.Collections;
using UnityEngine;

public class NSidedCubeLogic : TriggerItem
{
	public Transform mainContainer;

	private Rotatable rotatable;

	private Collider[] colliders;

	private bool _autoIdaStop;

	private bool _stopped;

	private bool _disableAllRotation = true;

	public IdaCloneFollower[] followers;

	public CameraViewportController viewportController;

	private bool _swapped;

	private void Start()
	{
		colliders = mainContainer.GetComponentsInChildren<Collider>();
		rotatable = mainContainer.GetComponentInChildren<Rotatable>();
	}

	[TriggerableAction]
	public IEnumerator ActivateAutoIdaStop()
	{
		_autoIdaStop = true;
		return null;
	}

	[TriggerableAction]
	public IEnumerator DeactivateAutoIdaStop()
	{
		_autoIdaStop = false;
		return null;
	}

	private void Update()
	{
		if (rotatable.isStationary)
		{
			_stopped = false;
		}
		else if (_autoIdaStop && !_stopped)
		{
			_stopped = true;
			GameScene.player.GetComponent<CharacterLocomotion>().Stop();
		}
		if (_autoIdaStop && !_stopped)
		{
			Transform roomContainingIda = viewportController.roomContainingIda;
			if (roomContainingIda != null && !(Vector3.Dot(roomContainingIda.GetComponentInChildren<NSidedForwardDirection>().transform.forward, Camera.main.transform.forward) < 0f))
			{
				_stopped = true;
				GameScene.player.GetComponent<CharacterLocomotion>().Stop();
			}
		}
	}

	private void LateUpdate()
	{
		if (rotatable.isStationary)
		{
			if (!followers[0].gameObject.activeInHierarchy || _swapped)
			{
				return;
			}
			IdaCloneFollower idaCloneFollower = followers[0];
			for (int i = 0; i < followers.Length; i++)
			{
				if (Vector3.Distance(followers[i].transform.parent.parent.position, Vector3.zero) < 1f)
				{
					idaCloneFollower = followers[i];
					break;
				}
			}
			if (idaCloneFollower != null)
			{
				idaCloneFollower.SwapCloneWithIda();
				_swapped = true;
			}
		}
		else
		{
			_swapped = false;
		}
	}

	[TriggerableAction]
	public IEnumerator DisableAllRotators()
	{
		_disableAllRotation = true;
		Collider[] array = colliders;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].enabled = false;
		}
		return null;
	}

	[TriggerableAction]
	public IEnumerator EnableAllRotators()
	{
		_disableAllRotation = false;
		Collider[] array = colliders;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].enabled = true;
		}
		return null;
	}
}
