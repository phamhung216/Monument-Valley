using System.Collections;
using UnityEngine;

public class AnalyticsEvent : MonoBehaviour
{
	[TriggerableAction]
	public IEnumerator SendEvent()
	{
		Analytics.LogEvent(base.name);
		return null;
	}
}
