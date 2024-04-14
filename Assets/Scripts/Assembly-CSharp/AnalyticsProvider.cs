public abstract class AnalyticsProvider
{
	protected bool isInitialised;

	public abstract void StartSession(string appId);

	public abstract void LogEvent(string name);

	public abstract void LogEventWithVars(string name, string[] vars);

	public abstract void LogPivotEventWithVars(string name, string paramName, string paramValue, string[] vars);

	public abstract void LogEventWithAttributesAndMetrics(string name, string[] attributeNames, string[] attributeValues, string[] metricNames, string[] metricValues);
}
