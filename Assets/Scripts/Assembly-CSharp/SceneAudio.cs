using System.Collections;
using System.Collections.Generic;
using Fabric;
using UnityEngine;

public class SceneAudio : MonoBehaviour
{
	public string ambientEvent;

	public string crowFootstepEvent;

	private List<AIController> _crows;

	private bool _ambiencePlaying;

	private string _storytellerEvent = "StoryTeller";

	private string _idolEvent = "Ida/Idol/Loop";

	private string _endMotifEvent = "Ending/Motif";

	private bool _isEndingPlaying;

	private float _startTime;

	private void Awake()
	{
		_ = (bool)EventManager.Instance;
	}

	private void Start()
	{
		if (ambientEvent == null || ambientEvent.Length == 0)
		{
			ambientEvent = "World/Amb/LevelSelect";
		}
		if (crowFootstepEvent == null || crowFootstepEvent.Length == 0)
		{
			crowFootstepEvent = "Crow/Footstep/Default";
		}
		_crows = new List<AIController>();
		AIController[] array = Object.FindObjectsOfType<AIController>();
		for (int i = 0; i < array.Length; i++)
		{
			_crows.Add(array[i]);
		}
		LevelUnlockLogic levelUnlockLogic = Object.FindObjectOfType<LevelUnlockLogic>();
		_isEndingPlaying = (bool)levelUnlockLogic && levelUnlockLogic.forceShowEnding;
		_startTime = Time.time;
		_ambiencePlaying = false;
	}

	private void Update()
	{
		if (!_isEndingPlaying && !_ambiencePlaying && Time.time - _startTime > 0.5f)
		{
			PlayAmbience();
		}
		UpdateCrowAudio();
	}

	public void PlayAmbience()
	{
		if ((bool)EventManager.Instance)
		{
			EventManager.Instance.PostEvent(ambientEvent, base.gameObject);
			_ambiencePlaying = true;
			if (Object.FindObjectOfType<AmbienceProgressChanger>() != null)
			{
				EventManager.Instance.SetParameter(ambientEvent, "level_progress", AmbienceProgressChanger.ambienceProgressValue, base.gameObject);
			}
		}
	}

	public void StopAmbience()
	{
		if ((bool)EventManager.Instance)
		{
			if (EventManager.Instance.IsEventActive(ambientEvent, null))
			{
				EventManager.Instance.PostEvent(ambientEvent, EventAction.StopAll);
			}
			if (EventManager.Instance.IsEventActive(_storytellerEvent, null))
			{
				EventManager.Instance.PostEvent(_storytellerEvent, EventAction.StopAll);
			}
			if (EventManager.Instance.IsEventActive(_idolEvent, null))
			{
				EventManager.Instance.PostEvent(_idolEvent, EventAction.StopAll);
			}
			if (EventManager.Instance.IsEventActive(_endMotifEvent, null))
			{
				EventManager.Instance.PostEvent(_endMotifEvent, EventAction.StopAll);
			}
			ThiefAudioController thiefAudioController = Object.FindObjectOfType<ThiefAudioController>();
			if (thiefAudioController != null)
			{
				thiefAudioController.KillAudio();
			}
			if (IsInvoking("PlayAmbience"))
			{
				CancelInvoke("PlayAmbience");
			}
			if (IsInvoking("PlayStoryteller"))
			{
				CancelInvoke("PlayStoryteller");
			}
			StopAllWater();
			string eventName = "World/Movers/WaterWheel/Loop";
			if (EventManager.Instance.IsEventActive(eventName, null))
			{
				EventManager.Instance.PostEvent(eventName, EventAction.StopAll);
			}
			RebuildAudioController rebuildAudioController = Object.FindObjectOfType<RebuildAudioController>();
			if (rebuildAudioController != null)
			{
				rebuildAudioController.KillAudio();
			}
		}
	}

	private void UpdateCrowAudio()
	{
		if (_crows == null || _crows.Count <= 0)
		{
			return;
		}
		AIController aIController = null;
		float num = float.MaxValue;
		for (int i = 0; i < _crows.Count; i++)
		{
			AIController aIController2 = _crows[i];
			if (aIController2.gameObject.activeInHierarchy)
			{
				float num2 = Vector3.Distance(GameScene.player.transform.position, aIController2.transform.position);
				if (num2 < num || !aIController)
				{
					num = num2;
					aIController = aIController2;
				}
				aIController2.audioEventController.SuppressAudioEvent(crowFootstepEvent);
			}
		}
		if ((bool)aIController && aIController.isOnScreen)
		{
			aIController.audioEventController.UnSuppressAudioEvent(crowFootstepEvent);
		}
	}

	[TriggerableAction]
	public IEnumerator AttachAudioListenerToPlayer()
	{
		AudioListener componentInChildren = GetComponentInChildren<AudioListener>();
		componentInChildren.transform.parent = GameScene.player.transform;
		componentInChildren.transform.localPosition = Vector3.zero;
		return null;
	}

	[TriggerableAction]
	public IEnumerator StopAllWater()
	{
		if ((bool)EventManager.Instance && EventManager.Instance.IsEventActive(WaterSection.waterLoopEvent, null))
		{
			EventManager.Instance.PostEvent(WaterSection.waterLoopEvent, EventAction.StopAll);
		}
		return null;
	}

	public void BeginStorytellerAmbience()
	{
		StopAmbience();
		Invoke("PlayStoryteller", 0.5f);
	}

	public void EndStorytellerAmbience()
	{
		if ((bool)EventManager.Instance)
		{
			EventManager.Instance.PostEvent(_storytellerEvent, EventAction.StopSound, base.gameObject);
		}
		Invoke("PlayAmbience", 2f);
	}

	private void PlayStoryteller()
	{
		if ((bool)EventManager.Instance)
		{
			EventManager.Instance.PostEvent(_storytellerEvent, base.gameObject);
		}
	}
}
