using UnityEngine;

public class HardwareExitHandler : MonoBehaviour
{
	private HardwareBackKeyHandler[] _keyHandlers;

	private TouchHandler _touchHandler;

	private void Start()
	{
		_keyHandlers = Object.FindObjectsOfType<HardwareBackKeyHandler>();
		_touchHandler = Camera.main.GetComponent<TouchHandler>();
	}

	private void Update()
	{
	}

	public void QuitApplication()
	{
		Application.Quit();
	}
}
