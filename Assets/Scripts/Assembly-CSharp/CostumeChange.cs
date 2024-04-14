using System.Collections;
using Fabric;
using UnityEngine;

public class CostumeChange : MonoBehaviour
{
	private CharacterLocomotion _ida;

	private HatPickup _hat;

	public string soundEvent;

	public GameObject dropShadow;

	public CameraLookAtPoint zoomOutCameraLookAt;

	public CustomAnimTarget changeHatAnimation;

	private void Start()
	{
		if (_hat == null)
		{
			_hat = base.gameObject.GetComponentInChildren<HatPickup>();
		}
	}

	private void EnsureCharacterRef()
	{
		if (!_ida && (bool)GameScene.player)
		{
			_ida = GameScene.player.GetComponent<CharacterLocomotion>();
		}
	}

	[TriggerableAction]
	public IEnumerator HoldHat()
	{
		EnsureCharacterRef();
		_ida.RemoveHat();
		HatPickup[] componentsInChildren = _ida.GetComponentsInChildren<HatPickup>();
		if (componentsInChildren != null && componentsInChildren.Length != 0)
		{
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				Object.Destroy(componentsInChildren[i].gameObject);
			}
		}
		_hat.HoldHat();
		return null;
	}

	[TriggerableAction]
	public IEnumerator HatOnHead()
	{
		EnsureCharacterRef();
		_hat.HatOnHead();
		dropShadow.SetActive(value: false);
		return null;
	}

	[TriggerableAction]
	public IEnumerator ShrinkHat()
	{
		_hat.ShrinkHat();
		return null;
	}

	[TriggerableAction]
	public IEnumerator ChangeCostume()
	{
		if (_hat != null)
		{
			EnsureCharacterRef();
			SkinnedMeshRenderer componentInChildren = _ida.GetComponentInChildren<SkinnedMeshRenderer>();
			if (componentInChildren != null && _hat.idaMaterials.Length != 0)
			{
				componentInChildren.materials = _hat.idaMaterials;
			}
		}
		return null;
	}

	[TriggerableAction(true)]
	public IEnumerator HatZoomOut()
	{
		yield return StartCoroutine(zoomOutCameraLookAt.LookAtWithZoomToDefault());
	}

	[TriggerableAction]
	public IEnumerator ChangeHatAnimation()
	{
		if (!TriggerAction.FastForward)
		{
			changeHatAnimation.StartAnimation();
			if ((bool)EventManager.Instance && !string.IsNullOrEmpty(soundEvent))
			{
				EventManager.Instance.PostEvent(soundEvent, EventAction.PlaySound);
			}
		}
		yield return null;
	}
}
