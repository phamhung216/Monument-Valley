using UnityEngine;

public class NavBrushTouchProxy : GameTouchable
{
	public NavBrushComponent navBrush;

	public Vector3 touchDirection;

	public override bool showTouchIndicator => false;

	public bool CanAcceptRay(Vector3 rayDir)
	{
		return Vector3.Dot(base.transform.TransformDirection(touchDirection), rayDir) < 0f;
	}

	public override void OnTouchEnded(GameTouch touch)
	{
		base.OnTouchEnded(touch);
		if (touch.tapCount == 1 && navBrush.touchable && navBrush.gameObject.activeInHierarchy)
		{
			PlayerInput component = GameScene.player.GetComponent<PlayerInput>();
			if (component.CanAcceptTouches())
			{
				component.MoveTo(navBrush);
				component.PlayDestinationEffect(navBrush);
			}
		}
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.green;
		Gizmos.DrawRay(new Ray(base.transform.position, base.transform.TransformDirection(touchDirection)));
	}
}
