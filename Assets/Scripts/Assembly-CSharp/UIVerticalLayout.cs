public class UIVerticalLayout : UIAxialLayout
{
	public override bool LayoutY()
	{
		LayoutAlongAxis(1, AlignMode.ParentMax);
		return base.LayoutY();
	}
}
