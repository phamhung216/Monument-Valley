using UnityEngine;

public class UITouchHandler : MonoBehaviour
{
	private GameTouch _fakeTouch;

	private void Awake()
	{
		_fakeTouch = new GameTouch(1000);
	}

	public GameObject HitTestHover(Vector2 pos)
	{
		GetComponent<UICamera>();
		_fakeTouch.pos = Input.mousePosition;
		_fakeTouch.tapCount = 1;
		_fakeTouch.phase = TouchPhase.Began;
		return HitTest(_fakeTouch);
	}

	public GameObject HitTest(GameTouch touch)
	{
		UITouchable uITouchable = GetComponent<UILayout>().FindTouchHandler(GetComponent<UICamera>().ScreenToDPPoint(touch.pos), touch);
		if (!uITouchable)
		{
			return null;
		}
		return uITouchable.gameObject;
	}
}
