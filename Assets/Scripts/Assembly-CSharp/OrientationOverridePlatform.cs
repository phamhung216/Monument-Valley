using UnityCommon;
using UnityEngine.Scripting;

[Preserve]
public class OrientationOverridePlatform : IOrientationOverrideProjectPlatformConfiguration
{
	public bool IsLandscape()
	{
		return true;
	}
}
