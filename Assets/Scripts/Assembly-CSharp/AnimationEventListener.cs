using System.Collections.Generic;
using UnityEngine;

public class AnimationEventListener : MonoBehaviour
{
	public Animation animatedObject;

	public List<AnimationEventPair> eventList = new List<AnimationEventPair>();

	private Animation _animation;

	private void Start()
	{
		_animation = GetComponent<Animation>();
	}

	private void Update()
	{
		if (!(null != animatedObject) || !animatedObject.gameObject.activeInHierarchy || eventList.Count <= 0)
		{
			return;
		}
		for (int i = 0; i < eventList.Count; i++)
		{
			AnimationEventPair animationEventPair = eventList[i];
			AnimationClip eventClip = animationEventPair.eventClip;
			bool flag = false;
			for (int j = 0; j < animationEventPair.animationClips.Count; j++)
			{
				AnimationClip animationClip = animationEventPair.animationClips[j];
				bool flag2 = animatedObject.IsPlaying(animationClip.name);
				flag = flag || flag2;
			}
			if (flag != _animation.IsPlaying(eventClip.name))
			{
				if (flag)
				{
					_animation.Play(eventClip.name);
				}
				else
				{
					_animation.Stop(eventClip.name);
				}
			}
		}
	}

	public void TestEvent(int param)
	{
	}

	public void TestEvent2(GameObject obj)
	{
	}
}
