using System.Collections.Generic;
using UnityEngine;

public class UILayout : MonoBehaviour
{
	public enum SizeMode
	{
		MatchParent = 0,
		WrapContent = 1,
		Fixed = 2
	}

	public enum HorizontalAlignMode
	{
		ParentLeft = 0,
		ParentCentre = 1,
		ParentRight = 2
	}

	public enum VerticalAlignMode
	{
		ParentTop = 0,
		ParentCentre = 1,
		ParentBottom = 2
	}

	public enum AlignMode
	{
		ParentMin = 0,
		ParentCentre = 1,
		ParentMax = 2
	}

	public enum Margin
	{
		Lower = 0,
		Upper = 1
	}

	public enum Axis
	{
		X = 0,
		Y = 1
	}

	public SizeMode layoutWidthMode = SizeMode.Fixed;

	public float layoutWidth;

	public SizeMode layoutHeightMode = SizeMode.Fixed;

	public float layoutHeight;

	public float layoutMarginTop;

	public float layoutMarginBottom;

	public float layoutMarginLeft;

	public float layoutMarginRight;

	public HorizontalAlignMode horizontalAlignMode;

	public VerticalAlignMode verticalAlignMode;

	public float opacity = 1f;

	private UICamera _rootLayout;

	private UILayout _parentLayout;

	protected List<UILayout> _childLayouts = new List<UILayout>();

	private List<UILayout> _childZOrder;

	protected bool _widthFinalised;

	protected bool _heightFinalised;

	private bool _xLayoutFinalised;

	protected bool _yLayoutFinalised;

	protected bool _layoutFinalised;

	private Vector2 _anchorParentOffset = new Vector2(0f, 0f);

	private UILayoutContent _content;

	private bool _hasContent;

	private UITouchable _touchable;

	private Vector3 _centre;

	private Animation _animationComponent;

	private bool _hasAnimation;

	private bool _isAnimating;

	[HideInInspector]
	public float layoutHeightScale = 1f;

	public Vector2 layoutSize => new Vector2(layoutWidth, layoutHeight);

	public float left => _centre.x - layoutWidth / 2f;

	public float right => _centre.x + layoutWidth / 2f;

	public float top => _centre.y + layoutHeight / 2f;

	public float bottom => _centre.y - layoutHeight / 2f;

	public Vector3 centre
	{
		get
		{
			return _centre;
		}
		set
		{
			if (_centre.x != value.x || _centre.y != value.y)
			{
				_centre = value;
				_centre.z = base.transform.position.z;
				base.transform.position = _centre;
			}
		}
	}

	public UICamera rootLayout => _rootLayout;

	public UILayout parentLayout => _parentLayout;

	public Animation animationComponent => _animationComponent;

	public int childLayoutCount => _childLayouts.Count;

	public float GetSize(int axis)
	{
		if (axis != 0)
		{
			return layoutHeight;
		}
		return layoutWidth;
	}

	public void SetSize(int axis, float value)
	{
		if (axis == 0)
		{
			layoutWidth = value;
		}
		else
		{
			layoutHeight = value;
		}
	}

	public SizeMode GetSizeMode(int axis)
	{
		if (axis != 0)
		{
			return layoutHeightMode;
		}
		return layoutWidthMode;
	}

	public void SetSizeMode(int axis, SizeMode value)
	{
		if (axis == 0)
		{
			layoutWidthMode = value;
		}
		else
		{
			layoutHeightMode = value;
		}
	}

	public void SetAlignMode(int axis, AlignMode value)
	{
		if (axis == 0)
		{
			switch (value)
			{
			case AlignMode.ParentMin:
				horizontalAlignMode = HorizontalAlignMode.ParentLeft;
				break;
			case AlignMode.ParentCentre:
				horizontalAlignMode = HorizontalAlignMode.ParentCentre;
				break;
			case AlignMode.ParentMax:
				horizontalAlignMode = HorizontalAlignMode.ParentRight;
				break;
			}
		}
		else
		{
			switch (value)
			{
			case AlignMode.ParentMin:
				verticalAlignMode = VerticalAlignMode.ParentBottom;
				break;
			case AlignMode.ParentCentre:
				verticalAlignMode = VerticalAlignMode.ParentCentre;
				break;
			case AlignMode.ParentMax:
				verticalAlignMode = VerticalAlignMode.ParentTop;
				break;
			}
		}
	}

