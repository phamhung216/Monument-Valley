using System.Collections.Generic;
using UnityEngine;

public class NavBoundaryComponent : MonoBehaviour
{
	public enum FrontFaceMode
	{
		Never = 0,
		WhenFront = 1,
		Always = 2
	}

	public NavBoundaryComponent permanentConnection;

	public int zone;

	public Vector3 panSpacePos;

	public Vector2 flattenedPanSpaceDir;

	public bool hasFrontFace;

	public FrontFaceMode frontFaceEligibility = FrontFaceMode.WhenFront;

	public List<NavBrushLink> links = new List<NavBrushLink>();

	private NavBrushComponent _parentBrush;

	public NavBrushComponent parentBrush => _parentBrush;

	public void Init()
	{
		DebugUtils.DebugAssert(links.Count == 0, base.gameObject);
		_parentBrush = base.transform.parent.GetComponent<NavBrushComponent>();
		DebugUtils.DebugAssert(_parentBrush, base.gameObject);
	}

	public void ConnectTo(NavBoundaryComponent other, bool permanent)
	{
		if (permanent)
		{
			permanentConnection = other;
			if ((bool)other)
			{
				other.permanentConnection = this;
			}
		}
	}

	public Vector3 GetDebugPos()
	{
		return base.transform.parent.TransformPoint(0.8f * base.transform.localPosition);
	}

	public NavBrushLink GetLinkTo(NavBoundaryComponent otherBoundary)
	{
		foreach (NavBrushLink link in links)
		{
			if (link.GetOtherBoundary(this) == otherBoundary)
			{
				return link;
			}
		}
		return null;
	}

	private void OnDrawGizmosSelected()
	{
	}

	private void OnDrawGizmos()
	{
		if (!parentBrush)
		{
			return;
		}
		Vector3 vector = base.transform.parent.position - base.transform.position;
		vector.Normalize();
		float num = 0.1f;
		float a = (parentBrush.gameObject.activeSelf ? 1f : 0.3f);
		Color color = ((zone >= 0) ? NavBrushComponent.colors[zone] : Color.grey);
		color.a = a;
		Gizmos.color = color;
		Gizmos.DrawCube(base.transform.position + 0.5f * num * vector, Vector3.one * num);
		if (links == null || links.Count <= 0)
		{
			return;
		}
		Gizmos.color = Color.yellow;
		foreach (NavBrushLink link in links)
		{
			if ((link.flags & NavAccessFlags.Player) == NavAccessFlags.Player)
			{
				Gizmos.color = Color.green;
			}
			if ((link.flags & NavAccessFlags.NotBlocked) == 0)
			{
				Gizmos.color = Color.red;
			}
		}
		Gizmos.DrawCube(base.transform.position, Vector3.one * num * 1.1f);
	}
}
