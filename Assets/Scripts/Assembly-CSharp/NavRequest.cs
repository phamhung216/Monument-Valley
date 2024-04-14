using System.Collections.Generic;

public class NavRequest
{
	public enum RequestStatus
	{
		Pending = 0,
		Processing = 1,
		Complete = 2
	}

	public RequestStatus status;

	public NavBrushComponent startBrush;

	public NavBrushComponent endBrush;

	public NavAccessFlags accessMask;

	public List<NavBrushComponent> route = new List<NavBrushComponent>();

	public void Init(NavBrushComponent start, NavBrushComponent end, NavAccessFlags accessMask)
	{
		startBrush = start;
		endBrush = end;
		status = RequestStatus.Pending;
		this.accessMask = accessMask;
		route.Clear();
	}

	public override string ToString()
	{
		string text = "NavRequest {";
		for (int i = 0; i < route.Count; i++)
		{
			text += route[i].name;
			if (i < route.Count - 1)
			{
				text += ", ";
			}
		}
		return text + "}";
	}
}
