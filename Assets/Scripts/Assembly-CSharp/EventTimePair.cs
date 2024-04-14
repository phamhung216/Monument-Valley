using System;

[Serializable]
public class EventTimePair
{
	public static int frameRate = 24;

	public string eventName;

	public int frame;

	public float time => (float)frame / (float)frameRate;
}
