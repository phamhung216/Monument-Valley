using UnityEngine;

public class UIScrollView : MonoBehaviour
{
	public UILayout viewport;

	public UILayout content;

	public UILayout scrollBarBackdrop;

	public UILayout scrollBarMarker;

	private float _scrollValue;

	public float debugScrollSpeed;

	private bool debugScrollDown = true;

	public float mouseScrollSensitivity = 20f;

	private bool _hasUpdatedScrollBar;

	public float scrollValue
	{
		get
		{
			return _scrollValue;
		}
		set
		{
			_scrollValue = value;
			OnScrollValueChanged();
		}
	}

	private void Start()
	{
	}

	private void Update()
	{
		float num = 0f;
		if (debugScrollSpeed > 0f && content.layoutHeight > viewport.layoutHeight)
		{
			float num2 = (debugScrollDown ? 1f : (-1f));
			if (_scrollValue > content.layoutHeight - viewport.layoutHeight)
			{
				debugScrollDown = false;
			}
			else if (_scrollValue < 0f)
			{
				debugScrollDown = true;
			}
			num = num2 * Time.deltaTime * debugScrollSpeed;
		}
		bool flag = viewport.GetLayoutOpacity() > 0f;
		if (Input.mouseScrollDelta.y != 0f && flag)
		{
			num = (0f - mouseScrollSensitivity) * Input.mouseScrollDelta.y;
		}
		if (num != 0f)
		{
			_scrollValue += num;
			float max = Mathf.Max(0f, content.layoutHeight - viewport.layoutHeight);
			_scrollValue = Mathf.Clamp(_scrollValue, 0f, max);
			OnScrollValueChanged();
		}
		else if (!_hasUpdatedScrollBar)
		{
			UpdateScrollBar();
		}
	}

	private void OnScrollValueChanged()
	{
		content.layoutMarginTop = 0f - _scrollValue;
		content.Unfinalise();
		content.Layout();
		UpdateScrollBar();
	}

	private void UpdateScrollBar()
	{
		if ((bool)scrollBarMarker)
		{
			scrollBarMarker.layoutHeight = Mathf.Min(1f, viewport.layoutHeight / content.layoutHeight) * viewport.layoutHeight;
			scrollBarMarker.layoutMarginTop = _scrollValue / content.layoutHeight * viewport.layoutHeight;
			scrollBarMarker.Unfinalise();
			scrollBarMarker.Layout();
		}
	}
}
