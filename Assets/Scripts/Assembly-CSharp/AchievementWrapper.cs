using System;
using Steamworks;
using UnityCommon;

public class AchievementWrapper : Singleton<AchievementWrapper>
{
	private bool _canUseAchievementsOnSteam;

	public void Init()
	{
		_canUseAchievementsOnSteam = SteamUserStats.RequestCurrentStats();
	}

	public void UnlockAchievement(string achievementID, double progress, Action<bool> callback)
	{
		bool obj = false;
		if (_canUseAchievementsOnSteam)
		{
			obj = SteamUserStats.SetAchievement(achievementID) && SteamUserStats.StoreStats();
		}
		callback(obj);
	}
}
