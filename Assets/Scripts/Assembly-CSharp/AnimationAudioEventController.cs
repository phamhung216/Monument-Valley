using System.Collections;
using System.Collections.Generic;
using Fabric;
using UnityEngine;

public class AnimationAudioEventController : MonoBehaviour
{
	public Animation animatedObject;

	public List<AnimationAudioDescription> animations;

	public GameObject audioInstanceOwner;

	private List<string> _suppressedEvents;

	public bool muteAudio;

	private void Start()
	{
		_suppressedEvents = new List<string>();
		if (audioInstanceOwner == null)
		{
			audioInstanceOwner = base.gameObject;
		}
		for (int i = 0; i < animations.Count; i++)
		{
			animations[i].name = animations[i].clip.name;
		}
	}

	private void Update()
	{
		if (!(animatedObject != null) || !animatedObject.gameObject.activeInHierarchy || animations.Count <= 0)
		{
			return;
		}
		for (int i = 0; i < animations.Count; i++)
		{
			AnimationAudioDescription animationAudioDescription = animations[i];
			if (animatedObject.IsPlaying(animationAudioDescription.name))
			{
				AnimationState animationState = animatedObject[animationAudioDescription.name];
				float num = ((animationState.normalizedTime > 1f) ? (animationState.normalizedTime - Mathf.Floor(animationState.normalizedTime)) : animationState.normalizedTime);
				if (animationAudioDescription.currentPair != null && num * animationState.length >= animationAudioDescription.currentPair.time)
				{
					PlayAudioEvent(animationAudioDescription.currentPair.eventName);
					if (animationAudioDescription.eventIdx < animationAudioDescription.eventPairs.Count)
					{
						animationAudioDescription.eventIdx++;
					}
				}
				if (animationAudioDescription.lastNormTime > num)
				{
					animationAudioDescription.eventIdx = 0;
				}
				animationAudioDescription.lastNormTime = num;
			}
			else
			{
				animationAudioDescription.eventIdx = 0;
			}
		}
	}

	public void PlayAudioEvent(string name)
	{
		if (!muteAudio && !TriggerAction.FastForward && !_suppressedEvents.Contains(name) && (bool)EventManager.Instance)
		{
			EventManager.Instance.PostEvent(name, audioInstanceOwner);
		}
	}

	public void SuppressAudioEvent(string name)
	{
		if (!_suppressedEvents.Contains(name))
		{
			_suppressedEvents.Add(name);
		}
	}

	public void UnSuppressAudioEvent(string name)
	{
		if (_suppressedEvents.Contains(name))
		{
			_suppressedEvents.Remove(name);
		}
	}

	[TriggerableAction]
	public IEnumerator MuteAudio()
	{
		muteAudio = true;
		return null;
	}

	[TriggerableAction]
	public IEnumerator UnmuteAudio()
	{
		muteAudio = false;
		return null;
	}
}
