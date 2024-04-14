using UnityEngine;

public class TotemCubeProxy : GameTouchable, IHoverable
{
	public TotemPole totemCube;

	private TotemCubeProxy[] _allCubes;

	private TotemCubeProxy _selectedProxy;

	private bool dragActive;

	private Vector3 touchOffset;

	private Vector3 lastTouchPos;

	private NavBrushComponent _initialBrush;

	private int _reselectCooldown;

	private IHoverable _hoverable;

	private TotemPoleInput _totemPoleInput;

	public override bool showTouchIndicator => true;

	private void Start()
	{
		_allCubes = Object.FindObjectsOfType<TotemCubeProxy>();
		if (!totemCube)
		{
			totemCube = base.transform.parent.GetComponent<TotemPole>();
			_totemPoleInput = totemCube.GetComponent<TotemPoleInput>();
			_totemPoleInput.enabled = false;
		}
		if (_selectedProxy != null)
		{
			_hoverable = _selectedProxy;
		}
	}

	private void Update()
	{
		if ((bool)_selectedProxy)
		{
			_selectedProxy.totemCube.dragging = dragActive;
			Vector3 vector = GameScene.ScreenToPanPoint(lastTouchPos + touchOffset, Camera.main);
			_selectedProxy.totemCube.SetTargetPanSpacePos(vector);
			if (_reselectCooldown > 0)
			{
				_reselectCooldown--;
			}
			if (_initialBrush == _selectedProxy.totemCube.targetBrush)
			{
				if (_reselectCooldown <= 0)
				{
					Vector3 panSpaceDirection = vector - GameScene.WorldToPanPoint(_selectedProxy.totemCube.transform.position);
					panSpaceDirection.z = 0f;
					if (panSpaceDirection.magnitude > 0.1f && !_selectedProxy.totemCube.CanMoveInPanSpaceDirection(panSpaceDirection))
					{
						TotemCubeProxy totemCubeProxy = null;
						TotemPole componentInParent = _selectedProxy.totemCube.lastValidBrush.GetComponentInParent<TotemPole>();
						if ((bool)componentInParent)
						{
							totemCubeProxy = componentInParent.GetComponentInChildren<TotemCubeProxy>();
						}
						if (!totemCubeProxy && (bool)_selectedProxy.totemCube.blocker && (bool)(_selectedProxy.totemCube.blocker as TotemPole))
						{
							totemCubeProxy = _selectedProxy.totemCube.blocker.GetComponentInChildren<TotemCubeProxy>();
						}
						if ((bool)totemCubeProxy)
						{
							_selectedProxy.totemCube.dragging = false;
							Vector3 vector2 = Camera.main.WorldToScreenPoint(totemCubeProxy.totemCube.transform.position) - Camera.main.WorldToScreenPoint(_selectedProxy.totemCube.transform.position);
							touchOffset += vector2;
							_selectedProxy = totemCubeProxy;
							_selectedProxy.dragActive = true;
							_initialBrush = _selectedProxy.totemCube.lastValidBrush;
							_reselectCooldown = 2;
						}
					}
				}
			}
			else
			{
				_initialBrush = null;
			}
		}
		if (totemCube.dragging)
		{
			_totemPoleInput.GlowFull();
		}
		else
		{
			_totemPoleInput.GlowDecrease();
		}
	}

	public void OnHover()
	{
		_totemPoleInput.GlowHalf();
	}

	public void OnHoverEnd()
	{
	}

	private TotemCubeProxy PickCube(Vector2 screenPos)
	{
		Vector3 vector = GameScene.ScreenToPanPoint(screenPos, Camera.main);
		vector.z = 0f;
		float num = float.MaxValue;
		TotemCubeProxy totemCubeProxy = null;
		TotemCubeProxy[] allCubes = _allCubes;
		foreach (TotemCubeProxy totemCubeProxy2 in allCubes)
		{
			Vector3 vector2 = GameScene.WorldToPanPoint(totemCubeProxy2.transform.position);
			vector2.z = 0f;
			float magnitude = (vector2 - vector).magnitude;
			if (magnitude < num)
			{
				_ = (bool)totemCubeProxy;
				num = magnitude;
				totemCubeProxy = totemCubeProxy2;
			}
		}
		_ = (bool)totemCubeProxy;
		return totemCubeProxy;
	}

	public override bool AcceptTouch(GameTouch touch)
	{
		return base.AcceptTouch(touch);
	}

	public override bool ReleaseOnTapEnded(GameTouch touch)
	{
		return base.ReleaseOnTapEnded(touch);
	}

	public override void OnTouchBegan(GameTouch touch)
	{
		_selectedProxy = PickCube(touch.initialPos);
		lastTouchPos = touch.initialPos;
		touchOffset = Camera.main.WorldToScreenPoint(_selectedProxy.totemCube.transform.position) - lastTouchPos;
		dragActive = true;
		_initialBrush = _selectedProxy.totemCube.lastValidBrush;
	}

	public override void OnTouchMoved(GameTouch touch)
	{
		if (dragActive)
		{
			lastTouchPos = touch.pos;
		}
	}

	public override void OnTouchEnded(GameTouch touch)
	{
		dragActive = false;
		_selectedProxy.totemCube.dragging = dragActive;
		_selectedProxy = null;
	}

	public override void OnTouchCancelled(GameTouch touch)
	{
		dragActive = false;
		_selectedProxy.totemCube.dragging = dragActive;
		_selectedProxy.totemCube.Stop();
		_selectedProxy = null;
	}
}
