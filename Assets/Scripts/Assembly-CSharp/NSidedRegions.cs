using UnityEngine;

public class NSidedRegions : MonoBehaviour
{
	private Vector3 _activePosition = Vector3.zero;

	private Vector3 _inactivePosition = new Vector3(60f, 60f, 60f);

	private Vector3 _forwardWS;

	private Vector3 _leftWS;

	public Transform[] rooms;

	private Transform[] _roomForwards;

	public Transform room3;

	public Transform room3Forward;

	private int _currentIndex;

	private int _nextIndex = 1;

	private void Start()
	{
		_forwardWS = new Vector3(-1f, 0f, -1f).normalized;
		_leftWS = new Vector3(-1f, 0f, 1f).normalized;
		_roomForwards = new Transform[rooms.Length];
		for (int i = 0; i < rooms.Length; i++)
		{
			_roomForwards[i] = rooms[i].GetComponentInChildren<NSidedForwardDirection>().transform;
		}
		UpdateStates();
	}

	private void LateUpdate()
	{
		UpdateStates();
	}

	private void UpdateStates()
	{
		rooms[_currentIndex].position = Vector3.zero;
		bool num = Vector3.Dot(_roomForwards[_currentIndex].forward, _forwardWS) > 0f;
		bool flag = Vector3.Dot(_roomForwards[_currentIndex].forward, _leftWS) > 0f;
		if (num)
		{
			if (flag)
			{
				_nextIndex = GetIndexBelow(_currentIndex);
			}
			else
			{
				_nextIndex = GetIndexAbove(_currentIndex);
			}
		}
		else
		{
			int nextIndex = _nextIndex;
			_nextIndex = _currentIndex;
			_currentIndex = nextIndex;
		}
		for (int i = 0; i < rooms.Length; i++)
		{
			rooms[i].position = ((i == _currentIndex || i == _nextIndex) ? _activePosition : (_inactivePosition * (i + 1)));
		}
	}

	private int GetIndexBelow(int index)
	{
		index--;
		if (index < 0)
		{
			index = rooms.Length - 1;
		}
		return index;
	}

	private int GetIndexAbove(int index)
	{
		index++;
		if (index > rooms.Length - 1)
		{
			index = 0;
		}
		return index;
	}
}
