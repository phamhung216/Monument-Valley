public abstract class UIAxialLayout : UILayout
{
	public void LayoutAlongAxis(int axis, AlignMode childAlignment)
	{
		float num = 0f;
		int num2 = 0;
		int num3 = 0;
		foreach (UILayout childLayout in _childLayouts)
		{
			if (childLayout.gameObject.activeSelf)
			{
				num2++;
				SizeMode sizeMode = childLayout.GetSizeMode(axis);
				if (sizeMode == SizeMode.WrapContent)
				{
					childLayout.Layout(axis);
				}
				if (sizeMode != 0)
				{
					num += childLayout.GetSize(axis);
				}
				else
				{
					num3++;
				}
			}
		}
		Margin margin = Margin.Lower;
		Margin margin2 = Margin.Upper;
		if (childAlignment == AlignMode.ParentMax)
		{
			margin = Margin.Upper;
			margin2 = Margin.Lower;
		}
		if (GetSizeMode(axis) == SizeMode.WrapContent)
		{
			SetSize(axis, num);
		}
		float size = GetSize(axis);
		float num4 = size - num;
		float num5 = 0f;
		int num6 = 0;
		foreach (UILayout childLayout2 in _childLayouts)
		{
			if (childLayout2.gameObject.activeSelf)
			{
				childLayout2.SetAlignMode(axis, childAlignment);
				if (childLayout2.GetSizeMode(axis) == SizeMode.MatchParent)
				{
					childLayout2.SetSize(axis, num4 / (float)num3);
				}
				float size2 = childLayout2.GetSize(axis);
				switch (childLayout2.GetSizeMode(axis))
				{
				case SizeMode.WrapContent:
					childLayout2.Layout(axis);
					size2 = childLayout2.GetSize(axis);
					childLayout2.SetMargin(axis, margin, num5);
					break;
				case SizeMode.Fixed:
					childLayout2.SetMargin(axis, margin, num5);
					break;
				case SizeMode.MatchParent:
					childLayout2.SetMargin(axis, margin, num5);
					childLayout2.SetMargin(axis, margin2, size - num5 - size2);
					break;
				}
				num5 += size2;
				num6++;
			}
		}
	}
}