	public void SetMargin(int axis, Margin margin, float value)
	{
		if (axis == 0)
		{
			if (margin == Margin.Lower)
			{
				layoutMarginLeft = value;
			}
			else
			{
				layoutMarginRight = value;
			}
		}
		else if (margin == Margin.Lower)
		{
			layoutMarginBottom = value;
		}
		else
		{
			layoutMarginTop = value;
		}
	}

	protected void Awake()
	{
		_touchable = GetComponent<UITouchable>();
		_animationComponent = GetComponent<Animation>();
		_hasAnimation = _animationComponent;
		_centre = base.transform.position;
		if (_layoutFinalised)
		{
			ApplyPositions(GetLayoutOpacity());
		}
	}

	public void Unfinalise()
	{
		_heightFinalised = false;
		_layoutFinalised = false;
		_widthFinalised = false;
		_xLayoutFinalised = false;
		_yLayoutFinalised = false;
		_centre = base.transform.position;
		if (_childLayouts == null)
		{
			return;
		}
		foreach (UILayout childLayout in _childLayouts)
		{
			childLayout.Unfinalise();
		}
	}

	private void SanityCheck()
	{
		if (Mathf.Abs(base.transform.position.x - _centre.x) > 0.0001f)
		{
			D.Error(base.name + " Mismatched X pos", this);
		}
		if (Mathf.Abs(base.transform.position.y - _centre.y) > 0.0001f)
		{
			D.Error(base.name + " Mismatched Y pos", this);
		}
	}

	public bool UpdateIsFinalised()
	{
		bool flag = _hasAnimation && _animationComponent.isPlaying;
		_layoutFinalised &= !flag && !_isAnimating;
		_isAnimating = flag;
		foreach (UILayout childLayout in _childLayouts)
		{
			_layoutFinalised &= childLayout.UpdateIsFinalised();
		}
		return _layoutFinalised;
	}

	public void Layout()
	{
		if (!_layoutFinalised && layoutHeightMode != 0 && layoutWidthMode != 0)
		{
			while (!_layoutFinalised)
			{
				_layoutFinalised = true;
				_layoutFinalised &= LayoutX();
				_layoutFinalised &= LayoutY();
			}
			ApplyPositions(opacity);
		}
	}

	private void ApplyPositions(float parentOpacity)
	{
		_centre = base.transform.position;
		if (_hasContent)
		{
			_content.opacity = opacity * parentOpacity;
		}
		if (!_xLayoutFinalised || !_yLayoutFinalised)
		{
			return;
		}
		if (null != _parentLayout)
		{
			switch (horizontalAlignMode)
			{
			case HorizontalAlignMode.ParentLeft:
				SetPosX(_parentLayout.left + _anchorParentOffset.x + layoutWidth / 2f);
				break;
			case HorizontalAlignMode.ParentCentre:
				SetPosX(_parentLayout.left + _anchorParentOffset.x);
				break;
			case HorizontalAlignMode.ParentRight:
				SetPosX(_parentLayout.right - _anchorParentOffset.x - layoutWidth / 2f);
				break;
			}
			switch (verticalAlignMode)
			{
			case VerticalAlignMode.ParentTop:
				SetPosY(_parentLayout.top - _anchorParentOffset.y - layoutHeight / 2f);
				break;
			case VerticalAlignMode.ParentCentre:
				SetPosY(_parentLayout.top - _anchorParentOffset.y);
				break;
			case VerticalAlignMode.ParentBottom:
				SetPosY(_parentLayout.bottom + _anchorParentOffset.y + layoutHeight / 2f);
				break;
			}
		}
		if (_hasContent)
		{
			if (layoutWidth != _content.size.x || layoutHeight != _content.size.y)
			{
				Vector2 size = new Vector2(layoutWidth, layoutHeight);
				_content.size = size;
			}
			_content.UpdateRect(this);
		}
		if ((bool)_touchable)
		{
			_touchable.UpdateTouchRect(this);
		}
		foreach (UILayout childLayout in _childLayouts)
		{
			childLayout.ApplyPositions(opacity * parentOpacity);
		}
	}

