using System.Collections.Generic;
using UnityEngine;

public class NavBrushGraph
{
	private NavBrushLink[] _links;

	private int _staticLinkCount;

	private int _dynamicLinkCount;

	private NavBrushComponent[] _allBrushes;

	private List<SortedNavBrush> _sortedBrushes = new List<SortedNavBrush>();

	private List<SortedNavBrush> _sortedDynamicBrushes = new List<SortedNavBrush>();

	private NavBlockerPlane[] _blockers;

	private NavBlockerVolume[] _volumeBlockers;

	private bool _hasGeneratedStaticLinks;

	private int _dynamicTestCount;

	private int _staticTestCount;

	public NavBrushComponent[] allBrushes => _allBrushes;

	public void Init()
	{
		_allBrushes = Object.FindObjectsOfType(typeof(NavBrushComponent)) as NavBrushComponent[];
		for (int i = 0; i < _allBrushes.Length; i++)
		{
			_allBrushes[i].Init();
		}
		_blockers = Object.FindObjectsOfType(typeof(NavBlockerPlane)) as NavBlockerPlane[];
		_volumeBlockers = Object.FindObjectsOfType(typeof(NavBlockerVolume)) as NavBlockerVolume[];
		for (int j = 0; j < _volumeBlockers.Length; j++)
		{
			DebugUtils.DebugAssert(j < 32);
			int blockingMask = 1 << j;
			_volumeBlockers[j].blockingMask = blockingMask;
		}
		int num = 8;
		int num2 = _allBrushes.Length * num;
		_links = new NavBrushLink[num2];
		for (int k = 0; k < _links.Length; k++)
		{
			_links[k] = new NavBrushLink();
		}
		NavBrushComponent[] array = _allBrushes;
		foreach (NavBrushComponent obj in array)
		{
			SortedNavBrush item = new SortedNavBrush(obj);
			_sortedBrushes.Add(item);
			if (!obj.isStatic)
			{
				_sortedDynamicBrushes.Add(item);
			}
		}
		_hasGeneratedStaticLinks = false;
		SortBrushes();
	}

	private void ClearLinks()
	{
		for (int i = _staticLinkCount; i < _staticLinkCount + _dynamicLinkCount; i++)
		{
			NavBrushLink navBrushLink = _links[i];
			navBrushLink.boundaries[0].links.Remove(navBrushLink);
			navBrushLink.boundaries[1].links.Remove(navBrushLink);
			navBrushLink.enabled = false;
			navBrushLink.blockingMask = 0;
			navBrushLink.flags = (NavAccessFlags)0;
		}
		_dynamicLinkCount = 0;
	}

	public void UpdateLinks()
	{
		_dynamicTestCount = 0;
		_staticTestCount = 0;
		ClearLinks();
		SortBrushes();
		int count = _sortedDynamicBrushes.Count;
		if (!_hasGeneratedStaticLinks)
		{
			for (int i = 0; i < _sortedBrushes.Count; i++)
			{
				if (_sortedBrushes[i].brush.isStatic)
				{
					GenerateBrushLinks(i, count, doStaticLinks: true);
				}
				else
				{
					_sortedBrushes[i].visited = true;
				}
			}
		}
		else
		{
			int j = 0;
			for (int k = 0; k < _sortedBrushes.Count; k++)
			{
				for (; j < count && _sortedDynamicBrushes[j].visited; j++)
				{
				}
				GenerateBrushLinks(k, j, doStaticLinks: false);
			}
		}
		_hasGeneratedStaticLinks = true;
		UpdateBlockerVolumes();
	}

	private void UpdateBlockerVolumes()
	{
		NavBlockerVolume[] volumeBlockers = _volumeBlockers;
		for (int i = 0; i < volumeBlockers.Length; i++)
		{
			volumeBlockers[i].ApplyCollidingBrushesNav();
		}
	}

