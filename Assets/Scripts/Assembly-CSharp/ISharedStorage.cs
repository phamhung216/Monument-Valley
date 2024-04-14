public interface ISharedStorage
{
	int count { get; }

	bool HasKey(string key);

	string GetKeyAtIndex(int index);

	void SetInt(string key, int value);

	int GetInt(string key);

	void SetString(string key, string value);

	string GetString(string key);

	void Synchronise();

	void Reset();
}
