using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavBlockerVolume : MonoBehaviour
{
	private int _blockingMask;

	private List<NavBrushComponent> prevCollidingBrushes;

	public Vector3 size;

	private Vector3 lastPosition;

	private bool _dirty;

	private bool isEnabled = true;

	public int blockingMask
	{
		set
		{
			_blockingMask = value;
		}
	}

	private void Start()
	{
		lastPosition = base.transform.position;
		if (size == Vector3.zero)
		{
			D.Error("Zero-sized NavBlockerVolume", base.gameObject);
		}
	}

	[TriggerableAction]
	public IEnumerator Enable()
	{
		GameScene.navManager.NotifyReconfigurationBegan(base.gameObject);
		isEnabled = true;
		_dirty = true;
		GameScene.navManager.NotifyReconfigurationEnded();
		return null;
	}

	[TriggerableAction]
	public IEnumerator Disable()
	{
		GameScene.navManager.NotifyReconfigurationBegan(base.gameObject);
		isEnabled = false;
		GameScene.navManager.NotifyReconfigurationEnded();
		return null;
	}

	private void Update()
	{
		if (_dirty || (lastPosition - base.transform.position).sqrMagnitude > 0.1f)
		{
			lastPosition = base.transform.position;
			ApplyCollidingBrushesNav();
			_dirty = false;
		}
	}

	private void UpdateBlockingNav()
	{
		if (prevCollidingBrushes != null)
		{
			int count = prevCollidingBrushes.Count;
			for (int i = 0; i < count; i++)
			{
				NavBoundaryComponent[] boundaries = prevCollidingBrushes[i].boundaries;
				foreach (NavBoundaryComponent navBoundaryComponent in boundaries)
				{
					int count2 = navBoundaryComponent.links.Count;
					for (int k = 0; k < count2; k++)
					{
						NavBrushLink navBrushLink = navBoundaryComponent.links[k];
						navBrushLink.blockingMask &= ~_blockingMask;
						if (navBrushLink.blockingMask == 0)
						{
							navBrushLink.flags |= NavAccessFlags.NotBlocked;
						}
					}
				}
			}
		}
		List<NavBrushComponent> list = null;
		if (isEnabled)
		{
			list = GameScene.navManager.FindNavBrushesInRegion(base.transform, size);
		}
		prevCollidingBrushes = list;
	}

	public void ApplyCollidingBrushesNav()
	{
		UpdateBlockingNav();
		if (prevCollidingBrushes == null)
		{
			return;
		}
		int count = prevCollidingBrushes.Count;
		for (int i = 0; i < count; i++)
		{
			NavBoundaryComponent[] boundaries = prevCollidingBrushes[i].boundaries;
			foreach (NavBoundaryComponent navBoundaryComponent in boundaries)
			{
				int count2 = navBoundaryComponent.links.Count;
				for (int k = 0; k < count2; k++)
				{
					NavBrushLink navBrushLink = navBoundaryComponent.links[k];
					navBrushLink.blockingMask |= _blockingMask;
					navBrushLink.flags &= ~NavAccessFlags.NotBlocked;
				}
			}
		}
	}

	private void OnDrawGizmos()
	{
		Color color = Color.red;
		if (!isEnabled)
		{
			color = Color.grey;
		}
		color.a = 0.5f;
		Gizmos.color = color;
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.DrawCube(Vector3.zero, size);
		Gizmos.matrix = Matrix4x4.identity;
		if (prevCollidingBrushes == null)
		{
			return;
		}
		foreach (NavBrushComponent prevCollidingBrush in prevCollidingBrushes)
		{
			Gizmos.DrawLine(base.transform.position, prevCollidingBrush.transform.position);
			Gizmos.DrawSphere(prevCollidingBrush.transform.position, 0.2f);
		}
	}
}