	private void SortBrushes()
	{
		int count = _sortedBrushes.Count;
		for (int i = 0; i < count; i++)
		{
			SortedNavBrush sortedNavBrush = _sortedBrushes[i];
			sortedNavBrush.brush.CalculateNavState();
			sortedNavBrush.depth = sortedNavBrush.brush.panSpacePos.z;
			sortedNavBrush.visited = false;
		}
		_sortedBrushes.Sort(CompareBrushes);
		_sortedDynamicBrushes.Sort(CompareBrushes);
	}

	private static int CompareBrushes(SortedNavBrush brushA, SortedNavBrush brushB)
	{
		return (int)(10f * (brushA.depth - brushB.depth));
	}

	private void GenerateBrushLinks(int sortedBrushIdx, int sortedDynamicBrushIdx, bool doStaticLinks)
	{
		NavBrushComponent brush = _sortedBrushes[sortedBrushIdx].brush;
		_sortedBrushes[sortedBrushIdx].visited = true;
		if (!brush.gameObject.activeSelf)
		{
			return;
		}
		if (doStaticLinks)
		{
			if (brush.isStatic)
			{
				int count = _sortedBrushes.Count;
				for (int i = sortedBrushIdx + 1; i < count; i++)
				{
					NavBrushComponent brush2 = _sortedBrushes[i].brush;
					if (brush2.isStatic)
					{
						_staticTestCount++;
						GenerateConnection(brush, brush2);
					}
				}
			}
		}
		else if (brush.isStatic)
		{
			int count2 = _sortedDynamicBrushes.Count;
			for (int j = sortedDynamicBrushIdx; j < count2; j++)
			{
				NavBrushComponent brush3 = _sortedDynamicBrushes[j].brush;
				_staticTestCount++;
				GenerateConnection(brush, brush3);
			}
		}
		else
		{
			int count3 = _sortedBrushes.Count;
			for (int k = sortedBrushIdx + 1; k < count3; k++)
			{
				NavBrushComponent brush4 = _sortedBrushes[k].brush;
				_dynamicTestCount++;
				GenerateConnection(brush, brush4);
			}
		}
		for (int l = 0; l < brush.boundaries.Length; l++)
		{
			NavBoundaryComponent navBoundaryComponent = brush.boundaries[l];
			if (navBoundaryComponent.permanentConnection != null && (!brush.isStatic || !navBoundaryComponent.permanentConnection.parentBrush.isStatic || doStaticLinks))
			{
				MakeLink(navBoundaryComponent, navBoundaryComponent.permanentConnection);
			}
		}
	}

	private void GenerateConnection(NavBrushComponent topBrush, NavBrushComponent otherBrush)
	{
		if (!otherBrush.gameObject.activeSelf || topBrush == otherBrush || (topBrush.chunkIndex != otherBrush.chunkIndex && (topBrush.isStatic || otherBrush.isStatic)) || !RectUtil.Intersects(topBrush.panSpaceBoundingRect, otherBrush.panSpaceBoundingRect))
		{
			return;
		}
		NavBlockerPlane[] blockers = _blockers;
		for (int i = 0; i < blockers.Length; i++)
		{
			if (blockers[i].DoesBlockConnection(topBrush, otherBrush))
			{
				return;
			}
		}
		NavBoundaryComponent[] boundaries = topBrush.boundaries;
		foreach (NavBoundaryComponent navBoundaryComponent in boundaries)
		{
			if (!(navBoundaryComponent.permanentConnection == null))
			{
				continue;
			}
			NavBoundaryComponent[] boundaries2 = otherBrush.boundaries;
			foreach (NavBoundaryComponent navBoundaryComponent2 in boundaries2)
			{
				if (CanConnect(navBoundaryComponent, navBoundaryComponent2, logFailures: false))
				{
					UpdateCurrentLinks(navBoundaryComponent, navBoundaryComponent2);
					MakeLink(navBoundaryComponent, navBoundaryComponent2);
					return;
				}
			}
		}
	}