	private void SetPosX(float posX)
	{
		if (_centre.x != posX)
		{
			_centre.x = posX;
			_centre.z = base.transform.position.z;
			base.transform.position = _centre;
		}
	}

	private void SetPosY(float posY)
	{
		if (_centre.y != posY)
		{
			_centre.y = posY;
			_centre.z = base.transform.position.z;
			base.transform.position = _centre;
		}
	}

	public bool Layout(int axis)
	{
		if (axis == 0)
		{
			return LayoutX();
		}
		return LayoutY();
	}

	public virtual bool LayoutX()
	{
		switch (layoutWidthMode)
		{
		case SizeMode.MatchParent:
			if (_parentLayout.layoutWidthMode == SizeMode.WrapContent)
			{
				throw new UnityException("Can't have MatchParent inside WrapContent");
			}
			layoutWidth = _parentLayout.layoutWidth - (layoutMarginLeft + layoutMarginRight);
			_widthFinalised = _parentLayout._widthFinalised;
			break;
		case SizeMode.WrapContent:
			_widthFinalised = GetContentWidth(out layoutWidth);
			break;
		case SizeMode.Fixed:
			_widthFinalised = true;
			break;
		}
		AlignX();
		bool flag = _xLayoutFinalised && _widthFinalised;
		foreach (UILayout childLayout in _childLayouts)
		{
			flag &= childLayout.LayoutX();
		}
		_layoutFinalised = _xLayoutFinalised && _widthFinalised && _yLayoutFinalised && _heightFinalised;
		return flag;
	}

	public virtual bool LayoutY()
	{
		switch (layoutHeightMode)
		{
		case SizeMode.MatchParent:
			if (_parentLayout.layoutWidthMode == SizeMode.WrapContent)
			{
				throw new UnityException("Can't have MatchParent inside WrapContent");
			}
			layoutHeight = _parentLayout.layoutHeight - (layoutMarginTop + layoutMarginBottom);
			_heightFinalised = _parentLayout._heightFinalised;
			break;
		case SizeMode.WrapContent:
			_heightFinalised = GetContentHeight(out layoutHeight);
			break;
		case SizeMode.Fixed:
			_heightFinalised = true;
			break;
		}
		AlignY();
		bool flag = _yLayoutFinalised && _heightFinalised;
		foreach (UILayout childLayout in _childLayouts)
		{
			flag &= childLayout.LayoutY();
		}
		_layoutFinalised = _xLayoutFinalised && _widthFinalised && _yLayoutFinalised && _heightFinalised;
		return flag;
	}

	public void AlignX()
	{
		if (null == _parentLayout)
		{
			_xLayoutFinalised = true;
			return;
		}
		switch (horizontalAlignMode)
		{
		case HorizontalAlignMode.ParentLeft:
			_anchorParentOffset.x = layoutMarginLeft;
			_xLayoutFinalised = true;
			break;
		case HorizontalAlignMode.ParentCentre:
			if (_widthFinalised && _parentLayout._widthFinalised)
			{
				_anchorParentOffset.x = _parentLayout.layoutWidth / 2f;
				_xLayoutFinalised = true;
			}
			break;
		case HorizontalAlignMode.ParentRight:
			_anchorParentOffset.x = layoutMarginRight;
			_xLayoutFinalised = true;
			break;
		}
	}

