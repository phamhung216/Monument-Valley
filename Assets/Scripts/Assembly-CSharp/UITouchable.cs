using UnityCommon;
using UnityEngine;

[RequireComponent(typeof(UILayout))]
public class UITouchable : GameTouchable
{
	public float touchMargin = 64f;

	protected Rect _touchRect = Rect.zero;

	public float touchableOpacity = 1f;

	public Rect touchRect => _touchRect;

	public virtual void Start()
	{
		if (OrientationOverrideManager.IsLandscape())
		{
			touchMargin = Mathf.Min(16f, touchMargin);
		}
	}

	public void UpdateTouchRect(UILayout layout)
	{
		if ((bool)layout)
		{
			_touchRect = new Rect(layout.left - touchMargin, layout.bottom - touchMargin, layout.layoutWidth + 2f * touchMargin, layout.layoutHeight + 2f * touchMargin);
		}
	}

	public bool AcceptTouch(Vector2 dpPos, GameTouch touch)
	{
		UILayout component = GetComponent<UILayout>();
		if ((bool)component && component.GetLayoutOpacity() < touchableOpacity)
		{
			return false;
		}
		if (base.enabled && base.gameObject.activeInHierarchy)
		{
			if (_touchRect.Contains(dpPos))
			{
				return base.AcceptTouch(touch);
			}
			return false;
		}
		return false;
	}

	public bool AcceptHover(Vector2 dpPos)
	{
		UILayout component = GetComponent<UILayout>();
		if ((bool)component && component.GetLayoutOpacity() < touchableOpacity)
		{
			return false;
		}
		if (base.enabled && base.gameObject.activeInHierarchy)
		{
			if (_touchRect.Contains(dpPos))
			{
				return base.AcceptHover();
			}
			return false;
		}
		return false;
	}

	protected void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.yellow;
		Vector3 vector = new Vector3(_touchRect.xMin, _touchRect.yMin, 0f);
		Vector3 to = new Vector3(_touchRect.xMax, _touchRect.yMin, 0f);
		Vector3 vector2 = new Vector3(_touchRect.xMin, _touchRect.yMax, 0f);
		Vector3 from = new Vector3(_touchRect.xMax, _touchRect.yMax, 0f);
		Gizmos.DrawLine(vector, to);
		Gizmos.DrawLine(from, to);
		Gizmos.DrawLine(from, vector2);
		Gizmos.DrawLine(vector2, vector);
	}
}
