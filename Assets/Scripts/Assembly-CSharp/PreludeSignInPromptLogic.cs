using System.Collections;
using UnityEngine;

public class PreludeSignInPromptLogic : MonoBehaviour
{
	public TriggerableActionSequence _signInPromptSequence;

	public TriggerableActionSequence _signInCompleteSequence;

	public TriggerableActionSequence _leaveLevelSequence;

	public CloudSave _signInHandler;

	public UIText _signInButtonText;

	private bool _startedSignInProcess;

	[TriggerableAction]
	public IEnumerator OnCompleteButtonPressed()
	{
		_leaveLevelSequence.TriggerActions();
		return null;
	}

	private void Update()
	{
	}
}