	private void UpdateCurrentLinks(NavBoundaryComponent topBoundary, NavBoundaryComponent otherBoundary)
	{
		if (topBoundary.parentBrush.type != NavBrushComponent.Type.Stairs || !(Vector3.Dot(topBoundary.transform.localPosition, topBoundary.parentBrush.transform.up) < 0f) || !(Vector3.Dot(topBoundary.parentBrush.transform.TransformDirection(topBoundary.parentBrush.normal), Camera.main.transform.forward) > -0.3f))
		{
			return;
		}
		int num = 0;
		while (num < otherBoundary.links.Count)
		{
			if (!otherBoundary.links[num].isStatic && otherBoundary.links[num].GetOtherBoundary(otherBoundary).parentBrush.type != NavBrushComponent.Type.Stairs)
			{
				RemoveLink(otherBoundary.links[num]);
			}
			else
			{
				num++;
			}
		}
	}

	public bool CanConnect(NavBoundaryComponent topBoundary, NavBoundaryComponent otherBoundary, bool logFailures)
	{
		if (otherBoundary.permanentConnection != null)
		{
			return false;
		}
		if (topBoundary.zone != otherBoundary.zone)
		{
			return false;
		}
		if (((Vector2)topBoundary.panSpacePos - (Vector2)otherBoundary.panSpacePos).sqrMagnitude > 0.1f)
		{
			return false;
		}
		if (topBoundary.hasFrontFace && topBoundary.panSpacePos.z < otherBoundary.panSpacePos.z - 0.5f)
		{
			return false;
		}
		Transform transform = topBoundary.parentBrush.transform;
		Transform transform2 = otherBoundary.parentBrush.transform;
		if (Vector3.Dot(transform.up, transform2.up) < Mathf.Cos(1.9198622f))
		{
			return false;
		}
		float num = Vector2.Dot(topBoundary.flattenedPanSpaceDir, otherBoundary.flattenedPanSpaceDir);
		if (num > Mathf.Cos(0.17453292f))
		{
			return false;
		}
		Transform transform3 = topBoundary.transform;
		Transform transform4 = otherBoundary.transform;
		Vector3 lhs = transform3.position - transform4.position + transform2.position - transform.position;
		Vector3 rhs = transform.TransformDirection(topBoundary.parentBrush.normal);
		Vector3 rhs2 = transform2.TransformDirection(otherBoundary.parentBrush.normal);
		if (num > Mathf.Cos(1.3962634f))
		{
			if (Mathf.Sign(Vector3.Dot(lhs, rhs)) == Mathf.Sign(Vector3.Dot(lhs, rhs2)))
			{
				return false;
			}
		}
		else if (num > Mathf.Cos(2.7925267f) && Mathf.Sign(Vector3.Dot(lhs, rhs)) == Mathf.Sign(Vector3.Dot(lhs, rhs2)))
		{
			return false;
		}
		bool flag = true;
		for (int i = 0; i < topBoundary.links.Count; i++)
		{
			NavBoundaryComponent otherBoundary2 = topBoundary.links[i].GetOtherBoundary(topBoundary);
			if (otherBoundary2.panSpacePos.z < otherBoundary.panSpacePos.z - 0.5f && Vector3.Dot(otherBoundary2.parentBrush.transform.TransformDirection(otherBoundary2.parentBrush.normal), Camera.main.transform.forward) < 0.1f)
			{
				flag = false;
				break;
			}
		}
		if (flag)
		{
			return true;
		}
		return false;
	}

