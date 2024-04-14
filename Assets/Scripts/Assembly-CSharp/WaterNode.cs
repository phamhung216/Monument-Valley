using System.Collections;
using UnityEngine;

public class WaterNode : MonoBehaviour
{
	public enum WaterTileType
	{
		WaterFlat = 0,
		WaterStartFall = 1,
		WaterFall = 2,
		WaterPool = 3
	}

	private enum WaterMode
	{
		WaterInvalid = 0,
		WaterEmpty = 1,
		WaterFilling = 2,
		WaterScrolling = 3
	}

	public bool isStartNode;

	public WaterNode outputNode;

	private WaterNode _inputNode;

	public float _waterLevel;

	private Material _scrollMat;

	private int _matID;

	private const int _scrollDistance = 4;

	private float _fillSpeed = 0.5f;

	public WaterTileType type;

	private WaterMode _waterMode;

	private Vector2 _scrollDir;

	public WaterManager waterManager;

	public ActionSequence triggerObjectOnFill = new ActionSequence();

	private float _waterExtra;

	public bool flowWhenReady;

	public bool isFlowing;

	private static string s_iconTexture = "GizmoWaterStart";

	public int matID => _matID;

	[TriggerableAction]
	public IEnumerator OnTrigger()
	{
		_ = isStartNode;
		flowWhenReady = true;
		return null;
	}

	private void Start()
	{
		_fillSpeed = waterManager.waterSpeed;
		if (isStartNode && outputNode != null)
		{
			outputNode.InitFromParentRecurse(this);
		}
		_scrollDir = new Vector2(0f, 1f);
	}

	private void InitFromParentRecurse(WaterNode inputNode)
	{
		_inputNode = inputNode;
		_matID = (inputNode.matID + 1) % 4;
		if (outputNode != null)
		{
			outputNode.InitFromParentRecurse(this);
		}
		SetMode(WaterMode.WaterEmpty);
	}

	private void ReInitFromParentRecurse(WaterNode inputNode)
	{
		_matID = (inputNode.matID + 1) % 4;
		if (outputNode != null)
		{
			outputNode.InitFromParentRecurse(this);
		}
		SetMode(WaterMode.WaterEmpty);
	}

	private void SetParentMatFromMatID(int ID)
	{
		switch (ID)
		{
		case 0:
			SetParentMaterial(waterManager.waterScroll1);
			break;
		case 1:
			SetParentMaterial(waterManager.waterScroll2);
			break;
		case 2:
			SetParentMaterial(waterManager.waterScroll3);
			break;
		case 3:
			SetParentMaterial(waterManager.waterScroll4);
			break;
		}
	}

	private void SetParentMaterial(Material mat)
	{
		base.gameObject.GetComponent<MeshRenderer>().material = mat;
		_scrollMat = mat;
	}

	private void SetMode(WaterMode mode)
	{
		switch (mode)
		{
		case WaterMode.WaterEmpty:
			if (_waterMode != WaterMode.WaterEmpty)
			{
				_waterExtra = 0f;
				_waterMode = WaterMode.WaterEmpty;
				SetParentMaterial(waterManager.waterEmpty);
			}
			break;
		case WaterMode.WaterFilling:
			if (_waterMode != WaterMode.WaterFilling)
			{
				StartCoroutine(triggerObjectOnFill.DoSequence());
				_waterMode = WaterMode.WaterFilling;
				switch (matID)
				{
				case 0:
					SetParentMaterial(waterManager.waterAppear);
					break;
				case 1:
					SetParentMaterial(waterManager.waterAppear2);
					break;
				case 2:
					SetParentMaterial(waterManager.waterAppear3);
					break;
				case 3:
					SetParentMaterial(waterManager.waterAppear4);
					break;
				}
			}
			break;
		case WaterMode.WaterScrolling:
			if (_waterMode != WaterMode.WaterScrolling)
			{
				_waterMode = WaterMode.WaterScrolling;
				SetParentMatFromMatID(matID);
			}
			break;
		}
	}

	private void LateUpdate()
	{
		if (isStartNode && flowWhenReady && waterManager.canStartFlowing)
		{
			if (!isFlowing)
			{
				_waterLevel = 1f;
				_matID = waterManager.StartGetOffset();
				if (outputNode != null)
				{
					outputNode.ReInitFromParentRecurse(this);
				}
			}
			isFlowing = true;
		}
		if (!isFlowing && isStartNode)
		{
			SetMode(WaterMode.WaterEmpty);
			return;
		}
		float num = 1f - (float)matID * 0.25f;
		if ((isStartNode || (_inputNode != null && _inputNode._waterLevel >= 0.25f)) && _waterLevel < 0.5f)
		{
			float value = _waterLevel + 0.25f;
			if (_waterMode == WaterMode.WaterFilling && waterManager.canStartFlowing)
			{
				_waterExtra = _waterLevel;
			}
			SetMode(WaterMode.WaterFilling);
			if (waterManager.waterFillLevel + _waterExtra > _waterLevel)
			{
				_waterLevel = waterManager.waterFillLevel + _waterExtra;
			}
			value = Mathf.Clamp(value, 0f, 0.745f);
			_scrollMat.SetFloat("_Scroll", value);
			float value2 = (float)(Time.frameCount / 3 % 4) * 0.25f;
			_scrollMat.SetFloat("_ScrollU", value2);
		}
		else if (_inputNode != null && _inputNode._waterLevel < 0.25f)
		{
			SetMode(WaterMode.WaterEmpty);
		}
		else if (type != WaterTileType.WaterPool)
		{
			SetMode(WaterMode.WaterScrolling);
			_scrollMat.SetFloat("_Scroll", waterManager.waterScroll + num);
		}
	}

	private void OnDrawGizmos()
	{
		if (isStartNode)
		{
			Gizmos.DrawIcon(base.transform.position + new Vector3(0f, 0.5f, 0f), s_iconTexture);
		}
	}
}
