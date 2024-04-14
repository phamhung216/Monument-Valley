using System.Collections;
using Fabric;
using UnityEngine;

public class RebuildAudioController : MonoBehaviour
{
	private string _altarEvent = "World/Rebuild/Altar/";

	private int _altarsActivated;

	private bool _fading;

	private float _startTime;

	private float _from;

	private float _fadeValue;

	public float fadeTargetValue = 1f;

	public float fadeDuration = 4f;

	private GameObject _sceneAudioObject;

	private void Start()
	{
		_sceneAudioObject = GameScene.instance.gameObject;
	}

	[TriggerableAction]
	public IEnumerator ActivateAltar()
	{
		if (_altarsActivated < 4)
		{
			_altarsActivated++;
			if ((bool)EventManager.Instance)
			{
				EventManager.Instance.PostEvent(_altarEvent + _altarsActivated, EventAction.PlaySound, _sceneAudioObject);
			}
		}
		return null;
	}

	[TriggerableAction]
	public IEnumerator DeactivateAltar()
	{
		if (_altarsActivated > 0)
		{
			EventManager.Instance.PostEvent(_altarEvent + _altarsActivated, EventAction.StopAll);
			_altarsActivated--;
		}
		return null;
	}

	public void KillAudio()
	{
		_fading = false;
		if ((bool)EventManager.Instance)
		{
			for (int i = 1; i <= 4; i++)
			{
				EventManager.Instance.PostEvent(_altarEvent + i, EventAction.StopAll);
			}
		}
	}

	[TriggerableAction]
	public IEnumerator StartAltarFade()
	{
		StartFade();
		return null;
	}

	[TriggerableAction(true)]
	public IEnumerator RunAltarFade()
	{
		StartFade();
		while (_fading)
		{
			yield return null;
		}
	}

	private void StartFade()
	{
		if (!TriggerAction.FastForward)
		{
			_from = _fadeValue;
			_startTime = Time.time;
			_fading = true;
		}
	}

	private void Update()
	{
		if (!_fading)
		{
			return;
		}
		float num = ((fadeDuration <= 0.01f) ? 1f : ((Time.time - _startTime) / fadeDuration));
		_fadeValue = Mathf.Clamp(Mathf.Lerp(_from, fadeTargetValue, num), 0f, 1f);
		if (num >= 1f)
		{
			_fading = false;
			KillAudio();
		}
		if ((bool)EventManager.Instance)
		{
			for (int i = 1; i <= 4; i++)
			{
				EventManager.Instance.SetParameter(_altarEvent + i, "Fade", _fadeValue, _sceneAudioObject);
			}
		}
	}
}
