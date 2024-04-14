using System.Xml.Serialization;
using UnityEngine;

public class GameTouch
{
	private int _id;

	private TouchPhase _phase;

	private Vector2 _pos;

	private Vector2 _lastPos;

	private Vector2 _initialPos;

	private float _startTime;

	public float endTime;

	private int _tapCount;

	[XmlIgnore]
	private GameObject _owner;

	public int id
	{
		get
		{
			return _id;
		}
		set
		{
			_id = value;
		}
	}

	public TouchPhase phase
	{
		get
		{
			return _phase;
		}
		set
		{
			_phase = value;
		}
	}

	public Vector2 pos
	{
		get
		{
			return _pos;
		}
		set
		{
			_pos = value;
		}
	}

	public Vector2 lastPos
	{
		get
		{
			return _lastPos;
		}
		set
		{
			_lastPos = value;
		}
	}

	public Vector2 initialPos
	{
		get
		{
			return _initialPos;
		}
		set
		{
			_initialPos = value;
		}
	}

	public float startTime => _startTime;

	public int tapCount
	{
		get
		{
			return _tapCount;
		}
		set
		{
			_tapCount = value;
		}
	}

	public bool isTap => tapCount > 0;

	[XmlIgnore]
	public GameObject owner
	{
		get
		{
			return _owner;
		}
		set
		{
			_owner = value;
		}
	}

	public GameTouch(int id)
	{
		_id = id;
		_startTime = Time.time;
	}

	private GameTouch()
	{
	}

	public GameTouch Clone()
	{
		GameTouch gameTouch = new GameTouch();
		gameTouch.Copy(this);
		return gameTouch;
	}

	public void Copy(GameTouch other)
	{
		_id = other._id;
		_phase = other._phase;
		_pos = other._pos;
		_lastPos = other._lastPos;
		_initialPos = other._initialPos;
		_startTime = other._startTime;
		endTime = other.endTime;
		_tapCount = other._tapCount;
		_owner = other._owner;
	}
}
