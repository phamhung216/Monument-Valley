using UnityEngine;

public class HardwareBackKeyHandler : MonoBehaviour
{
	[HideInInspector]
	public bool handlerActive;

	private UILayout _buttonLayout;

	private UIButton _button;

	private TouchHandler _touchHandler;

	private float _inputCooldown;

	private void Start()
	{
		_buttonLayout = GetComponent<UILayout>();
		_ = _buttonLayout == null;
		_button = GetComponent<UIButton>();
		_ = _button == null;
		_touchHandler = Camera.main.GetComponent<TouchHandler>();
	}

	private bool IsVisible(UILayout layout)
	{
		if (layout.parentLayout == layout.rootLayout)
		{
			return layout.opacity == 1f;
		}
		if (layout.opacity == 1f)
		{
			return IsVisible(layout.parentLayout);
		}
		return false;
	}

	private void Update()
	{
		if (!_touchHandler.touchDisabled && IsVisible(_buttonLayout))
		{
			handlerActive = true;
			if (Input.GetKeyDown(KeyCode.Escape) && _inputCooldown <= 0f)
			{
				_inputCooldown = 1f;
				_button.ExecuteButtonActions();
			}
		}
		else
		{
			handlerActive = false;
		}
		if (_inputCooldown > 0f)
		{
			_inputCooldown -= Time.deltaTime;
		}
	}
}