	private NavBrushLink MakeLink(NavBoundaryComponent boundaryA, NavBoundaryComponent boundaryB)
	{
		DebugUtils.DebugAssert(_links.Length > _staticLinkCount + _dynamicLinkCount);
		NavBrushLink navBrushLink = null;
		if (boundaryA.parentBrush.isStatic && boundaryB.parentBrush.isStatic)
		{
			DebugUtils.DebugAssert(_dynamicLinkCount == 0);
			if (_dynamicLinkCount > 0)
			{
				D.Error("Trying to make static link for " + boundaryA.name + " and " + boundaryB.name, boundaryA);
			}
			navBrushLink = _links[_staticLinkCount];
			_staticLinkCount++;
			navBrushLink.isStatic = true;
		}
		else
		{
			navBrushLink = _links[_staticLinkCount + _dynamicLinkCount];
			_dynamicLinkCount++;
			navBrushLink.isStatic = false;
		}
		navBrushLink.enabled = true;
		navBrushLink.boundaries[0] = boundaryA;
		navBrushLink.boundaries[1] = boundaryB;
		navBrushLink.flags = (NavAccessFlags)0;
		boundaryA.links.Add(navBrushLink);
		boundaryB.links.Add(navBrushLink);
		DebugUtils.DebugAssert(boundaryA.parentBrush, boundaryA.gameObject);
		DebugUtils.DebugAssert(boundaryB.parentBrush, boundaryB.gameObject);
		navBrushLink.flags = GetLinkAccess(boundaryA, boundaryB);
		return navBrushLink;
	}

	private NavAccessFlags GetLinkAccess(NavBoundaryComponent boundaryA, NavBoundaryComponent boundaryB)
	{
		if (Vector3.Dot(boundaryA.parentBrush.transform.up, boundaryB.parentBrush.transform.up) < Mathf.Cos(0.87266463f))
		{
			return NavAccessFlags.Crow | NavAccessFlags.Autowalker | NavAccessFlags.NotBlocked;
		}
		if (boundaryA.parentBrush.type == NavBrushComponent.Type.Hole || boundaryB.parentBrush.type == NavBrushComponent.Type.Hole)
		{
			return NavAccessFlags.Totem | NavAccessFlags.NotBlocked;
		}
		if (boundaryA.parentBrush.type == NavBrushComponent.Type.Stairs || boundaryB.parentBrush.type == NavBrushComponent.Type.Stairs)
		{
			return NavAccessFlags.Player | NavAccessFlags.NotBlocked;
		}
		if (boundaryA.parentBrush.type == NavBrushComponent.Type.Ladder || boundaryB.parentBrush.type == NavBrushComponent.Type.Ladder)
		{
			return NavAccessFlags.Player | NavAccessFlags.NotBlocked;
		}
		return NavAccessFlags.Player | NavAccessFlags.Crow | NavAccessFlags.Totem | NavAccessFlags.Autowalker | NavAccessFlags.NotBlocked | NavAccessFlags.CrowGrounded;
	}

	public void OnDrawGizmos()
	{
		Gizmos.color = Color.green;
		for (int i = 0; i < _staticLinkCount + _dynamicLinkCount; i++)
		{
			if (_links[i].enabled)
			{
				Gizmos.color = (((_links[i].flags & NavAccessFlags.Player) == NavAccessFlags.Player) ? Color.green : Color.yellow);
				Gizmos.DrawLine(_links[i].boundaries[0].GetDebugPos(), _links[i].boundaries[1].GetDebugPos());
			}
		}
	}

