using UnityEngine;

public class BuildInfo : ScriptableObject
{
	public string builtByUser;

	public string buildTime;

	public string[] levelNames;

	private const string BUILD_INFO_NAME = "BuildInfo";

	private static BuildInfo _currentBuildInfo;

	public static BuildInfo Current
	{
		get
		{
			if (_currentBuildInfo == null)
			{
				_currentBuildInfo = Resources.Load("BuildInfo") as BuildInfo;
			}
			return _currentBuildInfo;
		}
	}
}
