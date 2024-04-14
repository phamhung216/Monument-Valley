using System.Collections;
using UnityEngine;

public class ViewportSwitcher : TriggerItem
{
	public CameraViewportController viewPortController;

	private Vector3 _forwardWS;

	private Vector3 _leftWS;

	public Transform[] rooms;

	private Transform[] _roomForwards;

	private int _currentIndex;

	private int _nextIndex = 1;

	public int totalRooms;

	public Transform[] trees;

	private int _currentTreeIndex;

	private int _nextTreeIndex = 1;

	public bool treeGrowing;

	public Transform finalPath;

	private int _belowDirection = 1;

	private int _aboveDirection = 1;

	private void Start()
	{
		_forwardWS = new Vector3(-1f, 0f, -1f).normalized;
		_leftWS = new Vector3(-1f, 0f, 1f).normalized;
		_roomForwards = new Transform[rooms.Length];
		for (int i = 0; i < rooms.Length; i++)
		{
			_roomForwards[i] = rooms[i].GetComponentInChildren<NSidedForwardDirection>().transform;
		}
		viewPortController.SetRoots(rooms);
	}

	private void Update()
	{
		if (viewPortController.viewportScrollingEnabled)
		{
			UpdateStates();
		}
	}

	private void UpdateStates()
	{
		Transform transform = rooms[_currentIndex];
		bool num = Vector3.Dot(_roomForwards[_currentIndex].forward, _forwardWS) > 0f;
		bool flag = Vector3.Dot(_roomForwards[_currentIndex].forward, _leftWS) < 0f;
		if (num)
		{
			if (flag)
			{
				_nextIndex = GetIndexBelow(_currentIndex);
				if (treeGrowing)
				{
					_nextTreeIndex = GetTreeIndexBelow(_currentTreeIndex);
				}
			}
			else
			{
				_nextIndex = GetIndexAbove(_currentIndex);
				if (treeGrowing)
				{
					_nextTreeIndex = GetTreeIndexAbove(_currentTreeIndex);
				}
			}
			Transform transform2 = rooms[_nextIndex];
			viewPortController.SetRearViewport(transform2);
			if (treeGrowing)
			{
				Transform transform3 = trees[_nextTreeIndex];
				if (!transform3.IsChildOf(transform2))
				{
					transform3.parent = transform2.GetComponentInChildren<NSidedTreeRoot>().transform;
					transform3.localPosition = Vector3.zero;
					transform3.rotation = _roomForwards[_nextIndex].rotation;
					if (_nextTreeIndex % 2 == 0)
					{
						transform3.Rotate(0f, 180f, 0f);
					}
				}
				if (_nextTreeIndex == trees.Length - 1)
				{
					finalPath.parent = transform2.GetComponentInChildren<NSidedFinalPathRoot>().transform;
					finalPath.localPosition = Vector3.zero;
					finalPath.rotation = _roomForwards[_nextIndex].rotation;
					finalPath.Rotate(0f, -45f, 0f);
				}
				else if (_currentTreeIndex == trees.Length - 1)
				{
					finalPath.parent = transform.GetComponentInChildren<NSidedFinalPathRoot>().transform;
					finalPath.localPosition = Vector3.zero;
					finalPath.rotation = _roomForwards[_currentIndex].rotation;
					finalPath.Rotate(0f, -45f, 0f);
				}
				else
				{
					finalPath.parent = null;
					finalPath.position = new Vector3(30f, 50f, -30f);
				}
			}
			if (Vector3.Dot(_roomForwards[_nextIndex].forward, _forwardWS) > 0f)
			{
				TransformFollower componentInChildren = transform2.GetComponentInChildren<NSidedFrame>().GetComponentInChildren<TransformFollower>();
				componentInChildren.SetRotationOffset(componentInChildren.rotationOffset + new Vector3(0f, 180f, 0f));
			}
		}
		else
		{
			int nextIndex = _nextIndex;
			_nextIndex = _currentIndex;
			_currentIndex = nextIndex;
			nextIndex = _nextTreeIndex;
			_nextTreeIndex = _currentTreeIndex;
			_currentTreeIndex = nextIndex;
		}
		if (treeGrowing)
		{
			for (int i = 0; i < trees.Length; i++)
			{
				trees[i].gameObject.SetActive(i == _nextTreeIndex || i == _currentTreeIndex);
			}
		}
	}

	private int GetIndexBelow(int index)
	{
		index--;
		if (index < 0)
		{
			index = totalRooms - 1;
		}
		return index;
	}

	private int GetIndexAbove(int index)
	{
		index++;
		if (index > totalRooms - 1)
		{
			index = 0;
		}
		return index;
	}

	private int GetTreeIndexBelow(int index)
	{
		if (index == 0)
		{
			_belowDirection = 1;
			_aboveDirection = -1;
		}
		else if (index == trees.Length - 1)
		{
			_belowDirection = -1;
			_aboveDirection = 1;
		}
		index += _belowDirection;
		return index;
	}

	private int GetTreeIndexAbove(int index)
	{
		if (index == 0)
		{
			_aboveDirection = 1;
			_belowDirection = -1;
		}
		else if (index == trees.Length - 1)
		{
			_aboveDirection = -1;
			_belowDirection = 1;
		}
		index += _aboveDirection;
		return index;
	}

	[TriggerableAction]
	public IEnumerator AddRoom()
	{
		totalRooms++;
		return null;
	}

	[TriggerableAction]
	public IEnumerator StartTreeGrowing()
	{
		treeGrowing = true;
		_currentTreeIndex = 0;
		_nextTreeIndex = 1;
		return null;
	}

