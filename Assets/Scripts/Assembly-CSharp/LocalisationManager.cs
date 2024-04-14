using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LocalisationManager
{
	private Dictionary<Font, Font> _fontMap = new Dictionary<Font, Font>();

	private static LocalisationManager _instance;

	private DeviceLanguage _currentLanguage;

	private LocalisedFonts _fontInfo;

	private DeviceLanguage[] _languages;

	private Dictionary<string, string> _stringTable = new Dictionary<string, string>();

	private UnityEvent<DeviceLanguage> _onLanguageChanged = new UnityEvent<DeviceLanguage>();

	private TextAsset _stringTableTextAsset;

	private List<LocalisedFonts> _fontOverrides;

	private Dictionary<string, DeviceLanguage> _languageNameLookup = new Dictionary<string, DeviceLanguage>();

	public UnityEvent<DeviceLanguage> onLanguageChanged => _onLanguageChanged;

	public static LocalisationManager Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new LocalisationManager();
			}
			return _instance;
		}
	}

	public DeviceLanguage[] languages => _languages;

	public DeviceLanguage currentLanguage => _currentLanguage;

	public LocalisedFonts fontInfo => _fontInfo;

	public LocalisedFonts defaultFontInfo
	{
		get
		{
			if (_fontOverrides == null)
			{
				return null;
			}
			return _fontOverrides[0];
		}
	}

	private LocalisationManager()
	{
		foreach (DeviceLanguage value in Enum.GetValues(typeof(DeviceLanguage)))
		{
			string languageString = GetLanguageString(value);
			_languageNameLookup[languageString] = value;
		}
	}

	public void Clear()
	{
		_stringTable.Clear();
	}

	public void SetAndSaveCurrentLanguage(DeviceLanguage language)
	{
		bool num = language != currentLanguage;
		LoadStringTableWithLanguage(_stringTableTextAsset, language, _fontOverrides);
		UserDataController.SetUserLocalPrefsString("language", GetLanguageString(language));
		if (num)
		{
			onLanguageChanged.Invoke(currentLanguage);
		}
	}

	public void LoadStringTableWithLanguage(TextAsset textAsset, DeviceLanguage overrideLanguage, List<LocalisedFonts> fonts)
	{
		LoadStringTable(textAsset, overrideLanguage, fonts);
	}

	public void LoadStringTable(TextAsset textAsset, List<LocalisedFonts> fonts)
	{
		DeviceLanguage language = DeviceUtils.GetDeviceLanguage();
		if (UserDataController.HasLocalUserPrefsKey("language"))
		{
			string userLocalPrefsString = UserDataController.GetUserLocalPrefsString("language");
			if (_languageNameLookup.ContainsKey(userLocalPrefsString))
			{
				language = _languageNameLookup[userLocalPrefsString];
			}
		}
		LoadStringTable(textAsset, language, fonts);
	}

	private string GetLanguageString(DeviceLanguage language)
	{
		return language.ToString("G").Trim();
	}

	private void LoadStringTable(TextAsset textAsset, DeviceLanguage language, List<LocalisedFonts> fonts)
	{
		if (_stringTable.Count > 0 && _currentLanguage == language)
		{
			return;
		}
		_stringTableTextAsset = textAsset;
		_fontOverrides = fonts;
		CSVReader cSVReader = new CSVReader();
		cSVReader.Test();
		_currentLanguage = language;
		cSVReader.LoadFromTextAsset(textAsset);
		int num = 2;
		_languages = new DeviceLanguage[cSVReader.GetRowElementCount(0) - num];
		for (int i = 0; i < _languages.Length; i++)
		{
			int colIdx = i + num;
			string key = cSVReader.GetRowElement(0, colIdx).Trim();
			_languages[i] = _languageNameLookup[key];
			if (i != 0 && _languages[i] != _currentLanguage)
			{
				continue;
			}
			for (int j = 1; j < cSVReader.RowCount; j++)
			{
				string rowElement = cSVReader.GetRowElement(j, 0);
				string rowElement2 = cSVReader.GetRowElement(j, colIdx);
				if (rowElement2.Length > 0)
				{
					rowElement = "$(" + rowElement + ")";
					_stringTable[rowElement] = rowElement2;
				}
			}
		}
		LocalisedFonts localisedFonts = null;
		LocalisedFonts localisedFonts2 = null;
		foreach (LocalisedFonts font in fonts)
		{
			if (font.language == DeviceLanguage.English)
			{
				localisedFonts = font;
			}
			if (font.language == language)
			{
				localisedFonts2 = font;
			}
		}
		if (localisedFonts2 == null)
		{
			localisedFonts2 = localisedFonts;
		}
		SetLocalisedFont(localisedFonts.storytellerFont, localisedFonts2.storytellerFont);
		SetLocalisedFont(localisedFonts.sansSerifFont, localisedFonts2.sansSerifFont);
		SetLocalisedFont(localisedFonts.serifFont, localisedFonts2.serifFont);
		_fontInfo = localisedFonts2;
	}

	private void SetLocalisedFont(Font defaultFont, Font localisedFont)
	{
		_fontMap[defaultFont] = localisedFont;
	}

	public Font LocaliseFont(Font font)
	{
		if (_fontMap.ContainsKey(font))
		{
			return _fontMap[font];
		}
		return font;
	}

	public string LocaliseString(string unlocalised)
	{
		try
		{
			return _stringTable[unlocalised];
		}
		catch (Exception)
		{
			return unlocalised;
		}
	}

	public string GetStringWithKey(string key)
	{
		try
		{
			return _stringTable[key];
		}
		catch (Exception)
		{
			return null;
		}
	}
}
