using UnityEngine;

public class TotemPoleInput : Dragger, IHoverable
{
	private bool dragActive;

	private Vector3 touchOffset;

	private Vector3 lastTouchPos;

	public TotemPole pole;

	private bool _hoverActive;

	public override GameObject targetObject => base.gameObject;

	public override bool showTouchIndicator => true;

	private void Update()
	{
		pole.dragging = dragActive;
		if (dragActive)
		{
			pole.SetTargetPanSpacePos(GameScene.ScreenToPanPoint(lastTouchPos + touchOffset, Camera.main));
			GlowFull();
		}
		else if (_hoverActive)
		{
			GlowHalf();
		}
		else
		{
			GlowDecrease();
		}
		_hoverActive = false;
	}

	public override void StartDrag(Vector3 position)
	{
		touchOffset = Camera.main.WorldToScreenPoint(pole.transform.position) - position;
		dragActive = true;
	}

	public override void Drag(Vector3 position, Vector3 delta)
	{
		if (dragActive)
		{
			lastTouchPos = position + delta;
		}
	}

	public new void OnHover()
	{
		_hoverActive = true;
	}

	public override void OnTouchEnded(GameTouch touch)
	{
		dragActive = false;
		pole.dragging = dragActive;
	}

	public override void CancelDrag()
	{
		base.CancelDrag();
		dragActive = false;
		pole.dragging = dragActive;
		pole.Stop();
	}

	public override void OnTouchCancelled(GameTouch touch)
	{
		dragActive = false;
		pole.dragging = dragActive;
		pole.Stop();
	}
}
