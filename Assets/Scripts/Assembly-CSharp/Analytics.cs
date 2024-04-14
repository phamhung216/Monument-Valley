using System.Collections.Generic;
using UnityEngine;

public class Analytics : MonoBehaviour
{
	private static List<AnalyticsProvider> analyticsProviders = new List<AnalyticsProvider>();

	private void Awake()
	{
		if (0 == 0)
		{
			UnityAnalyticsProvider unityAnalyticsProvider = new UnityAnalyticsProvider();
			unityAnalyticsProvider.StartSession("TODO");
			analyticsProviders.Add(unityAnalyticsProvider);
		}
	}

	public static void LogEvent(string _event)
	{
		foreach (AnalyticsProvider analyticsProvider in analyticsProviders)
		{
			analyticsProvider.LogEvent(_event);
		}
	}

	public static void LogEventWithVars(string _event, string[] _vars)
	{
		foreach (AnalyticsProvider analyticsProvider in analyticsProviders)
		{
			analyticsProvider.LogEventWithVars(_event, _vars);
		}
	}

	public static void LogPivotEventWithVars(string name, string paramName, string paramValue, string[] vars)
	{
		foreach (AnalyticsProvider analyticsProvider in analyticsProviders)
		{
			analyticsProvider.LogPivotEventWithVars(name, paramName, paramValue, vars);
		}
	}

	public static void LogEventWithAttributesAndMetrics(string name, string[] attributeNames, string[] attributeValues, string[] metricNames, string[] metricValues)
	{
	}
}
