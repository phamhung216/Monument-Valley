using System.Collections;
using UnityEngine;

public class LSEPortalManager : MonoBehaviour
{
	public TriggerableActionSequence worldSelectStartup;

	public TriggerableActionSequence revealFinaleSequence;

	public TriggerableActionSequence showFinaleImmediateSequence;

	public DistanceSwitch waitForArtifactSequenceSwitch;

	public bool[] expansionLevelCompleted;

	public LSEPortal[] lsePortalList;

	public int newArtifact = -1;

	private int newPhase = -1;

	private LevelManager _levelManager;

	public bool unlockAllLevels;

	public MaterialInstantiator[] phase3Lines;

	public AutoInterp phase3Interp;

	public GameObject constellation;

	private void Start()
	{
		if (LevelUnlockLogic.doNormalToWorldSelectSequence)
		{
			worldSelectStartup.TriggerActions();
		}
		_levelManager = LevelManager.Instance;
		_levelManager.SetHasShownLevelUnlock(LevelName.Volcano);
		Refresh();
		if (newArtifact >= 0 && newArtifact <= 7)
		{
			waitForArtifactSequenceSwitch.ForceOn();
		}
	}

	private void Refresh()
	{
		for (int i = 0; i < lsePortalList.Length; i++)
		{
			LevelName levelNameFromIndex = GetLevelNameFromIndex(i);
			_levelManager.HasShownLevelUnlock(levelNameFromIndex);
			bool flag = _levelManager.IsLevelCompleted(levelNameFromIndex);
			expansionLevelCompleted[i] = flag || unlockAllLevels;
		}
		LevelName lastPlayedLevel = LevelManager.LastPlayedLevel;
		if (_levelManager.IsLevelCompleted(lastPlayedLevel) && _levelManager.IsExpansionLevel(lastPlayedLevel) && !_levelManager.HasShownLevelUnlock(lastPlayedLevel))
		{
			int num = (int)(lastPlayedLevel - 14);
			newArtifact = num;
			if (newArtifact < 0 || newArtifact >= lsePortalList.Length)
			{
				newArtifact = -1;
			}
		}
		if (!_levelManager.IsLevelCompleted(LevelName.Volcano))
		{
			constellation.SetActive(value: false);
		}
		if (unlockAllLevels)
		{
			newArtifact = -1;
		}
		for (int j = 0; j <= 7; j++)
		{
			if (expansionLevelCompleted[j])
			{
				lsePortalList[j].SetCompleted();
				if (newArtifact != j)
				{
					lsePortalList[j].ShowArtifactImmediate();
				}
				else
				{
					lsePortalList[j].HideArtifactImmediate();
				}
			}
			else
			{
				lsePortalList[j].SetUncompleted();
				lsePortalList[j].HideArtifactImmediate();
			}
		}
		bool flag2 = true;
		bool flag3 = true;
		lsePortalList[0].ShowButton();
		if (!expansionLevelCompleted[0])
		{
			flag2 = false;
			flag3 = false;
		}
		for (int k = 1; k <= 6; k++)
		{
			if (flag2)
			{
				lsePortalList[k].ShowButton();
			}
			else
			{
				lsePortalList[k].HideButton();
			}
		}
		for (int l = 0; l <= 6; l++)
		{
			if (!expansionLevelCompleted[l])
			{
				flag3 = false;
			}
		}
		if (unlockAllLevels)
		{
			flag3 = true;
			lsePortalList[7].ShowButton();
		}
		if (flag3)
		{
			if (expansionLevelCompleted[7])
			{
				lsePortalList[7].SetCompleted();
			}
			else
			{
				lsePortalList[7].SetUncompleted();
			}
			if (newArtifact >= 1 && newArtifact <= 6)
			{
				newPhase = 3;
			}
			else
			{
				showFinaleImmediateSequence.TriggerActions();
			}
		}
		else
		{
			lsePortalList[7].HideButton();
		}
	}

	[TriggerableAction]
	public IEnumerator DoNewArtifactSequence()
	{
		if (newArtifact >= 0 && newArtifact <= 7)
		{
			lsePortalList[newArtifact].ArtifactAppear();
		}
		return null;
	}

	[TriggerableAction]
	public IEnumerator DoRevealButtonSequence()
	{
		if (newArtifact >= 0 && newArtifact < lsePortalList.Length)
		{
			string sceneName = lsePortalList[newArtifact].gameObject.GetComponent<TriggerLoadScene>().sceneName;
			LevelName levelIDFromName = _levelManager.GetLevelIDFromName(sceneName);
			_levelManager.SetHasShownLevelUnlock(levelIDFromName);
		}
		if (newPhase == 3)
		{
			revealFinaleSequence.TriggerActions();
		}
		return null;
	}

	[TriggerableAction]
	public IEnumerator DeactivateOtherPortals(LSEPortal newActivePortal)
	{
		LSEPortal[] array = lsePortalList;
		foreach (LSEPortal lSEPortal in array)
		{
			if (lSEPortal != newActivePortal)
			{
				lSEPortal.Deactivate();
			}
		}
		return null;
	}

	private LevelName GetLevelNameFromIndex(int idx)
	{
		string sceneName = lsePortalList[idx].gameObject.GetComponent<TriggerLoadScene>().sceneName;
		return _levelManager.GetLevelIDFromName(sceneName);
	}

	private int GetNextLevelIndex()
	{
		int num = -1;
		for (int i = 0; i < lsePortalList.Length; i++)
		{
			string sceneName = lsePortalList[i].gameObject.GetComponent<TriggerLoadScene>().sceneName;
			LevelName levelIDFromName = _levelManager.GetLevelIDFromName(sceneName);
			_levelManager.HasShownLevelUnlock(levelIDFromName);
			if (_levelManager.IsLevelCompleted(levelIDFromName))
			{
				num = i + 1;
				break;
			}
		}
		if (num >= lsePortalList.Length)
		{
			num = -1;
		}
		return num;
	}

	[TriggerableAction]
	public IEnumerator ActivateDebugMode()
	{
		unlockAllLevels = true;
		Refresh();
		return null;
	}

	[TriggerableAction]
	public IEnumerator DeActivateDebugMode()
	{
		unlockAllLevels = false;
		Refresh();
		return null;
	}

	private void LateUpdate()
	{
		MaterialInstantiator[] array = phase3Lines;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].instantiatedMaterial.SetColor("_Color", new Color(1f, 1f, 1f, phase3Interp.interpAmount));
		}
	}

	[TriggerableAction]
	public IEnumerator ResetSave()
	{
		UserDataController.Instance.Reset();
		LevelManager.Instance.LoadLevel(LevelName.Prelude);
		return null;
	}
}