	public List<NavBrushComponent> FindBrushesInRegion(Transform position, Vector3 size)
	{
		float z = GameScene.WorldToPanPoint(position.position).z;
		float magnitude = size.magnitude;
		_ = size.sqrMagnitude;
		List<NavBrushComponent> list = new List<NavBrushComponent>();
		if (_sortedBrushes != null)
		{
			int i = 0;
			int count = _sortedBrushes.Count;
			int num = count;
			int num2 = -1;
			while (i >= 0 && i < count)
			{
				float num3 = _sortedBrushes[i].depth - z;
				if (Mathf.Abs(num3) < magnitude)
				{
					num2 = i;
					break;
				}
				if (num3 < 0f)
				{
					num >>= 1;
					if (num == 0)
					{
						break;
					}
					i += num;
				}
				else
				{
					num >>= 1;
					if (num == 0)
					{
						break;
					}
					i -= num;
				}
			}
			if (num2 >= 0)
			{
				while (i - 1 >= 0 && Mathf.Abs(_sortedBrushes[i - 1].depth - z) < magnitude)
				{
					i--;
				}
				for (; i < count && Mathf.Abs(_sortedBrushes[i].depth - z) < magnitude; i++)
				{
					Vector3 vector = position.InverseTransformPoint(_sortedBrushes[i].brush.transform.position);
					if (Mathf.Abs(vector.x) <= 0.5f * size.x && Mathf.Abs(vector.y) <= 0.5f * size.y && Mathf.Abs(vector.z) <= 0.5f * size.z)
					{
						list.Add(_sortedBrushes[i].brush);
					}
				}
			}
		}
		return list;
	}

	public NavBrushComponent FindTopBrushBelowPanPoint(Vector3 point, bool touchableOnly, float extra)
	{
		int num = LayerMask.NameToLayer("Touchable");
		float timeout = 1f;
		Vector3 vector = GameScene.WorldToPanPoint(Camera.main.transform.position);
		Ray ray = new Ray(GameScene.PanToWorldPoint(new Vector3(point.x, point.y, vector.z)), Camera.main.transform.forward);
		RenderDebug.DrawLine(ray.origin, ray.origin + ray.direction * 100f, Color.white, timeout);
		if (_sortedBrushes == null || _sortedBrushes.Count == 0)
		{
			D.Error("No brushes");
		}
		if (_sortedBrushes == null)
		{
			return null;
		}
		NavBrushComponent navBrushComponent = null;
		NavBrushComponent navBrushComponent2 = null;
		foreach (SortedNavBrush sortedBrush in _sortedBrushes)
		{
			NavBrushComponent brush = sortedBrush.brush;
			Rect rect = new Rect(brush.panSpaceBoundingRect.xMin - extra, brush.panSpaceBoundingRect.yMin - extra, brush.panSpaceBoundingRect.width + 2f * extra, brush.panSpaceBoundingRect.height + 2f * extra);
			if (!(sortedBrush.depth >= point.z) || !rect.Contains(point) || !brush.gameObject.activeInHierarchy)
			{
				continue;
			}
			if (touchableOnly)
			{
				if (!brush.touchable)
				{
					RenderDebug.DrawSphere(brush.transform.position, Color.red, 0.2f, timeout);
					continue;
				}
				bool flag = false;
				if (!brush.touchableWhenFacingAway && Vector3.Dot(brush.transform.TransformDirection(brush.normal), ray.direction) > 0f)
				{
					continue;
				}
				float num2 = Vector3.Dot(brush.transform.position - ray.origin, ray.direction);
				int layerMask = 1 << num;
				RaycastHit[] array = Physics.RaycastAll(ray, float.PositiveInfinity, layerMask);
				for (int i = 0; i < array.Length; i++)
				{
					RaycastHit raycastHit = array[i];
					float num3 = 1f;
					NavBrushTouchProxy component = raycastHit.collider.transform.gameObject.GetComponent<NavBrushTouchProxy>();
					if ((bool)component)
					{
						if (!component.CanAcceptRay(ray.direction))
						{
							continue;
						}
						navBrushComponent2 = component.navBrush;
					}
					if (raycastHit.collider.transform.gameObject.GetComponent<GameTouchable>() == null && raycastHit.distance + num3 < num2)
					{
						RenderDebug.DrawSphere(brush.transform.position, Color.red, 0.2f, timeout);
						RenderDebug.DrawSphere(ray.GetPoint(raycastHit.distance), Color.red, 0.1f, timeout);
						RenderDebug.Add(new RenderDebugLine(ray.origin, ray.GetPoint(raycastHit.distance), Color.red), timeout);
						RenderDebug.Add(new RenderDebugLine(raycastHit.collider.gameObject.transform.position, ray.GetPoint(raycastHit.distance), Color.red), timeout);
						flag = true;
						break;
					}
					RenderDebug.DrawSphere(ray.GetPoint(raycastHit.distance), Color.green, 0.1f, timeout);
				}
				if (flag)
				{
					continue;
				}
			}
			float num4 = Vector2.Distance(brush.panSpacePos, point);
			float num5 = (navBrushComponent ? Vector2.Distance(navBrushComponent.panSpacePos, point) : 10000f);
			float num6 = 0.1f;
			bool flag2 = navBrushComponent == null;
			flag2 |= Mathf.Abs(num5 - num4) < num6 && brush.panSpacePos.z < navBrushComponent.panSpacePos.z;
			flag2 |= Mathf.Abs(num5 - num4) > num6 && num4 < num5;
			if ((bool)navBrushComponent)
			{
				if (!navBrushComponent.isAccessibleByPlayer && brush.isAccessibleByPlayer)
				{
					flag2 = true;
				}
				else if (navBrushComponent.isAccessibleByPlayer && !brush.isAccessibleByPlayer)
				{
					flag2 = false;
				}
			}
			if (flag2)
			{
				navBrushComponent = brush;
			}
		}
		if ((bool)navBrushComponent2 && navBrushComponent2.touchable && navBrushComponent2.gameObject.activeInHierarchy)
		{
			float num7 = Vector2.Distance(navBrushComponent2.panSpacePos, point);
			float num8 = (navBrushComponent ? Vector2.Distance(navBrushComponent.panSpacePos, point) : 10000f);
			float num9 = 0.3f;
			if (num7 < num8 + num9)
			{
				navBrushComponent = navBrushComponent2;
			}
		}
		if ((bool)navBrushComponent)
		{
			return navBrushComponent;
		}
		return null;
	}

