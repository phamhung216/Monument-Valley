using System.Collections.Generic;
using UnityEngine.Analytics;

public class UnityAnalyticsProvider : AnalyticsProvider
{
	public override void StartSession(string appId)
	{
	}

	public override void LogEvent(string name)
	{
		UnityEngine.Analytics.Analytics.CustomEvent(name, null);
	}

	public override void LogEventWithVars(string name, string[] vars)
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		for (int i = 0; i < vars.Length; i += 2)
		{
			dictionary.Add(vars[i + 1], vars[i]);
		}
		UnityEngine.Analytics.Analytics.CustomEvent(name, dictionary);
	}

	public override void LogPivotEventWithVars(string name, string paramName, string paramValue, string[] vars)
	{
		string[] vars2 = new string[2] { paramValue, paramName };
		LogEventWithVars(name, vars2);
		if (vars != null && vars.Length != 0)
		{
			LogEventWithVars(name + "_" + paramValue, vars);
		}
	}

	public override void LogEventWithAttributesAndMetrics(string name, string[] attributeNames, string[] attributeValues, string[] metricNames, string[] metricValues)
	{
	}
}