	public void AlignY()
	{
		if (null == _parentLayout)
		{
			_yLayoutFinalised = true;
			return;
		}
		switch (verticalAlignMode)
		{
		case VerticalAlignMode.ParentTop:
			_anchorParentOffset.y = layoutMarginTop;
			_yLayoutFinalised = true;
			break;
		case VerticalAlignMode.ParentCentre:
			if (_heightFinalised && _parentLayout._heightFinalised)
			{
				_anchorParentOffset.y = _parentLayout.layoutHeight / 2f;
				_yLayoutFinalised = true;
			}
			break;
		case VerticalAlignMode.ParentBottom:
			_anchorParentOffset.y = layoutMarginBottom;
			_yLayoutFinalised = true;
			break;
		}
	}

	public virtual bool GetContentWidth(out float width)
	{
		float num = ((_content == null) ? 0f : _content.contentSize.x);
		foreach (UILayout childLayout in _childLayouts)
		{
			if (childLayout.isActiveAndEnabled)
			{
				if (!childLayout._widthFinalised)
				{
					childLayout.LayoutX();
				}
				if (!childLayout._widthFinalised)
				{
					width = layoutWidth;
					return false;
				}
				num = Mathf.Max(num, childLayout.layoutWidth + childLayout.layoutMarginLeft + childLayout.layoutMarginRight);
			}
		}
		width = num;
		return true;
	}

	public virtual bool GetContentHeight(out float height)
	{
		float num = ((_content == null) ? 0f : _content.contentSize.y);
		foreach (UILayout childLayout in _childLayouts)
		{
			if (childLayout.isActiveAndEnabled)
			{
				if (!childLayout._heightFinalised)
				{
					childLayout.LayoutY();
				}
				if (!childLayout._heightFinalised)
				{
					height = num;
					return false;
				}
				num = Mathf.Max(num, childLayout.layoutHeight + childLayout.layoutMarginTop + childLayout.layoutMarginBottom);
			}
		}
		height = num;
		return true;
	}

	public static int SortByZ(UILayout obj1, UILayout obj2)
	{
		if (obj1.transform.localPosition.z > obj2.transform.localPosition.z)
		{
			return 1;
		}
		if (obj1.transform.localPosition.z == obj2.transform.localPosition.z)
		{
			return 0;
		}
		return -1;
	}

	protected void FindChildren()
	{
		_content = null;
		_hasContent = false;
		_childLayouts.Clear();
		UILayout uILayout = _parentLayout;
		while ((bool)uILayout && (bool)uILayout)
		{
			_rootLayout = uILayout as UICamera;
			if ((bool)_rootLayout)
			{
				break;
			}
			uILayout = uILayout._parentLayout;
		}
		for (int i = 0; i < base.transform.childCount; i++)
		{
			UILayoutContent component = base.transform.GetChild(i).GetComponent<UILayoutContent>();
			if (component != null && _hasContent && _content != component)
			{
				D.Error("UILayout " + base.name + " has more than one content object (" + component.name + " and " + _content.name + ")", this);
			}
			if (component != null)
			{
				_content = component;
				_hasContent = true;
			}
			UILayout component2 = base.transform.GetChild(i).GetComponent<UILayout>();
			if (component2 != null)
			{
				component2._parentLayout = this;
				_childLayouts.Add(component2);
				component2.FindChildren();
			}
		}
		UpdateChildZOrder();
	}

	private void OnDrawGizmos()
	{
		if (!Application.isPlaying || (base.gameObject.activeInHierarchy && GetLayoutOpacity() > 0f))
		{
			DrawOutline(Color.grey, withCross: false);
		}
	}

	private void OnDrawGizmosSelected()
	{
	}

