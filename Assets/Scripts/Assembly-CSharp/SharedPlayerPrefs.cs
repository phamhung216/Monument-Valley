using System.Collections.Generic;

public class SharedPlayerPrefs
{
	public delegate void IntConflictHandler(string name, int mine, int theirs, out int resolved);

	public delegate void StringConflictHandler(string name, string mine, string theirs, out string resolved);

	private class LocalPrefs
	{
		private static Dictionary<string, int> _intValues = new Dictionary<string, int>();

		private static Dictionary<string, string> _stringValues = new Dictionary<string, string>();

		public static bool HasKey(string key)
		{
			if (!_intValues.ContainsKey(key))
			{
				return _stringValues.ContainsKey(key);
			}
			return true;
		}

		public static int GetInt(string key)
		{
			int value = 0;
			_intValues.TryGetValue(key, out value);
			return value;
		}

		public static void SetInt(string key, int value)
		{
			_intValues[key] = value;
		}

		public static string GetString(string key)
		{
			string value = "";
			_stringValues.TryGetValue(key, out value);
			return value;
		}

		public static void SetString(string key, string value)
		{
			_stringValues[key] = value;
		}

		public static void Save()
		{
		}
	}

	private static ISharedStorage _sharedStorage;

	private static ISharedStorage sharedStorage
	{
		get
		{
			if (_sharedStorage == null)
			{
				_sharedStorage = new SharedStorageSteamworks();
			}
			return _sharedStorage;
		}
	}

	public static event IntConflictHandler IntConflictDetected;

	public static event StringConflictHandler StringConflictDetected;

	private static void OnIntConflictDetected(string name, int mine, int theirs, ref int resolved)
	{
		if (SharedPlayerPrefs.IntConflictDetected != null)
		{
			SharedPlayerPrefs.IntConflictDetected(name, mine, theirs, out resolved);
		}
	}

	private static void OnStringConflictDetected(string name, string mine, string theirs, ref string resolved)
	{
		if (SharedPlayerPrefs.StringConflictDetected != null)
		{
			SharedPlayerPrefs.StringConflictDetected(name, mine, theirs, out resolved);
		}
	}

	public static void Reset()
	{
		sharedStorage.Reset();
	}

	public static bool HasKey(string key)
	{
		return LocalPrefs.HasKey(key);
	}

	public static int GetInt(string key)
	{
		return LocalPrefs.GetInt(key);
	}

	public static void SetInt(string key, int value)
	{
		LocalPrefs.SetInt(key, value);
		sharedStorage.SetInt(key, value);
	}

	public static string GetString(string key)
	{
		return LocalPrefs.GetString(key);
	}

	public static void SetString(string key, string value)
	{
		LocalPrefs.SetString(key, value);
		sharedStorage.SetString(key, value);
	}

	public static void Save()
	{
		LocalPrefs.Save();
		sharedStorage.Synchronise();
	}

	public static void Sync(bool _save = true)
	{
		sharedStorage.Synchronise();
		int count = sharedStorage.count;
		for (int i = 0; i < count; i++)
		{
			string keyAtIndex = sharedStorage.GetKeyAtIndex(i);
			if (keyAtIndex == null)
			{
				continue;
			}
			if (keyAtIndex == UserDataController.LastPlayedLevelKey)
			{
				string @string = sharedStorage.GetString(keyAtIndex);
				string string2 = GetString(keyAtIndex);
				if (@string != string2)
				{
					string resolved = string2;
					OnStringConflictDetected(keyAtIndex, string2, @string, ref resolved);
					SetString(keyAtIndex, resolved);
				}
			}
			else
			{
				int @int = sharedStorage.GetInt(keyAtIndex);
				int int2 = GetInt(keyAtIndex);
				if (@int != int2)
				{
					int resolved2 = int2;
					OnIntConflictDetected(keyAtIndex, int2, @int, ref resolved2);
					SetInt(keyAtIndex, resolved2);
				}
			}
		}
		if (_save)
		{
			Save();
		}
	}
}
