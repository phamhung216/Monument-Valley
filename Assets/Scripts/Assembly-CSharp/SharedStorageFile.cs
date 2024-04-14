using System;
using System.IO;
using System.Linq;
using UnityCommon;
using UnityEngine;

public class SharedStorageFile : ISharedStorage
{
	[Serializable]
	private class StorageData
	{
		public SerializableDictionary_string_int _intValues = new SerializableDictionary_string_int();

		public SerializableDictionary_string_string _stringValues = new SerializableDictionary_string_string();
	}

	private StorageData _data = new StorageData();

	protected string _filePath;

	public int count => _data._intValues.Count + _data._stringValues.Count;

	public SharedStorageFile()
	{
		_filePath = Application.persistentDataPath;
		_filePath = _filePath + Path.DirectorySeparatorChar + "user_data.sav";
	}

	public bool HasKey(string key)
	{
		if (_data._intValues.ContainsKey(key))
		{
			return true;
		}
		return _data._stringValues.ContainsKey(key);
	}

	public string GetKeyAtIndex(int index)
	{
		if (index < _data._intValues.Count)
		{
			return Enumerable.ElementAt(_data._intValues.GetKeys(), index);
		}
		if (index < _data._stringValues.Count)
		{
			return Enumerable.ElementAt(_data._stringValues.GetKeys(), index);
		}
		return "";
	}

	public void SetInt(string key, int value)
	{
		_data._intValues[key] = value;
	}

	public int GetInt(string key)
	{
		if (_data._intValues.ContainsKey(key))
		{
			return _data._intValues[key];
		}
		return 0;
	}

	public void SetString(string key, string value)
	{
		_data._stringValues[key] = value;
	}

	public string GetString(string key)
	{
		if (_data._stringValues.ContainsKey(key))
		{
			return _data._stringValues[key];
		}
		return "";
	}

	public virtual void Synchronise()
	{
		if (count == 0)
		{
			LoadJson();
		}
		else
		{
			SaveJson();
		}
	}

	public void Reset()
	{
		try
		{
			if (File.Exists(_filePath))
			{
				File.Delete(_filePath);
			}
		}
		catch (Exception ex)
		{
			D.Error(ex);
			throw;
		}
	}

	private void SaveJson()
	{
		try
		{
			string contents = JsonUtility.ToJson(_data);
			FileInfo fileInfo = new FileInfo(_filePath);
			fileInfo.Directory.Create();
			File.WriteAllText(fileInfo.FullName, contents);
		}
		catch (Exception ex)
		{
			D.Error(ex);
			throw;
		}
	}

	private void LoadJson()
	{
		try
		{
			if (File.Exists(_filePath))
			{
				_data = JsonUtility.FromJson<StorageData>(File.ReadAllText(_filePath));
			}
		}
		catch (Exception ex)
		{
			D.Error(ex);
			throw;
		}
	}
}
