public class UIHorizontalLayout : UIAxialLayout
{
	public override bool LayoutX()
	{
		LayoutAlongAxis(0, AlignMode.ParentMin);
		return base.LayoutX();
	}
}
