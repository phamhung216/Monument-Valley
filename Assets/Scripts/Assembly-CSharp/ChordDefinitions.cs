using System.Collections.Generic;
using Fabric;
using UnityEngine;

public class ChordDefinitions : MonoBehaviour
{
	public GameObject instrumentsGroup;

	public Chord[] chords;

	private static Dictionary<string, int[]> _chordTones;

	public static bool initialised;

	private void Awake()
	{
		_chordTones = new Dictionary<string, int[]>();
		Chord[] array = chords;
		foreach (Chord chord in array)
		{
			_chordTones.Add(chord.name, chord.semitones);
		}
	}

	private void Update()
	{
		if (initialised)
		{
			return;
		}
		initialised = true;
		Transform[] componentsInChildren = instrumentsGroup.GetComponentsInChildren<Transform>();
		foreach (Transform transform in componentsInChildren)
		{
			if (transform.parent == instrumentsGroup.transform)
			{
				InitialiseInstrument(transform);
			}
		}
	}

	private void InitialiseInstrument(Transform instrumentRoot)
	{
		string text = "World/Instruments/" + instrumentRoot.name + "/";
		for (int i = 0; i < instrumentRoot.childCount; i++)
		{
			string text2 = text + i;
			EventManager.Instance._eventList.Add(text2);
			Transform[] componentsInChildren = instrumentRoot.GetComponentsInChildren<Transform>();
			foreach (Transform transform in componentsInChildren)
			{
				if (transform.parent == instrumentRoot && int.Parse(transform.gameObject.name.Substring(0, Mathf.Min(transform.gameObject.name.Length, 2))) == i)
				{
					transform.GetComponent<EventListener>()._eventName = text2;
					EventManager.Instance.RegisterListener(transform.GetComponent<Fabric.Component>(), text2);
				}
			}
		}
	}

	public int GetNumTones(string chordName)
	{
		return _chordTones[chordName].Length;
	}

	public int GetSemiTone(string chordName, int semitoneIndex)
	{
		return _chordTones[chordName][semitoneIndex];
	}
}
