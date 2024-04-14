using System;

[Serializable]
public class MoverAudioPreset
{
	public MoverAudioPresets.MoverAudioPresetType presetType;

	public MoverAudio.MotionType motionType;

	public string moveEvent;

	public string secondaryMoveEvent;

	public string chord;

	public string secondaryChord;

	public string stopEvent;
}
