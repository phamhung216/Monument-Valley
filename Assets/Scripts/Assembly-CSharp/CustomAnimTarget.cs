using System.Collections;
using UnityEngine;

public class CustomAnimTarget : MonoBehaviour
{
	public string animName;

	public bool ignoreNav;

	public bool doBlend;

	public BaseLocomotion character;

	public bool faceTarget = true;

	public bool detachHat = true;

	public Transform dropShadowPosition;

	public float startNormalizeTime;

	private void EnsureCharacterRef()
	{
		if (!character && (bool)GameScene.player)
		{
			character = GameScene.player.GetComponent<BaseLocomotion>();
		}
	}

	private void Start()
	{
		EnsureCharacterRef();
	}

	[TriggerableAction]
	public IEnumerator FaceHere()
	{
		EnsureCharacterRef();
		Vector3 normalized = (base.transform.position - character.currentBrush.transform.position).normalized;
		character.FaceDirection(normalized);
		return null;
	}

	[TriggerableAction]
	public IEnumerator StartAnimation()
	{
		EnsureCharacterRef();
		if (faceTarget)
		{
			character.FaceDirection((base.transform.position - character.transform.position).normalized);
		}
		character.RequestCustomAnim(animName, ignoreNav, doBlend, detachHat, dropShadowPosition, startNormalizeTime);
		return null;
	}

	[TriggerableAction(true)]
	public IEnumerator RunAnimation()
	{
		EnsureCharacterRef();
		StartAnimation();
		yield return null;
		while (character.animSystem.IsPlaying(animName))
		{
			yield return null;
		}
	}

	[TriggerableAction(true)]
	public IEnumerator RunAnimationAfterLoop()
	{
		EnsureCharacterRef();
		foreach (AnimationState item in character.animSystem)
		{
			if (item.clip.wrapMode == WrapMode.Loop && character.animSystem.IsPlaying(item.clip.name))
			{
				float num = item.time % item.length;
				num = item.length - num;
				num -= Time.deltaTime;
				yield return new WaitForSeconds(num);
				break;
			}
		}
		StartAnimation();
		yield return null;
		AnimationState animationState2 = character.animSystem[animName];
		float seconds = animationState2.length - animationState2.time - Time.deltaTime;
		yield return new WaitForSeconds(seconds);
	}
}
