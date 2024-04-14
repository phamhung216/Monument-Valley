using UnityEngine;

public class DevDebug
{
	public static void Log(string message, Object context)
	{
		_ = Debug.isDebugBuild;
	}

	public static void Log(string message)
	{
		_ = Debug.isDebugBuild;
	}
}