	private void DrawOutline(Color color, bool withCross)
	{
		Gizmos.color = color;
		Vector3 vector = base.transform.position + new Vector3((0f - layoutWidth) / 2f, layoutHeight / 2f, 0f);
		Vector3 to = base.transform.position + new Vector3(layoutWidth / 2f, layoutHeight / 2f, 0f);
		Vector3 vector2 = base.transform.position + new Vector3((0f - layoutWidth) / 2f, (0f - layoutHeight) / 2f, 0f);
		Vector3 vector3 = base.transform.position + new Vector3(layoutWidth / 2f, (0f - layoutHeight) / 2f, 0f);
		Gizmos.DrawLine(vector, to);
		Gizmos.DrawLine(vector3, to);
		Gizmos.DrawLine(vector3, vector2);
		Gizmos.DrawLine(vector2, vector);
		if (withCross)
		{
			Gizmos.DrawLine(vector, vector3);
			Gizmos.DrawLine(vector2, to);
		}
	}

	public UITouchable FindTouchHandler(Vector2 dpPos, GameTouch touch)
	{
		UITouchable touchable = _touchable;
		if ((bool)touchable && touchable.AcceptTouch(dpPos, touch))
		{
			return touchable;
		}
		if (_childZOrder != null)
		{
			float num = float.MaxValue;
			UITouchable uITouchable = null;
			foreach (UILayout item in _childZOrder)
			{
				if (!item.gameObject.activeSelf || item.opacity <= 0f)
				{
					continue;
				}
				UITouchable uITouchable2 = item.FindTouchHandler(dpPos, touch);
				if ((bool)uITouchable2)
				{
					float z = uITouchable2.transform.position.z;
					if (z < num)
					{
						uITouchable = uITouchable2;
						num = z;
					}
					else if (z == num && PosMoreInAThanB(dpPos, uITouchable2.touchRect, uITouchable.touchRect))
					{
						uITouchable = uITouchable2;
						num = z;
					}
				}
			}
			if ((bool)uITouchable)
			{
				return uITouchable;
			}
		}
		return null;
	}

	private bool PosMoreInAThanB(Vector3 pos, Rect rectA, Rect rectB)
	{
		int num = 1;
		Vector3 vector = rectB.center - rectA.center;
		bool num2 = rectA.width > rectA.height;
		bool flag = rectB.width > rectB.height;
		bool flag2 = !(rectB.xMin > rectA.xMax) && !(rectB.xMax < rectA.xMin);
		num = (((num2 || flag) && flag2) ? 1 : ((Mathf.Abs(vector.y) > Mathf.Abs(vector.x)) ? 1 : 0));
		float num3 = Mathf.Abs(pos[num] - rectA.center[num]) - 0.5f * rectA.size[num];
		float num4 = Mathf.Abs(pos[num] - rectB.center[num]) - 0.5f * rectB.size[num];
		return num3 < num4;
	}

	public float GetLayoutOpacity()
	{
		if (!_parentLayout)
		{
			return opacity;
		}
		return opacity * _parentLayout.GetLayoutOpacity();
	}

	public UILayout RemoveChild(int idx)
	{
		UILayout uILayout = _childLayouts[idx];
		_childLayouts[idx].transform.parent = null;
		uILayout._parentLayout = null;
		_childLayouts.RemoveAt(idx);
		UpdateChildZOrder();
		return uILayout;
	}

	public UILayout GetChild(int idx)
	{
		return _childLayouts[idx];
	}

	public void AddChild(UILayout layout)
	{
		layout._parentLayout = this;
		layout._rootLayout = _rootLayout;
		layout.transform.parent = base.transform;
		layout.transform.localPosition = Vector3.zero;
		_childLayouts.Add(layout);
		UpdateChildZOrder();
	}

	private void UpdateChildZOrder()
	{
		_childZOrder = new List<UILayout>(_childLayouts);
		_childZOrder.Sort(SortByZ);
	}

	public void OnSpawnedAtRuntime()
	{
		FindChildren();
	}

	public void OnLanguageChanged()
	{
		if (_hasContent)
		{
			_content.OnLanguageChanged();
		}
		foreach (UILayout childLayout in _childLayouts)
		{
			childLayout.OnLanguageChanged();
		}
	}
}
