using System.Collections.Generic;
using UnityEngine;

public class MoverAudioPresets : MonoBehaviour
{
	public enum MoverAudioPresetType
	{
		PhysicalDragger = 0,
		PhysicalRotator = 1,
		InteractiveDragger = 2,
		InteractiveRotator = 3
	}

	public List<MoverAudioPreset> presets;

	private static MoverAudioPresets _instance;

	public static MoverAudioPresets instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = Object.FindObjectOfType(typeof(MoverAudioPresets)) as MoverAudioPresets;
			}
			return _instance;
		}
	}

	public static void UpdateMoverWithPreset(MoverAudio mover, MoverAudioPresetType type)
	{
		int index = 0;
		for (int i = 0; i < instance.presets.Count; i++)
		{
			if (instance.presets[i].presetType == type)
			{
				index = i;
				break;
			}
		}
		MoverAudioPreset moverAudioPreset = instance.presets[index];
		mover.motionType = moverAudioPreset.motionType;
		mover.audioEvent = moverAudioPreset.moveEvent;
		if (mover.motionType != MoverAudio.MotionType.RotateInteractive && mover.motionType != MoverAudio.MotionType.TranslateInteractive)
		{
			mover.audioEvent += (mover.isLarge ? "/Large" : "/Normal");
		}
		mover.secondaryEvent = moverAudioPreset.secondaryMoveEvent;
		mover.chordName = moverAudioPreset.chord;
		mover.secondaryChord = moverAudioPreset.secondaryChord;
		mover.stopEvent = moverAudioPreset.stopEvent;
	}
}
