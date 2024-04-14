using System;
using System.Collections.Generic;
using UnityEngine;

public class NavAStar
{
	public delegate void FloodFillVisitor(NavBrushComponent brush);

	private List<NavAStarNode.Neighbour> _neighbourPool;

	private int _neighbourPoolUseCount;

	private NavAStarNode[] _allNodes;

	private List<NavAStarNode> _openList;

	private List<NavAStarNode> _closedList;

	private List<NavAStarNode> _visitedList;

	private Dictionary<NavBrushComponent, NavAStarNode> _brushMap;

	private bool _isSearching;

	private NavAStarNode _startNode;

	private NavAStarNode _endNode;

	private NavAccessFlags _accessMask;

	private List<NavAStarNode> _route;

	public List<NavAStarNode> route => _route;

	public List<NavAStarNode> visitedList => _visitedList;

	public bool drawSearchDebug
	{
		get
		{
			return _visitedList != null;
		}
		set
		{
			if (value != drawSearchDebug)
			{
				if (value)
				{
					_visitedList = new List<NavAStarNode>();
				}
				else
				{
					_visitedList = null;
				}
			}
		}
	}

	private void ClearNodes()
	{
		_allNodes = null;
		_openList = null;
		_closedList = null;
		_brushMap = null;
	}

	public void AddNodesForBrushes(NavBrushComponent[] brushes)
	{
		ClearNodes();
		_allNodes = new NavAStarNode[brushes.Length];
		_openList = new List<NavAStarNode>();
		_closedList = new List<NavAStarNode>();
		_brushMap = new Dictionary<NavBrushComponent, NavAStarNode>();
		_neighbourPool = new List<NavAStarNode.Neighbour>();
		int num = 0;
		foreach (NavBrushComponent navBrushComponent in brushes)
		{
			_allNodes[num] = new NavAStarNode();
			_allNodes[num].Init(navBrushComponent);
			_brushMap[navBrushComponent] = _allNodes[num];
			num++;
		}
	}

	public void UpdateConnectivity()
	{
		_neighbourPoolUseCount = 0;
		NavAStarNode[] allNodes = _allNodes;
		foreach (NavAStarNode navAStarNode in allNodes)
		{
			navAStarNode.neighbours.Clear();
			NavBrushComponent brush = navAStarNode.brush;
			int num = 0;
			NavBoundaryComponent[] boundaries = brush.boundaries;
			foreach (NavBoundaryComponent navBoundaryComponent in boundaries)
			{
				int count = navBoundaryComponent.links.Count;
				for (int k = 0; k < count; k++)
				{
					NavBrushLink navBrushLink = navBoundaryComponent.links[k];
					NavBrushComponent parentBrush = navBrushLink.GetOtherBoundary(navBoundaryComponent).parentBrush;
					NavAStarNode.Neighbour neighbourFromPool = GetNeighbourFromPool();
					neighbourFromPool.link = navBrushLink;
					neighbourFromPool.node = _brushMap[parentBrush];
					navAStarNode.neighbours.Add(neighbourFromPool);
					num++;
				}
			}
		}
	}

	private NavAStarNode.Neighbour GetNeighbourFromPool()
	{
		if (_neighbourPoolUseCount >= _neighbourPool.Count)
		{
			_neighbourPool.Add(new NavAStarNode.Neighbour());
		}
		_neighbourPoolUseCount++;
		return _neighbourPool[_neighbourPoolUseCount - 1];
	}

	private void ResetOpenList()
	{
		_openList.Clear();
		_closedList.Clear();
		NavAStarNode[] allNodes = _allNodes;
		for (int i = 0; i < allNodes.Length; i++)
		{
			allNodes[i].searchParent = null;
		}
	}

	public void FloodFill(NavBrushComponent startBrush, NavAccessFlags accessMask, FloodFillVisitor visitor)
	{
		if (_openList != null)
		{
			if (_isSearching)
			{
				throw new Exception("Illegal point to do flood fill");
			}
			ResetOpenList();
			_route = null;
			if (_visitedList != null)
			{
				_visitedList.Clear();
			}
			_isSearching = true;
			_startNode = _brushMap[startBrush];
			_endNode = null;
			_accessMask = accessMask;
			_openList.Add(_startNode);
			AdvanceFloodFill(10000, visitor);
		}
	}

	private bool AdvanceFloodFill(int maxVisits, FloodFillVisitor visitor)
	{
		if (!_isSearching)
		{
			return false;
		}
		for (int i = 0; i < maxVisits; i++)
		{
			if (_openList.Count == 0)
			{
				_isSearching = false;
				return true;
			}
			NavAStarNode navAStarNode = _openList[0];
			if (_visitedList != null)
			{
				_visitedList.Add(navAStarNode);
			}
			_openList.Remove(navAStarNode);
			_closedList.Add(navAStarNode);
			visitor(navAStarNode.brush);
			int count = navAStarNode.neighbours.Count;
			for (int j = 0; j < count; j++)
			{
				NavAStarNode.Neighbour neighbour = navAStarNode.neighbours[j];
				if (neighbour.node != null && !_closedList.Contains(neighbour.node) && (neighbour.link.flags & _accessMask) == _accessMask && (!IsNodePartOfTeleporter(navAStarNode) || navAStarNode == _endNode || navAStarNode == _startNode) && neighbour.node.searchParent == null)
				{
					neighbour.node.searchParent = navAStarNode;
					_openList.Add(neighbour.node);
				}
			}
		}
		return false;
	}