	public void DebugLink(NavBoundaryComponent boundaryA, NavBoundaryComponent boundaryB)
	{
		foreach (NavBrushLink link in boundaryA.links)
		{
			if (link.GetOtherBoundary(boundaryA) == boundaryB)
			{
				if (!link.enabled)
				{
					return;
				}
				break;
			}
		}
		NavBoundaryComponent navBoundaryComponent = ((boundaryA.panSpacePos.z < boundaryB.panSpacePos.z) ? boundaryA : boundaryB);
		NavBoundaryComponent navBoundaryComponent2 = ((navBoundaryComponent == boundaryA) ? boundaryB : boundaryA);
		Vector3 vector = Vector3.Cross(navBoundaryComponent.GetDebugPos() - navBoundaryComponent2.GetDebugPos(), Camera.main.transform.forward);
		vector.Normalize();
		Vector3 to = 0.5f * (navBoundaryComponent.GetDebugPos() + navBoundaryComponent2.GetDebugPos()) + vector * 3f;
		bool flag = CanConnect(navBoundaryComponent, navBoundaryComponent2, logFailures: true);
		GetLinkAccess(navBoundaryComponent, navBoundaryComponent2);
		RenderDebug.DrawLine(navBoundaryComponent.GetDebugPos(), to, flag ? Color.green : Color.red, 10f);
		RenderDebug.DrawLine(navBoundaryComponent2.GetDebugPos(), to, flag ? Color.green : Color.red, 10f);
	}

	private void RemoveLink(NavBrushLink link)
	{
		link.enabled = false;
		DebugUtils.DebugAssert(!link.isStatic, link.boundaries[0].gameObject);
		NavBoundaryComponent[] boundaries = link.boundaries;
		for (int i = 0; i < boundaries.Length; i++)
		{
			boundaries[i].links.Remove(link);
		}
	}
}
