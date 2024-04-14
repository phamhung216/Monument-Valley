using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogoAnimator : MonoBehaviour
{
	private List<Animation> _logoAnimations;

	public string logoAnimationString;

	public Transform[] logoPieces;

	private void Start()
	{
		_logoAnimations = new List<Animation>();
		LocalisationManager.Instance.onLanguageChanged.AddListener(OnLanguageChanged);
		OnLanguageChanged(LocalisationManager.Instance.currentLanguage);
	}

	private void OnLanguageChanged(DeviceLanguage language)
	{
		string text = language.ToString();
		if ((uint)(language - 5) > 1u)
		{
			text = "Default";
		}
		_logoAnimations.Clear();
		Transform[] array = logoPieces;
		for (int i = 0; i < array.Length; i++)
		{
			Animation[] componentsInChildren = array[i].GetComponentsInChildren<Animation>(includeInactive: true);
			foreach (Animation animation in componentsInChildren)
			{
				if (animation.gameObject.name == text)
				{
					animation.gameObject.SetActive(value: true);
					_logoAnimations.Add(animation);
				}
				else
				{
					animation.gameObject.SetActive(value: false);
				}
			}
		}
		foreach (Animation logoAnimation in _logoAnimations)
		{
			logoAnimation[logoAnimationString].enabled = true;
			logoAnimation[logoAnimationString].weight = 1f;
			logoAnimation[logoAnimationString].time = 0f;
			logoAnimation[logoAnimationString].speed = 0f;
		}
	}

	[TriggerableAction]
	public IEnumerator DoLogoAnimation()
	{
		foreach (Animation logoAnimation in _logoAnimations)
		{
			logoAnimation[logoAnimationString].enabled = true;
			logoAnimation[logoAnimationString].weight = 1f;
			logoAnimation[logoAnimationString].speed = 1f;
		}
		return null;
	}
}
