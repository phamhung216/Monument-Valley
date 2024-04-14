using System.Collections.Generic;
using UnityEngine;

public class NavManager : MonoBehaviour
{
	public bool drawSearchDebug;

	public List<RestrictiveNavZone> zoneRestrictions;

	private List<NavRequest> _navRequests;

	private NavBrushGraph _graph = new NavBrushGraph();

	private NavAStar _aStar = new NavAStar();

	private int _reconfigurationCount;

	private bool _scanWhenReady;

	public float extraNavTouchArea;

	private CharacterLocomotion _characterLocomotion;

	public static readonly float DefaultConnectionTolerance = 0.316f;

	public void NotifyReconfigurationBegan(GameObject rootObject)
	{
		_reconfigurationCount++;
		_ = drawSearchDebug;
	}

	public void NotifyReconfigurationEnded()
	{
		_reconfigurationCount = 0;
		_ = drawSearchDebug;
		_scanWhenReady = true;
	}

	public int ReconfigurationCount()
	{
		return _reconfigurationCount;
	}

	private void Awake()
	{
		_navRequests = new List<NavRequest>();
	}

	private void Start()
	{
		Init();
	}

	public void Init()
	{
		if (_graph.allBrushes == null)
		{
			_graph.Init();
			_aStar.AddNodesForBrushes(_graph.allBrushes);
			_scanWhenReady = true;
			ScanAllConnections();
			InitAllCharacters();
		}
	}

	private void InitAllCharacters()
	{
		BaseLocomotion[] array = Object.FindObjectsOfType<BaseLocomotion>();
		for (int i = 0; i < array.Length; i++)
		{
			array[i].InitNav();
		}
	}

	public bool IsNavRestricted(NavAccessFlags objectFlag, NavBrushComponent brush)
	{
		foreach (RestrictiveNavZone zoneRestriction in zoneRestrictions)
		{
			if (zoneRestriction.zone == brush.zone && (zoneRestriction.accessors & objectFlag) != 0)
			{
				return true;
			}
		}
		return false;
	}

	public void ScanAllConnections()
	{
		_graph.UpdateLinks();
		if ((bool)GameScene.player)
		{
			if (_characterLocomotion == null)
			{
				_characterLocomotion = GameScene.player.GetComponent<CharacterLocomotion>();
			}
			UpdatePlayerAccessibility(_characterLocomotion.lastValidBrush);
		}
	}

	public void Update()
	{
		_aStar.drawSearchDebug = drawSearchDebug;
		if (_reconfigurationCount <= 0 && _scanWhenReady)
		{
			for (int i = 0; i < _navRequests.Count; i++)
			{
				if (_navRequests[i].status == NavRequest.RequestStatus.Processing)
				{
					ResetRoutePlan(_navRequests[i]);
				}
			}
			ScanAllConnections();
			_scanWhenReady = false;
		}
		for (int j = 0; j < _navRequests.Count; j++)
		{
			if (_navRequests[j].status == NavRequest.RequestStatus.Complete)
			{
				continue;
			}
			if (_navRequests[j].status == NavRequest.RequestStatus.Pending)
			{
				StartRoutePlan(_navRequests[j]);
				_navRequests[j].status = NavRequest.RequestStatus.Processing;
				break;
			}
			if (_navRequests[j].status != NavRequest.RequestStatus.Processing)
			{
				continue;
			}
			if (!_aStar.AdvanceQuery(int.MaxValue))
			{
				break;
			}
			if (_aStar.route != null)
			{
				for (int k = 0; k < _aStar.route.Count; k++)
				{
					_navRequests[j].route.Insert(0, _aStar.route[k].brush);
				}
			}
			_navRequests[j].status = NavRequest.RequestStatus.Complete;
			break;
		}
	}

	public void AddRequest(NavRequest request)
	{
		_navRequests.Add(request);
	}

	public void RemoveRequest(NavRequest request)
	{
		if (request.status == NavRequest.RequestStatus.Processing)
		{
			ResetRoutePlan(request);
			request.status = NavRequest.RequestStatus.Complete;
		}
		_navRequests.Remove(request);
	}

