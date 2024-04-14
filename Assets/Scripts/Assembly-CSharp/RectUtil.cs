using UnityEngine;

public class RectUtil
{
	public static bool Intersects(Rect a, Rect b)
	{
		if (!(a.xMin > b.xMax) && !(a.xMax < b.xMin) && !(a.yMin > b.yMax))
		{
			return !(a.yMax < b.yMin);
		}
		return false;
	}
}
