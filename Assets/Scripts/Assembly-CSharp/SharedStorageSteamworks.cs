using System.IO;
using Steamworks;
using UnityEngine;

public class SharedStorageSteamworks : SharedStorageFile
{
	public SharedStorageSteamworks()
	{
		_filePath = Application.persistentDataPath + Path.DirectorySeparatorChar + "UserData";
		_filePath = _filePath + "_" + SteamUser.GetSteamID().ToString();
		_filePath = _filePath + Path.DirectorySeparatorChar + "user_data.sav";
	}

	public override void Synchronise()
	{
		base.Synchronise();
		AchievementManager.Instance.SyncAchievements();
	}
}
