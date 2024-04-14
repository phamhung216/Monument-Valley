using System.Collections.Generic;
using UnityEngine;

public class TouchableProxyAccessor : MonoBehaviour
{
	private List<TouchableProxy> _touchableProxies = new List<TouchableProxy>();

	public void AddTouchableProxy(TouchableProxy proxy)
	{
		_touchableProxies.Add(proxy);
	}

	public void EnableTouchableProxies()
	{
		foreach (TouchableProxy touchableProxy in _touchableProxies)
		{
			touchableProxy.Enable();
		}
	}

	public void DisableTouchableProxies()
	{
		foreach (TouchableProxy touchableProxy in _touchableProxies)
		{
			touchableProxy.Disable();
		}
	}

	public int GetNumProxies()
	{
		return _touchableProxies.Count;
	}

	public Collider GetColliderOfTouchableProxyAtIndex(int index)
	{
		if (index < _touchableProxies.Count)
		{
			return _touchableProxies[index].TargetCollider;
		}
		return null;
	}
}