	public void ResetRoutePlan(NavRequest request)
	{
		if (request.status == NavRequest.RequestStatus.Processing)
		{
			_aStar.CancelQuery();
			request.status = NavRequest.RequestStatus.Pending;
		}
	}

	private void StartRoutePlan(NavRequest request)
	{
		if (!_scanWhenReady)
		{
			_ = _reconfigurationCount;
			_ = 0;
		}
		_aStar.UpdateConnectivity();
		_aStar.StartQuery(request.startBrush, request.endBrush, request.accessMask);
	}

	public NavBrushComponent FindNavBrushBelowPanPoint(Vector3 point, bool touchableOnly)
	{
		float num = 0.5f;
		point.z -= num;
		NavBrushComponent navBrushComponent = _graph.FindTopBrushBelowPanPoint(point, touchableOnly, extraNavTouchArea);
		if ((bool)navBrushComponent && (bool)GameScene.instance && touchableOnly && (GameScene.WorldToPanPoint(navBrushComponent.transform.position) - navBrushComponent.panSpacePos).sqrMagnitude > 1f)
		{
			return null;
		}
		return navBrushComponent;
	}

	public bool TestNavBrushesAreStillConnected(NavBrushComponent brushA, NavBrushComponent brushB, float connectionTolerance)
	{
		bool result = false;
		if (!brushB.discarded)
		{
			NavBoundaryComponent[] boundaries = brushA.boundaries;
			foreach (NavBoundaryComponent navBoundaryComponent in boundaries)
			{
				for (int j = 0; j < navBoundaryComponent.links.Count; j++)
				{
					NavBrushLink navBrushLink = navBoundaryComponent.links[j];
					if ((navBrushLink.flags & NavAccessFlags.NotBlocked) != 0 && navBrushLink.GetOtherBoundary(navBoundaryComponent).parentBrush == brushB)
					{
						NavBoundaryComponent otherBoundary = navBrushLink.GetOtherBoundary(navBoundaryComponent);
						if (((Vector2)GameScene.WorldToPanPoint(navBoundaryComponent.transform.position) - (Vector2)GameScene.WorldToPanPoint(otherBoundary.transform.position)).sqrMagnitude <= connectionTolerance * connectionTolerance)
						{
							result = true;
						}
					}
				}
			}
		}
		return result;
	}

	public List<NavBrushComponent> FindNavBrushesInRegion(Transform position, Vector3 size)
	{
		Init();
		return _graph.FindBrushesInRegion(position, size);
	}

	private void OnDrawGizmos()
	{
		_aStar.DrawDebug();
		if (_reconfigurationCount == 0)
		{
			_graph.OnDrawGizmos();
		}
	}

	public void TestConnection(NavBoundaryComponent boundaryA, NavBoundaryComponent boundaryB)
	{
		_graph.DebugLink(boundaryA, boundaryB);
	}

	public List<NavAStarNode> GetAStarVisitedList()
	{
		return _aStar.visitedList;
	}

	public void UpdatePlayerAccessibility(NavBrushComponent startBrush)
	{
		if ((bool)startBrush)
		{
			for (int i = 0; i < _graph.allBrushes.Length; i++)
			{
				_graph.allBrushes[i].isAccessibleByPlayer = false;
			}
			_aStar.UpdateConnectivity();
			_aStar.FloodFill(startBrush, NavAccessFlags.Player | NavAccessFlags.NotBlocked, VisitBrushForAccessibility);
		}
	}

	private void VisitBrushForAccessibility(NavBrushComponent brush)
	{
		brush.isAccessibleByPlayer = true;
	}

	public void BakeNav()
	{
		UpdateChunks();
	}

	public void UpdateChunks()
	{
		NavChunk[] obj = (NavChunk[])Object.FindObjectsOfType(typeof(NavChunk));
		int num = 0;
		NavChunk[] array = obj;
		foreach (NavChunk navChunk in array)
		{
			num = (navChunk.index = num + 1);
			NavBrushComponent[] componentsInChildren = navChunk.transform.GetComponentsInChildren<NavBrushComponent>(includeInactive: true);
			int num2 = 0;
			NavBrushComponent[] array2 = componentsInChildren;
			for (int j = 0; j < array2.Length; j++)
			{
				array2[j].chunkIndex = navChunk.index;
				num2++;
			}
		}
	}
}
