using System.Collections;
using UnityEngine;

public class SecretObject : MonoBehaviour
{
	public int secretNumber;

	private void Start()
	{
		if (LevelManager.Instance.IsLevelSecretCollected(LevelManager.CurrentLevel, secretNumber))
		{
			base.gameObject.SetActive(value: false);
		}
	}

	private void Update()
	{
	}

	[TriggerableAction]
	public IEnumerator CollectSecret()
	{
		LevelManager.Instance.SetLevelSecretCollected(LevelManager.CurrentLevel, secretNumber);
		return null;
	}
}
