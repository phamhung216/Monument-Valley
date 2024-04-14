public class SortedNavBrush
{
	public float depth;

	public NavBrushComponent brush;

	public bool visited;

	public SortedNavBrush(NavBrushComponent brush)
	{
		this.brush = brush;
	}
}
