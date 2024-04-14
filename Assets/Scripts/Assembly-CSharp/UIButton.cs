using System.Collections.Generic;
using Fabric;
using UnityEngine;
using UnityEngine.Events;

public class UIButton : UITouchable, IHoverable
{
	public enum DimmingMode
	{
		Darken = 0,
		OverDarken = 1,
		Lighten = 2
	}

	private struct DimmingColours
	{
		public Color hoverState;

		public Color downState;
	}

	public ActionSequence onPressed;

	public string onPressedAudioEvent = "User/ButtonSuccess";

	public UnityEvent onPressedUIEvent;

	public DimmingMode dimmingMode;

	private DimmingColours[] _dimmingColours = new DimmingColours[3]
	{
		new DimmingColours
		{
			hoverState = new Color(0.78f, 0.78f, 0.78f, 1f),
			downState = new Color(0.5f, 0.5f, 0.5f, 1f)
		},
		new DimmingColours
		{
			hoverState = new Color(0.6f, 0.6f, 0.6f, 1f),
			downState = new Color(0.1f, 0.1f, 0.1f, 1f)
		},
		new DimmingColours
		{
			hoverState = new Color(1.5f, 1.5f, 1.5f, 1f),
			downState = new Color(2f, 2f, 2f, 1f)
		}
	};

	public List<UIText> _textToDim;

	public bool dimButton = true;

	protected UIImage[] _images;

	private bool _isTouched;

	public override void Start()
	{
		base.Start();
		releaseOnTouchNotTap = false;
		ignoreMultiTaps = false;
	}

	public override void OnTouchBegan(GameTouch touch)
	{
		base.OnTouchBegan(touch);
		if (dimButton)
		{
			SetButtonGraphicColour(_dimmingColours[(int)dimmingMode].downState);
		}
		_isTouched = true;
	}

	private void SetButtonGraphicColour(Color colour)
	{
		if (_images == null)
		{
			_images = GetComponentsInChildren<UIImage>();
			UIText[] componentsInChildren = GetComponentsInChildren<UIText>();
			_textToDim.AddRange(componentsInChildren);
		}
		for (int i = 0; i < _images.Length; i++)
		{
			if (_images[i].canBeDimmedByButton)
			{
				Color color = colour * _images[i].originalColor;
				color.a = _images[i].color.a;
				_images[i].color = color;
			}
		}
		for (int j = 0; j < _textToDim.Count; j++)
		{
			_textToDim[j].color = colour * _textToDim[j].originalColor;
		}
	}

	public override void OnTouchMoved(GameTouch touch)
	{
		base.OnTouchMoved(touch);
		UILayout component = GetComponent<UILayout>();
		if (!_touchRect.Contains(component.rootLayout.ScreenToDPPoint(touch.pos)))
		{
			RestoreButtonColor();
		}
	}

	public override void OnTouchCancelled(GameTouch touch)
	{
		base.OnTouchCancelled(touch);
		RestoreButtonColor();
		_isTouched = false;
	}

	public override void OnTouchEnded(GameTouch touch)
	{
		base.OnTouchEnded(touch);
		if (!releaseOnTouchNotTap || touch.isTap)
		{
			UILayout component = GetComponent<UILayout>();
			if (_touchRect.Contains(component.rootLayout.ScreenToDPPoint(touch.pos)))
			{
				ExecuteButtonActions();
			}
		}
		_isTouched = false;
	}

	public void OnHover()
	{
		if (dimButton && !_isTouched)
		{
			SetButtonGraphicColour(_dimmingColours[(int)dimmingMode].hoverState);
		}
	}

	public void OnHoverEnd()
	{
		RestoreButtonColor();
	}

	public void ExecuteButtonActions()
	{
		if (((_images != null && _images.Length != 0) || dimButton) && onPressedAudioEvent.Length > 0 && (bool)EventManager.Instance)
		{
			EventManager.Instance.PostEvent(onPressedAudioEvent, EventAction.PlaySound);
		}
		RestoreButtonColor();
		onPressedUIEvent.Invoke();
		if (onPressed.actionCount > 0)
		{
			StartCoroutine(onPressed.DoSequence());
		}
	}

	private void RestoreButtonColor()
	{
		if (_images != null && dimButton)
		{
			for (int i = 0; i < _images.Length; i++)
			{
				Color originalColor = _images[i].originalColor;
				originalColor.a = _images[i].color.a;
				_images[i].color = originalColor;
			}
		}
		for (int j = 0; j < _textToDim.Count; j++)
		{
			_textToDim[j].color = _textToDim[j].originalColor;
		}
	}

	private new void OnDrawGizmosSelected()
	{
		base.OnDrawGizmosSelected();
	}
}
