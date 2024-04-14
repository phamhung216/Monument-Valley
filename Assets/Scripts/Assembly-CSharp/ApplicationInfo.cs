using UnityEngine;

public class ApplicationInfo
{
	public const string folder = "Resources";

	public const string versionFile = "bundle-version";

	public const string buildTimeFile = "build-time";

	private static string _buildTime;

	private static string _bundleVersion;

	public const string ASSETS_FOLDER = "Assets";

	public static string bundleVersion
	{
		get
		{
			if (_bundleVersion == null)
			{
				TextAsset textAsset = Resources.Load("bundle-version", typeof(TextAsset)) as TextAsset;
				_bundleVersion = ((textAsset != null && textAsset.text.Length > 0) ? textAsset.text : "Not Found");
			}
			return _bundleVersion;
		}
	}

	public static string buildTime
	{
		get
		{
			if (_buildTime == null)
			{
				TextAsset textAsset = Resources.Load("build-time", typeof(TextAsset)) as TextAsset;
				_buildTime = ((textAsset != null && textAsset.text.Length > 0) ? textAsset.text : "Not Found");
			}
			return _buildTime;
		}
	}
}
