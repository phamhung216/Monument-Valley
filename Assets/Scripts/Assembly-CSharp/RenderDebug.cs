using System.Collections.Generic;
using UnityEngine;

public class RenderDebug : MonoBehaviour
{
	private static RenderDebug _singleton;

	public Material lineMaterial;

	private List<RenderDebugItem> _items = new List<RenderDebugItem>();

	private List<GameObject> _lineRenderers = new List<GameObject>();

	public RenderDebug()
	{
		_singleton = null;
	}

	private void Start()
	{
	}

	private GameObject AddLineRenderer()
	{
		GameObject gameObject = new GameObject("LineRenderer" + _lineRenderers.Count, typeof(LineRenderer));
		gameObject.SetActive(value: false);
		gameObject.GetComponent<LineRenderer>().material = lineMaterial;
		gameObject.transform.parent = base.transform;
		_lineRenderers.Add(gameObject);
		return gameObject;
	}

	private void Update()
	{
		if (Debug.isDebugBuild)
		{
			RemoveOnScreenItems();
		}
	}

	private void OnDrawGizmos()
	{
		int num = 0;
		while (num < _items.Count)
		{
			RenderDebugItem renderDebugItem = _items[num];
			if (!renderDebugItem.onScreen)
			{
				renderDebugItem.Draw();
				if (Time.time - renderDebugItem.timestamp > renderDebugItem.timeout)
				{
					_items.RemoveAt(num);
				}
				else
				{
					num++;
				}
			}
			else
			{
				num++;
			}
		}
	}

	private void RemoveOnScreenItems()
	{
		int num = 0;
		while (num < _items.Count)
		{
			RenderDebugItem renderDebugItem = _items[num];
			if (renderDebugItem.onScreen)
			{
				if (Time.time - renderDebugItem.timestamp > renderDebugItem.timeout)
				{
					_items.RemoveAt(num);
					if ((bool)renderDebugItem.lineRenderer)
					{
						renderDebugItem.lineRenderer.gameObject.SetActive(value: false);
						renderDebugItem.lineRenderer = null;
					}
				}
				else
				{
					num++;
				}
			}
			else
			{
				num++;
			}
		}
	}

	public static void Add(RenderDebugItem item, float timeout)
	{
		if (!_singleton || !Debug.isDebugBuild)
		{
			return;
		}
		item.timeout = timeout;
		_singleton._items.Add(item);
		if (!item.onScreen)
		{
			return;
		}
		for (int i = 0; i < _singleton._lineRenderers.Count; i++)
		{
			if (!_singleton._lineRenderers[i].activeSelf)
			{
				item.lineRenderer = _singleton._lineRenderers[i].GetComponent<LineRenderer>();
				item.lineRenderer.gameObject.SetActive(value: true);
				item.UpdateLineRenderer();
				break;
			}
		}
		if (!item.lineRenderer)
		{
			item.lineRenderer = _singleton.AddLineRenderer().GetComponent<LineRenderer>();
		}
	}

	public static void DrawOnScreenSphere(Vector3 position, Color color, float radius, float timeout = -1f)
	{
		if ((bool)_singleton && Debug.isDebugBuild)
		{
			Add(new RenderDebugSphere(position, color, radius)
			{
				onScreen = true
			}, timeout);
		}
	}

	public static void DrawSphere(Vector3 position, Color color, float radius, float timeout = -1f)
	{
		if ((bool)_singleton && Debug.isDebugBuild)
		{
			Add(new RenderDebugSphere(position, color, radius), timeout);
		}
	}

	public static void DrawRay(Vector3 origin, Vector3 vector, Color color, float timeout = -1f)
	{
		DrawLine(origin, origin + vector, color, timeout);
	}

	public static void DrawLine(Vector3 from, Vector3 to, Color color, float timeout = -1f)
	{
		if ((bool)_singleton && Debug.isDebugBuild)
		{
			Add(new RenderDebugLine(from, to, color), timeout);
		}
	}
}
