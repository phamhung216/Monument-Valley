using System.Collections;
using UnityEngine;

public class InstrumentChanger : MonoBehaviour
{
	public MoverAudio moverAudio;

	public string newInstrument;

	[TriggerableAction]
	public IEnumerator ChangeInstrument()
	{
		moverAudio.audioEvent = newInstrument;
		return null;
	}
}
