using System.Collections.Generic;
using UnityCommon;

public class AchievementManager
{
	public enum AchievementId
	{
		CompletedPrelude = 0,
		CompletedTheGarden = 1,
		CompletedHiddenTemple = 2,
		CompletedWaterPalace = 3,
		CompletedTheSpire = 4,
		CompletedTheLabyrinth = 5,
		CompletedTheRookery = 6,
		CompletedTheBox = 7,
		CompletedTheDescent = 8,
		CompletedObservatory = 9,
		CompletedTheChasm = 10,
		CompletedTheSerpentLake = 11,
		CompletedTheThief = 12,
		CompletedHalcyonCourt = 13,
		CompletedTheLostFalls = 14,
		CompletedTheCitadelofDeceit = 15,
		CompletedTheOubliette = 16,
		CompletedNocturne = 17,
		CompletedIdasDream = 18
	}

	private static AchievementManager _instance;

	private string[] _achievementIds = new string[19]
	{
		"CgkIgN7c3e8XEAIQAQ", "CgkIgN7c3e8XEAIQAg", "CgkIgN7c3e8XEAIQAw", "CgkIgN7c3e8XEAIQBA", "CgkIgN7c3e8XEAIQBQ", "CgkIgN7c3e8XEAIQBg", "CgkIgN7c3e8XEAIQBw", "CgkIgN7c3e8XEAIQCA", "CgkIgN7c3e8XEAIQCQ", "CgkIgN7c3e8XEAIQCg",
		"CgkIgN7c3e8XEAIQCw", "CgkIgN7c3e8XEAIQDA", "CgkIgN7c3e8XEAIQDQ", "CgkIgN7c3e8XEAIQDg", "CgkIgN7c3e8XEAIQDw", "CgkIgN7c3e8XEAIQEA", "CgkIgN7c3e8XEAIQEQ", "CgkIgN7c3e8XEAIQEg", "CgkIgN7c3e8XEAIQEw"
	};

	private Dictionary<LevelName, AchievementId> _levelAchievements;

	public static AchievementManager Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new AchievementManager();
			}
			return _instance;
		}
	}

	public AchievementManager()
	{
		_levelAchievements = new Dictionary<LevelName, AchievementId>
		{
			{
				LevelName.Prelude,
				AchievementId.CompletedPrelude
			},
			{
				LevelName.Intro,
				AchievementId.CompletedTheGarden
			},
			{
				LevelName.Draggers,
				AchievementId.CompletedHiddenTemple
			},
			{
				LevelName.GripRotate,
				AchievementId.CompletedWaterPalace
			},
			{
				LevelName.Spire,
				AchievementId.CompletedTheSpire
			},
			{
				LevelName.Labyrinth,
				AchievementId.CompletedTheLabyrinth
			},
			{
				LevelName.Keep,
				AchievementId.CompletedTheRookery
			},
			{
				LevelName.InsideBox,
				AchievementId.CompletedTheBox
			},
			{
				LevelName.Descent,
				AchievementId.CompletedTheDescent
			},
			{
				LevelName.Zen,
				AchievementId.CompletedObservatory
			},
			{
				LevelName.Volcano,
				AchievementId.CompletedTheChasm
			},
			{
				LevelName.Twister,
				AchievementId.CompletedTheSerpentLake
			},
			{
				LevelName.Thief,
				AchievementId.CompletedTheThief
			},
			{
				LevelName.Waterways,
				AchievementId.CompletedHalcyonCourt
			},
			{
				LevelName.Crush,
				AchievementId.CompletedTheLostFalls
			},
			{
				LevelName.Reintro,
				AchievementId.CompletedTheCitadelofDeceit
			},
			{
				LevelName.NSided,
				AchievementId.CompletedTheOubliette
			},
			{
				LevelName.Rebuild,
				AchievementId.CompletedNocturne
			},
			{
				LevelName.Windmill_blue,
				AchievementId.CompletedIdasDream
			}
		};
	}

	public void SyncAchievements()
	{
		foreach (LevelName key in _levelAchievements.Keys)
		{
			if (LevelManager.Instance.IsLevelCompleted(key))
			{
				UnlockAchievementForLevel(key);
			}
		}
	}

	public void UnlockAchievementForLevel(LevelName level)
	{
		if (!_levelAchievements.ContainsKey(level))
		{
			return;
		}
		AchievementId achievementId = _levelAchievements[level];
		string idRaw = _achievementIds[(int)achievementId];
		Singleton<AchievementWrapper>.Instance.UnlockAchievement(idRaw, 100.0, delegate(bool success)
		{
			if (success)
			{
				unlockAchievementSucceededEvent(idRaw, newlyUnlocked: true);
			}
			else
			{
				unlockAchievementFailedEvent(idRaw, "error");
			}
		});
	}

	private void unlockAchievementFailedEvent(string achievementId, string error)
	{
	}

	private void unlockAchievementSucceededEvent(string achievementId, bool newlyUnlocked)
	{
	}
}
