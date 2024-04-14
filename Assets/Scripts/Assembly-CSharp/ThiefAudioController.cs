using System.Collections;
using Fabric;
using UnityEngine;

public class ThiefAudioController : MonoBehaviour
{
	public static string melodyEvent = "World/Amb/ThiefMelody";

	public static string steamStartEvent = "World/Thief/Steam/Start";

	public static string steamEndEvent = "World/Thief/Steam/End";

	public static string steamLoopEvent = "World/Thief/Steam/Loop";

	public static string clicksEvent = "World/Amb/ThiefClicks";

	public static string accompanimentEvent = "World/Amb/ThiefAccompaniment";

	public static string steamFirstSting = "Sting/Thief/Steam";

	private Fabric.Component _clicksComp;

	private Fabric.Component _melodyComp;

	private Fabric.Component _accompComp;

	private int _currentSamplesLeft;

	private int _lastSamplesLeft;

	private bool _melodyTriggered;

	private bool _forcePlayOnNextFrame;

	private GameObject _sceneAudioObject;

	private float _triggerValue = 0.5f;

	private bool _steamStarted;

	private bool _clicksStarted;

	private bool _melodyStarted;

	private void Start()
	{
		_currentSamplesLeft = -1;
		_lastSamplesLeft = -1;
		_sceneAudioObject = GameScene.instance.gameObject;
	}

	[TriggerableAction]
	public IEnumerator StartSteam()
	{
		if ((bool)EventManager.Instance)
		{
			if (!_steamStarted)
			{
				_steamStarted = true;
			}
			EventManager.Instance.PostEvent(steamStartEvent, _sceneAudioObject);
			EventManager.Instance.PostEvent(steamLoopEvent, _sceneAudioObject);
		}
		return null;
	}

	[TriggerableAction]
	public IEnumerator EndSteam()
	{
		if ((bool)EventManager.Instance)
		{
			EventManager.Instance.PostEvent(steamEndEvent, _sceneAudioObject);
			EventManager.Instance.PostEvent(steamLoopEvent, EventAction.StopAll);
		}
		return null;
	}

	[TriggerableAction]
	public IEnumerator StartClicks()
	{
		_clicksStarted = true;
		if ((bool)EventManager.Instance)
		{
			EventManager.Instance.PostEvent(clicksEvent, _sceneAudioObject);
			UpdateEvents();
		}
		return null;
	}

	[TriggerableAction]
	public IEnumerator StopClicks()
	{
		_clicksStarted = false;
		if ((bool)EventManager.Instance)
		{
			UpdateEvents();
			EventManager.Instance.PostEvent(clicksEvent, EventAction.StopAll);
		}
		return null;
	}

	[TriggerableAction]
	public IEnumerator StartMelody()
	{
		FindComponents();
		_melodyComp.FadeInTime = 0f;
		_accompComp.FadeInTime = 0f;
		PlayMelody();
		return null;
	}

	private void PlayMelody()
	{
		_melodyStarted = true;
		if ((bool)EventManager.Instance)
		{
			EventManager.Instance.PostEvent(melodyEvent, _sceneAudioObject);
			EventManager.Instance.PostEvent(accompanimentEvent, _sceneAudioObject);
			UpdateEvents();
		}
	}

	[TriggerableAction]
	public IEnumerator StopMelody()
	{
		_melodyStarted = false;
		if ((bool)EventManager.Instance)
		{
			UpdateEvents();
			EventManager.Instance.PostEvent(melodyEvent, EventAction.StopAll);
			EventManager.Instance.PostEvent(accompanimentEvent, EventAction.StopAll);
		}
		return null;
	}

	[TriggerableAction]
	public IEnumerator StartOnCheckpoint()
	{
		Invoke("StartClicks", 1.5f);
		Invoke("PlayMelody", 1.5f);
		FindComponents();
		_clicksComp.FadeInTime = 4f;
		_melodyComp.FadeInTime = 4f;
		_accompComp.FadeInTime = 4f;
		return null;
	}

	public void KillAudio()
	{
		if ((bool)EventManager.Instance)
		{
			EventManager.Instance.PostEvent(clicksEvent, EventAction.StopAll);
			EventManager.Instance.PostEvent(melodyEvent, EventAction.StopAll);
			EventManager.Instance.PostEvent(accompanimentEvent, EventAction.StopAll);
			EventManager.Instance.PostEvent(steamLoopEvent, EventAction.StopAll);
			_clicksStarted = false;
			_melodyStarted = false;
		}
	}

	private void LateUpdate()
	{
		UpdateEvents();
	}

	private void UpdateEvents()
	{
		if (_clicksStarted && (bool)EventManager.Instance)
		{
			EventManager.Instance.SetParameter(clicksEvent, "level_progress", AmbienceProgressChanger.ambienceProgressValue, _sceneAudioObject);
		}
		if (_melodyStarted && (bool)EventManager.Instance)
		{
			EventManager.Instance.SetParameter(melodyEvent, "level_progress", AmbienceProgressChanger.ambienceProgressValue, _sceneAudioObject);
			EventManager.Instance.SetParameter(accompanimentEvent, "level_progress", AmbienceProgressChanger.ambienceProgressValue, _sceneAudioObject);
		}
	}

	private void FindComponents()
	{
		if ((bool)FabricManager.Instance)
		{
			if (!_clicksComp)
			{
				_clicksComp = FabricManager.Instance.GetComponentByName("Audio_Music_ThiefAmbience_ThiefMusic_ThiefMixClicks");
			}
			if (!_melodyComp)
			{
				_melodyComp = FabricManager.Instance.GetComponentByName("Audio_Music_ThiefAmbience_ThiefMusic_ThiefMixMelody");
			}
			if (!_accompComp)
			{
				_accompComp = FabricManager.Instance.GetComponentByName("Audio_Music_ThiefAmbience_ThiefMusic_ThiefMixAccompaniment");
			}
		}
	}
}
