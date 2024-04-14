using System.Collections;
using Fabric;
using UnityEngine;

public class AudioSettings : MonoBehaviour
{
	public static bool musicMuted;

	public static bool soundMuted;

	public UIToggleButton musicButton;

	public UIToggleButton soundButton;

	public UIToggleButton pcMusicButton;

	public UIToggleButton pcSoundButton;

	private const string musicMutedKey = "musicMutedKey";

	private const string soundMutedKey = "soundMutedKey";

	private const string _musicGroupEvent = "Global/Music";

	private const string _sfxGroupEvent = "Global/SFX";

	private bool _isOtherMusicPlaying;

	private void OnApplicationPause(bool pauseStatus)
	{
		_isOtherMusicPlaying = DeviceUtils.IsDeviceMusicPlaying();
		UpdateMusicMuting();
	}

	private void Start()
	{
		_isOtherMusicPlaying = DeviceUtils.IsDeviceMusicPlaying();
		LoadKeys();
		UpdateMusicMuting();
		if ((bool)EventManager.Instance)
		{
			EventManager.Instance.PostEvent("Global/SFX", EventAction.SetVolume, soundMuted ? 0f : 1f);
		}
	}

	private void OnMusicDropdownChanged(int value)
	{
		if (musicMuted != (value == 1))
		{
			ToggleMuteMusic();
		}
	}

	private void OnSoundDropdownChanged(int value)
	{
		if (soundMuted != (value == 1))
		{
			ToggleMuteSound();
		}
	}

	[TriggerableAction]
	public IEnumerator RefreshAudioButtons()
	{
		musicButton.isSelected = musicMuted;
		pcMusicButton.isSelected = !musicMuted;
		soundButton.isSelected = !soundMuted;
		pcSoundButton.isSelected = !soundMuted;
		return null;
	}

	[TriggerableAction]
	public IEnumerator ToggleMuteMusic()
	{
		musicMuted = !musicMuted;
		UpdateMusicMuting();
		SaveKeys();
		if (!musicMuted)
		{
			GameScene.instance.GetComponent<SceneAudio>().PlayAmbience();
		}
		return null;
	}

	[TriggerableAction]
	public IEnumerator ToggleMuteSound()
	{
		soundMuted = !soundMuted;
		if ((bool)EventManager.Instance)
		{
			EventManager.Instance.PostEvent("Global/SFX", EventAction.SetVolume, soundMuted ? 0f : 1f);
		}
		SaveKeys();
		return null;
	}

	private void LoadKeys()
	{
		musicMuted = UserDataController.GetUserLocalPrefsInt("musicMutedKey", 0) == 1;
		soundMuted = UserDataController.GetUserLocalPrefsInt("soundMutedKey", 0) == 1;
	}

	private void SaveKeys()
	{
		UserDataController.SetUserLocalPrefsInt("musicMutedKey", musicMuted ? 1 : 0);
		UserDataController.SetUserLocalPrefsInt("soundMutedKey", soundMuted ? 1 : 0);
	}

	private void UpdateMusicMuting()
	{
		bool flag = musicMuted || _isOtherMusicPlaying;
		if ((bool)EventManager.Instance)
		{
			EventManager.Instance.PostEvent("Global/Music", EventAction.SetVolume, flag ? 0f : 1f);
		}
	}
}
