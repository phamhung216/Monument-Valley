using UnityEngine;

public class TouchableProxy : GameTouchable, IHoverable
{
	public GameTouchable touchable;

	private IHoverable _hoverable;

	private Collider _collider;

	private TouchableProxyAccessor _accessor;

	public Collider TargetCollider => _collider;

	private void Start()
	{
		_collider = base.gameObject.GetComponent<Collider>();
		if ((bool)touchable)
		{
			claimOnTouchBegan = touchable.claimOnTouchBegan;
			claimOnTouchNotTap = touchable.claimOnTouchNotTap;
			releaseOnTouchNotTap = touchable.releaseOnTouchNotTap;
			_accessor = touchable.ProxyAccessor;
			if (_accessor == null)
			{
				_accessor = touchable.gameObject.AddComponent<TouchableProxyAccessor>();
				touchable.AddProxyAccessor(_accessor);
			}
			_accessor.AddTouchableProxy(this);
		}
		if (touchable != null)
		{
			_hoverable = touchable as IHoverable;
		}
	}

	private void Update()
	{
	}

	public override bool AcceptTouch(GameTouch touch)
	{
		if ((bool)touchable)
		{
			return touchable.AcceptTouch(touch);
		}
		return false;
	}

	public override bool ReleaseOnTapEnded(GameTouch touch)
	{
		if ((bool)touchable)
		{
			return touchable.ReleaseOnTapEnded(touch);
		}
		return false;
	}

	public override void OnTouchBegan(GameTouch touch)
	{
		if ((bool)touchable)
		{
			touchable.OnTouchBegan(touch);
		}
	}

	public override void OnTouchMoved(GameTouch touch)
	{
		if ((bool)touchable)
		{
			touchable.OnTouchMoved(touch);
		}
	}

	public override void OnTouchEnded(GameTouch touch)
	{
		if ((bool)touchable)
		{
			touchable.OnTouchEnded(touch);
		}
	}

	public override void OnTouchCancelled(GameTouch touch)
	{
		if ((bool)touchable)
		{
			touchable.OnTouchCancelled(touch);
		}
	}

	public override void OnTouchIsSingleTap(GameTouch touch)
	{
		if ((bool)touchable)
		{
			touchable.OnTouchIsSingleTap(touch);
		}
	}

	public override bool AcceptHover()
	{
		if ((bool)touchable)
		{
			return touchable.AcceptHover();
		}
		return false;
	}

	public void OnHover()
	{
		if (_hoverable != null)
		{
			_hoverable.OnHover();
		}
	}

	public void OnHoverEnd()
	{
	}
}
