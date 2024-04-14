using UnityEngine;

public class ScriptDefineEnabler : MonoBehaviour
{
	public enum SupportedDefine
	{
		DISABLE_EXTERNAL_LINKS = 0,
		DISABLE_CAMERA_MODE = 1,
		DISABLE_SHARING = 2,
		FS_UNLOCKED = 3,
		DISABLE_CROSS_PROMO = 4
	}

	public enum EnableMode
	{
		EnableOnSymbol = 0,
		DisableOnSymbol = 1
	}

	public SupportedDefine defineSymbol;

	public EnableMode enableMode;

	public GameObject[] targets;

	private void Awake()
	{
		CheckTargets(SupportedDefine.FS_UNLOCKED);
	}

	private void CheckTargets(SupportedDefine symbol)
	{
		if (defineSymbol == symbol)
		{
			GameObject[] array = targets;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetActive(enableMode == EnableMode.EnableOnSymbol);
			}
		}
	}
}
