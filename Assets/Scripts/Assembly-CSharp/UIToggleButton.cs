public class UIToggleButton : UIButton
{
	public string onTextureName;

	public string offTextureName;

	public string onTextStringID;

	public string offTextStringID;

	private bool _isSelected;

	public UIText buttonText;

	private UIImage _buttonImage;

	public bool isSelected
	{
		get
		{
			return _isSelected;
		}
		set
		{
			_isSelected = value;
			UpdateButtonImage();
		}
	}

	public override void OnTouchEnded(GameTouch touch)
	{
		base.OnTouchEnded(touch);
		if (!releaseOnTouchNotTap || touch.isTap)
		{
			UILayout component = GetComponent<UILayout>();
			if (_touchRect.Contains(component.rootLayout.ScreenToDPPoint(touch.pos)))
			{
				_isSelected = !_isSelected;
				UpdateButtonImage();
			}
		}
	}

	public void UpdateButtonImage()
	{
		if ((bool)_buttonImage)
		{
			if (_isSelected)
			{
				if (_buttonImage.atlas.atlas.subTextures.ContainsKey(onTextureName))
				{
					_buttonImage.SetSubTextureName(onTextureName);
				}
			}
			else if (_buttonImage.atlas.atlas.subTextures.ContainsKey(offTextureName))
			{
				_buttonImage.SetSubTextureName(offTextureName);
			}
		}
		if ((bool)buttonText)
		{
			buttonText.SetText(_isSelected ? onTextStringID : offTextStringID);
		}
	}

	public override void Start()
	{
		base.Start();
		if (_images == null)
		{
			_images = GetComponentsInChildren<UIImage>();
		}
		if (_images.Length != 0)
		{
			_buttonImage = _images[0];
		}
		if ((bool)buttonText)
		{
			buttonText.SetText(_isSelected ? onTextStringID : offTextStringID);
		}
	}

	private new void OnDrawGizmosSelected()
	{
		base.OnDrawGizmosSelected();
	}
}
