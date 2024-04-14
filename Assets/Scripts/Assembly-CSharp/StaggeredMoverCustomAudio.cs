using Fabric;
using UnityEngine;

public class StaggeredMoverCustomAudio : MonoBehaviour
{
	public string primaryInstrument;

	public int[] notes;

	private int _index;

	public void Reset()
	{
		_index = 0;
	}

	public void PlayNextNote()
	{
		if (_index >= 0 && (bool)EventManager.Instance)
		{
			EventManager.Instance.PostEvent(primaryInstrument + "/" + notes[_index], EventAction.PlaySound, base.gameObject);
			_index++;
			if (_index >= notes.Length)
			{
				_index = 0;
			}
		}
	}
}
