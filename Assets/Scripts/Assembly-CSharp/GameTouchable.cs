using System.Collections;
using UnityEngine;

public class GameTouchable : MonoBehaviour
{
	public bool isEnabled = true;

	public bool claimOnTouchBegan = true;

	public bool claimOnTouchNotTap;

	public bool releaseOnTouchNotTap = true;

	public bool canShowTouchIndicator = true;

	public bool ignoreMultiTaps = true;

	private TouchableProxyAccessor _proxyAccessor;

	public TouchableProxyAccessor ProxyAccessor => _proxyAccessor;

	public virtual bool showTouchIndicator => canShowTouchIndicator;

	public virtual bool CanOverRideNonPhysicals()
	{
		return false;
	}

	public virtual float GetHitDistance(GameTouch touch, Ray worldRay)
	{
		return 0f;
	}

	public virtual bool AcceptTouch(GameTouch touch)
	{
		if (!base.gameObject.activeInHierarchy || !base.enabled || !isEnabled)
		{
			return false;
		}
		if (ignoreMultiTaps && touch.tapCount > 1)
		{
			return false;
		}
		if ((touch.isTap && claimOnTouchBegan) || (!touch.isTap && claimOnTouchNotTap))
		{
			return true;
		}
		return false;
	}

	public virtual bool AcceptHover()
	{
		if (!base.gameObject.activeInHierarchy || !base.enabled || !isEnabled)
		{
			return false;
		}
		return true;
	}

	public virtual bool ReleaseOnTapEnded(GameTouch touch)
	{
		return releaseOnTouchNotTap;
	}

	public virtual void OnTouchBegan(GameTouch touch)
	{
	}

	public virtual void OnTouchMoved(GameTouch touch)
	{
	}

	public virtual void OnTouchEnded(GameTouch touch)
	{
	}

	public virtual void OnTouchCancelled(GameTouch touch)
	{
	}

	public virtual void OnTouchIsSingleTap(GameTouch touch)
	{
	}

	public virtual Vector3 GetTouchIndicatorPosition(GameTouch touch)
	{
		return base.transform.position;
	}

	public void AddProxyAccessor(TouchableProxyAccessor proxyAccessor)
	{
		if (_proxyAccessor == null)
		{
			_proxyAccessor = proxyAccessor;
		}
	}

	private void EnableTouchableProxies()
	{
		if (_proxyAccessor != null)
		{
			_proxyAccessor.EnableTouchableProxies();
		}
	}

	private void DisableTouchableProxies()
	{
		if (_proxyAccessor != null)
		{
			_proxyAccessor.DisableTouchableProxies();
		}
	}

	[TriggerableAction]
	public IEnumerator Enable()
	{
		isEnabled = true;
		EnableTouchableProxies();
		return null;
	}

	[TriggerableAction]
	public IEnumerator Disable()
	{
		isEnabled = false;
		DisableTouchableProxies();
		return null;
	}
}
