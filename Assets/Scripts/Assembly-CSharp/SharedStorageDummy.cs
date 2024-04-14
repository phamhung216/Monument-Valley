public class SharedStorageDummy : ISharedStorage
{
	public int count => 0;

	public bool HasKey(string key)
	{
		return false;
	}

	public string GetKeyAtIndex(int index)
	{
		return "";
	}

	public void SetInt(string key, int value)
	{
	}

	public int GetInt(string key)
	{
		return 0;
	}

	public void SetString(string key, string value)
	{
	}

	public string GetString(string key)
	{
		return "";
	}

	public void Synchronise()
	{
	}

	public void Reset()
	{
	}
}
