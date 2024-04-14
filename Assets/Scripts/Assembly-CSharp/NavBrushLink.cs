using System;

[Serializable]
public class NavBrushLink
{
	public NavBoundaryComponent[] boundaries = new NavBoundaryComponent[2];

	[BitMaskField(typeof(NavAccessFlags))]
	public NavAccessFlags flags;

	public bool isStatic;

	public bool enabled = true;

	public int blockingMask;

	public NavBoundaryComponent GetOtherBoundary(NavBoundaryComponent boundary)
	{
		if (boundaries[0] == boundary)
		{
			return boundaries[1];
		}
		if (boundaries[1] == boundary)
		{
			return boundaries[0];
		}
		return null;
	}

	public NavBoundaryComponent GetOtherBoundary(NavBrushComponent boundaryParent)
	{
		if (boundaries[0].parentBrush == boundaryParent)
		{
			return boundaries[1];
		}
		if (boundaries[1].parentBrush == boundaryParent)
		{
			return boundaries[0];
		}
		return null;
	}
}
