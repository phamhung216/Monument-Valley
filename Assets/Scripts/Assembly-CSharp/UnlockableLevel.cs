using System.Collections;
using UnityEngine;

public class UnlockableLevel : MonoBehaviour
{
	private enum LevelUnlockState
	{
		Locked = 0,
		Unlocked = 1,
		Invisible = 2
	}

	public LevelName levelName;

	public LevelSelectLogic levelSelectLogic;

	public AnimationClip clipOpen;

	public AnimationClip clipClosed;

	public AnimationClip clipUnlock;

	public TriggerableActionSequence unlockSequence;

	public TriggerableActionSequence firstCompleteSequence;

	public TriggerableActionSequence completeSequence;

	public Animation unlockAnimation;

	public TouchTrigger button;

	public AutoInterp iconInterp;

	public float unlockAnimationDuration = 2f;

	public bool animationTriggeredBySequence;

	private LevelUnlockState _state = LevelUnlockState.Invisible;

	private LevelUnlockState state => _state;

	public void RefreshState()
	{
		if (levelName == LevelName.None)
		{
			return;
		}
		LevelManager instance = LevelManager.Instance;
		if (instance.IsLevelUnlocked(levelName) || LevelUnlockLogic.debugMode)
		{
			_state = LevelUnlockState.Unlocked;
		}
		else
		{
			_state = LevelUnlockState.Invisible;
			LevelName[] levelPrecursors = instance.GetLevelPrecursors(levelName);
			if (levelPrecursors != null)
			{
				LevelName[] array = levelPrecursors;
				foreach (LevelName level in array)
				{
					if (instance.IsLevelUnlocked(level))
					{
						_state = LevelUnlockState.Locked;
						break;
					}
				}
			}
		}
		if (_state == LevelUnlockState.Unlocked)
		{
			if (!animationTriggeredBySequence)
			{
				button.isEnabled = true;
				unlockAnimation.clip = clipOpen;
				unlockAnimation.Play();
			}
		}
		else if (!animationTriggeredBySequence)
		{
			button.isEnabled = false;
			unlockAnimation.clip = clipClosed;
			unlockAnimation.Play();
		}
		if (instance.IsLevelCompleted(levelName))
		{
			iconInterp.SetToOne();
		}
		else
		{
			iconInterp.SetToZero();
		}
	}

	[TriggerableAction]
	public IEnumerator ShowClosed()
	{
		unlockAnimation.clip = clipClosed;
		unlockAnimation.Play();
		return null;
	}

	[TriggerableAction]
	public IEnumerator SelectLevel()
	{
		levelSelectLogic.SelectLevel(levelName);
		return null;
	}

	public void ToggleMeshRendererGroup(MeshRenderer[] mrs, bool show)
	{
		for (int i = 0; i < mrs.Length; i++)
		{
			mrs[i].enabled = show;
		}
	}
}
