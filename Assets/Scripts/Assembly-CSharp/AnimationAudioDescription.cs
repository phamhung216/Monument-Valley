using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AnimationAudioDescription
{
	public AnimationClip clip;

	public List<EventTimePair> eventPairs;

	[HideInInspector]
	public int eventIdx;

	[HideInInspector]
	public float lastNormTime;

	[HideInInspector]
	public string name;

	public EventTimePair currentPair
	{
		get
		{
			if (eventIdx >= 0 && eventIdx < eventPairs.Count)
			{
				return eventPairs[eventIdx];
			}
			return null;
		}
	}
}