	public void CancelQuery()
	{
		_isSearching = false;
	}

	public void StartQuery(NavBrushComponent start, NavBrushComponent end, NavAccessFlags accessMask)
	{
		ResetOpenList();
		_route = null;
		if (_visitedList != null)
		{
			_visitedList.Clear();
		}
		if (!(start == null) && !(end == null))
		{
			_isSearching = true;
			_startNode = _brushMap[start];
			_endNode = _brushMap[end];
			_accessMask = accessMask;
			_startNode.g = 0f;
			CalcHeuristics(_startNode, _startNode, _endNode);
			_openList.Add(_startNode);
		}
	}

	public bool AdvanceQuery(int maxVisits)
	{
		if (!_isSearching)
		{
			return false;
		}
		for (int i = 0; i < maxVisits; i++)
		{
			if (_openList.Count == 0)
			{
				_isSearching = false;
				return true;
			}
			NavAStarNode navAStarNode = FindBestNodeInList(_openList);
			if (navAStarNode == null)
			{
				throw new Exception("No best node found");
			}
			if (_visitedList != null)
			{
				_visitedList.Add(navAStarNode);
			}
			if (navAStarNode == _endNode)
			{
				_route = CreateRoute(_startNode, navAStarNode);
				_isSearching = false;
				return true;
			}
			_openList.Remove(navAStarNode);
			_closedList.Add(navAStarNode);
			foreach (NavAStarNode.Neighbour neighbour in navAStarNode.neighbours)
			{
				if (neighbour.node != null && !_closedList.Contains(neighbour.node) && (neighbour.link.flags & _accessMask) == _accessMask && (!IsNodePartOfTeleporter(navAStarNode) || navAStarNode == _endNode || navAStarNode == _startNode))
				{
					CalcHeuristics(neighbour.node, navAStarNode, _endNode);
					if (neighbour.node.searchParent == null)
					{
						neighbour.node.searchParent = navAStarNode;
						_openList.Add(neighbour.node);
					}
				}
			}
		}
		return false;
	}

	private bool IsNodePartOfTeleporter(NavAStarNode node)
	{
		return node.brush.hasPermanentConnections;
	}

	public void DrawDebug()
	{
		if (_route != null)
		{
			for (int i = 0; i < _route.Count - 1; i++)
			{
				Gizmos.color = Color.yellow;
				Gizmos.DrawLine(_route[i].brush.gameObject.transform.position + new Vector3(0f, 0.1f, 0f), _route[i + 1].brush.gameObject.transform.position + new Vector3(0f, 0.1f, 0f));
			}
		}
		if (_visitedList == null)
		{
			return;
		}
		foreach (NavAStarNode visited in _visitedList)
		{
			Vector3 vector = 0.25f * visited.brush.transform.TransformDirection(visited.brush.normal);
			Gizmos.color = new Color(0f, 1f, 0f, 1f);
			foreach (NavAStarNode.Neighbour neighbour in visited.neighbours)
			{
				if (neighbour != null)
				{
					Gizmos.color = (_visitedList.Contains(neighbour.node) ? Color.green : Color.gray);
					Gizmos.DrawLine(visited.brush.transform.position + vector, neighbour.node.brush.transform.position);
				}
			}
		}
		if (_startNode != null)
		{
			Gizmos.color = Color.cyan;
			Gizmos.DrawSphere(_startNode.brush.transform.position, 0.2f);
		}
		if (_endNode != null)
		{
			Gizmos.color = Color.magenta;
			Gizmos.DrawSphere(_endNode.brush.transform.position, 0.2f);
		}
	}

	private static void CalcHeuristics(NavAStarNode node, NavAStarNode parent, NavAStarNode target)
	{
		float num = parent.g + (node.panSpacePos - parent.panSpacePos).magnitude;
		float magnitude = (target.panSpacePos - node.panSpacePos).magnitude;
		float num2 = num + magnitude;
		if (node.searchParent == null || !(num2 >= node.f))
		{
			node.g = num;
			node.h = magnitude;
			node.f = num2;
		}
	}

	private static NavAStarNode FindBestNodeInList(List<NavAStarNode> list)
	{
		NavAStarNode navAStarNode = null;
		foreach (NavAStarNode item in list)
		{
			if (navAStarNode == null || item.f < navAStarNode.f)
			{
				navAStarNode = item;
			}
		}
		return navAStarNode;
	}

	private static List<NavAStarNode> CreateRoute(NavAStarNode start, NavAStarNode end)
	{
		List<NavAStarNode> list = new List<NavAStarNode>();
		NavAStarNode navAStarNode = end;
		while (true)
		{
			list.Add(navAStarNode);
			if (navAStarNode == start)
			{
				break;
			}
			navAStarNode = navAStarNode.searchParent;
		}
		return list;
	}
}
