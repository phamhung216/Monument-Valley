using System.Collections;
using Fabric;
using UnityEngine;

public class AmbienceProgressChanger : MonoBehaviour
{
	public static float ambienceProgressValue;

	private bool _fading;

	private float _startTime;

	private float _from;

	private string _ambientEvent;

	private GameObject _sceneAudioObject;

	public float targetValue = 1f;

	public float duration = 4f;

	private void Awake()
	{
		ambienceProgressValue = 0f;
	}

	private void Start()
	{
		_sceneAudioObject = GameScene.instance.gameObject;
		_ambientEvent = _sceneAudioObject.GetComponent<SceneAudio>().ambientEvent;
	}

	[TriggerableAction]
	public IEnumerator StartAmbienceFade()
	{
		StartFade();
		return null;
	}

	[TriggerableAction(true)]
	public IEnumerator RunAmbienceFade()
	{
		StartFade();
		while (_fading)
		{
			yield return null;
		}
	}

	private void StartFade()
	{
		if (TriggerAction.FastForward)
		{
			_fading = false;
			ambienceProgressValue = targetValue;
			if ((bool)EventManager.Instance)
			{
				EventManager.Instance.SetParameter(_ambientEvent, "level_progress", ambienceProgressValue, _sceneAudioObject);
			}
		}
		else
		{
			_from = ambienceProgressValue;
			_startTime = Time.time;
			_fading = true;
		}
	}

	private void Update()
	{
		if (_fading)
		{
			float num = ((duration <= 0.01f) ? 1f : ((Time.time - _startTime) / duration));
			ambienceProgressValue = Mathf.Clamp(Mathf.Lerp(_from, targetValue, num), 0f, 1f);
			if (num >= 1f)
			{
				_fading = false;
			}
			if ((bool)EventManager.Instance)
			{
				EventManager.Instance.SetParameter(_ambientEvent, "level_progress", ambienceProgressValue, _sceneAudioObject);
			}
		}
	}
}
