using UnityEngine;

public class WaterManager : MonoBehaviour
{
	public Material waterScroll1;

	private Material _waterScroll2;

	private Material _waterScroll3;

	private Material _waterScroll4;

	public Material waterAppear;

	private Material _waterAppear2;

	private Material _waterAppear3;

	private Material _waterAppear4;

	public Material waterPool;

	public Material waterEmpty;

	public float waterSpeed = 0.5f;

	private float _prevWaterScroll = -0.0001f;

	private float _waterScroll;

	private bool _canStartFlowing;

	public Material waterScroll2 => _waterScroll2;

	public Material waterScroll3 => _waterScroll3;

	public Material waterScroll4 => _waterScroll4;

	public Material waterAppear2 => _waterAppear2;

	public Material waterAppear3 => _waterAppear3;

	public Material waterAppear4 => _waterAppear4;

	public float waterScroll => _waterScroll;

	public bool canStartFlowing => _canStartFlowing;

	public float waterFillLevel
	{
		get
		{
			int num = Mathf.FloorToInt(_waterScroll * 4f);
			return _waterScroll - (float)num * 0.25f;
		}
	}

	private void Start()
	{
		string text = waterScroll1.name;
		waterScroll1 = new Material(waterScroll1);
		waterScroll1.name = text;
		_waterScroll2 = new Material(waterScroll1);
		_waterScroll2.name += 2;
		_waterScroll3 = new Material(waterScroll1);
		_waterScroll3.name += 3;
		_waterScroll4 = new Material(waterScroll1);
		_waterScroll4.name += 4;
		_waterAppear2 = new Material(waterAppear);
		_waterAppear3 = new Material(waterAppear);
		_waterAppear4 = new Material(waterAppear);
	}

	public int StartGetOffset()
	{
		return (Mathf.FloorToInt(_waterScroll * 4f) + 3) % 4;
	}

	private void Update()
	{
		if ((_waterScroll * 4f > 1f && _prevWaterScroll * 4f < 1f) || (_waterScroll * 4f > 2f && _prevWaterScroll * 4f < 2f) || (_waterScroll * 4f > 3f && _prevWaterScroll * 4f < 3f) || (_waterScroll * 4f > 4f && _prevWaterScroll * 4f < 4f))
		{
			_canStartFlowing = true;
		}
		else
		{
			_canStartFlowing = false;
		}
		if (_waterScroll > 1f && _prevWaterScroll < 1f)
		{
			_waterScroll -= 1f;
		}
		_prevWaterScroll = _waterScroll;
		_waterScroll += waterSpeed * Time.deltaTime;
	}
}
