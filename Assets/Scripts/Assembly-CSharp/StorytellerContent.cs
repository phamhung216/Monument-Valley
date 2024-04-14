using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StorytellerContent : MonoBehaviour
{
	public List<string> screenTexts = new List<string>();

	public UIText uiText;

	private int _currentTextIdx;

	public bool setOnStartUp;

	public TriggerableActionSequence nextSequence;

	public TriggerableActionSequence exitSequence;

	private Storyteller _storyteller;

	private void Start()
	{
		if (setOnStartUp)
		{
			DoUseFirstText();
		}
		_storyteller = Object.FindObjectOfType<Storyteller>();
	}

	[TriggerableAction]
	public IEnumerator UseFirstText()
	{
		DoUseFirstText();
		return null;
	}

	public void DoUseFirstText()
	{
		_currentTextIdx = 0;
		uiText.SetText(screenTexts[_currentTextIdx]);
	}

	[TriggerableAction]
	public IEnumerator ButtonPressed()
	{
		if (_currentTextIdx + 1 < screenTexts.Count)
		{
			nextSequence.TriggerActions();
		}
		else
		{
			exitSequence.TriggerActions();
			if ((bool)_storyteller)
			{
				_storyteller.TalkEnded();
			}
		}
		return null;
	}

	[TriggerableAction]
	public IEnumerator UseNextText()
	{
		_currentTextIdx++;
		uiText.SetText(screenTexts[_currentTextIdx]);
		return null;
	}
}