	[TriggerableAction]
	public IEnumerator SecondPhaseRooms()
	{
		totalRooms = 2;
		_currentIndex = 0;
		_nextIndex = 1;
		rooms = new Transform[2]
		{
			rooms[2],
			rooms[3]
		};
		return null;
	}

	[TriggerableAction]
	public IEnumerator AssignFinalRoom()
	{
		viewPortController.frontRoot = rooms[1];
		viewPortController.rearRoot = rooms[0];
		if (Vector3.Dot(_roomForwards[1].forward, _forwardWS) > 0f)
		{
			TransformFollower componentInChildren = viewPortController.frontRoot.GetComponentInChildren<NSidedFrame>().GetComponentInChildren<TransformFollower>();
			componentInChildren.SetRotationOffset(componentInChildren.rotationOffset + new Vector3(0f, 180f, 0f));
		}
		viewPortController.frontRoot.GetComponentInChildren<Camera>();
		viewPortController.frontRoot.GetComponentInChildren<Camera>().enabled = true;
		return null;
	}

	[TriggerableAction]
	public IEnumerator PlaceFinalPathAndTree()
	{
		Transform obj = trees[trees.Length - 1];
		Transform transform = rooms[1];
		Transform transform2 = _roomForwards[1];
		obj.parent = transform.GetComponentInChildren<NSidedTreeRoot>().transform;
		obj.localPosition = Vector3.zero;
		obj.rotation = transform2.rotation;
		finalPath.parent = transform.GetComponentInChildren<NSidedFinalPathRoot>().transform;
		finalPath.localPosition = Vector3.zero;
		finalPath.rotation = transform2.rotation;
		finalPath.Rotate(0f, -45f, 0f);
		return null;
	}

	[TriggerableAction]
	public IEnumerator CheckpointBridgeRooms()
	{
		totalRooms = 4;
		_currentIndex = 2;
		_nextIndex = 3;
		Transform frontRoot = rooms[2];
		Transform rearRoot = rooms[3];
		viewPortController.frontRoot = frontRoot;
		viewPortController.rearRoot = rearRoot;
		if (!(Vector3.Dot(_roomForwards[2].forward, _forwardWS) > 0f))
		{
			TransformFollower componentInChildren = viewPortController.frontRoot.GetComponentInChildren<NSidedFrame>().GetComponentInChildren<TransformFollower>();
			componentInChildren.SetRotationOffset(componentInChildren.rotationOffset + new Vector3(0f, 180f, 0f));
		}
		if (Vector3.Dot(_roomForwards[3].forward, _forwardWS) > 0f)
		{
			TransformFollower componentInChildren2 = viewPortController.rearRoot.GetComponentInChildren<NSidedFrame>().GetComponentInChildren<TransformFollower>();
			componentInChildren2.SetRotationOffset(componentInChildren2.rotationOffset + new Vector3(0f, 180f, 0f));
		}
		viewPortController.UpdateStates();
		return null;
	}

	[TriggerableAction]
	public IEnumerator CheckpointWaterRooms()
	{
		totalRooms = 4;
		_currentIndex = 0;
		_nextIndex = 1;
		Transform frontRoot = rooms[0];
		Transform rearRoot = rooms[1];
		viewPortController.frontRoot = frontRoot;
		viewPortController.rearRoot = rearRoot;
		if (!(Vector3.Dot(_roomForwards[0].forward, _forwardWS) > 0f))
		{
			TransformFollower componentInChildren = viewPortController.frontRoot.GetComponentInChildren<NSidedFrame>().GetComponentInChildren<TransformFollower>();
			componentInChildren.SetRotationOffset(componentInChildren.rotationOffset + new Vector3(0f, 180f, 0f));
		}
		if (Vector3.Dot(_roomForwards[1].forward, _forwardWS) > 0f)
		{
			TransformFollower componentInChildren2 = viewPortController.rearRoot.GetComponentInChildren<NSidedFrame>().GetComponentInChildren<TransformFollower>();
			componentInChildren2.SetRotationOffset(componentInChildren2.rotationOffset + new Vector3(0f, 180f, 0f));
		}
		viewPortController.UpdateStates();
		return null;
	}

	[TriggerableAction]
	public IEnumerator CheckpointTreeRooms()
	{
		totalRooms = 2;
		_currentIndex = 0;
		_nextIndex = 1;
		Transform transform = rooms[3];
		Transform transform2 = rooms[2];
		rooms = new Transform[2] { transform, transform2 };
		_roomForwards = new Transform[2]
		{
			transform.GetComponentInChildren<NSidedForwardDirection>().transform,
			transform2.GetComponentInChildren<NSidedForwardDirection>().transform
		};
		viewPortController.frontRoot = transform;
		viewPortController.rearRoot = transform2;
		if (Vector3.Dot(_roomForwards[0].forward, _forwardWS) > 0f)
		{
			TransformFollower componentInChildren = viewPortController.frontRoot.GetComponentInChildren<NSidedFrame>().GetComponentInChildren<TransformFollower>();
			componentInChildren.SetRotationOffset(componentInChildren.rotationOffset + new Vector3(0f, 180f, 0f));
		}
		if (!(Vector3.Dot(_roomForwards[1].forward, _forwardWS) > 0f))
		{
			TransformFollower componentInChildren2 = viewPortController.rearRoot.GetComponentInChildren<NSidedFrame>().GetComponentInChildren<TransformFollower>();
			componentInChildren2.SetRotationOffset(componentInChildren2.rotationOffset + new Vector3(0f, 180f, 0f));
		}
		viewPortController.UpdateStates();
		return null;
	}
}
