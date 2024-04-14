using System.Collections.Generic;
using UnityEngine;

public class NavAStarNode
{
	public class Neighbour
	{
		public NavAStarNode node;

		public NavBrushLink link;
	}

	public NavBrushComponent brush;

	public Vector3 panSpacePos;

	public float f;

	public float g;

	public float h;

	public NavAStarNode searchParent;

	public List<Neighbour> neighbours = new List<Neighbour>();

	public void Init(NavBrushComponent brush)
	{
		this.brush = brush;
		panSpacePos = brush.panSpaceBoundingRect.center;
		neighbours.Clear();
	}
}
