using System.Collections.Generic;
using UnityEngine;

public class LocalisationLoader : MonoBehaviour
{
	public TextAsset localisedStrings;

	public List<LocalisedFonts> localisedFonts;

	private void Awake()
	{
		LocalisationManager.Instance.LoadStringTable(localisedStrings, localisedFonts);
	}
}
