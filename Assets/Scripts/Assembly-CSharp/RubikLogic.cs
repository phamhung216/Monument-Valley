using System.Collections.Generic;
using UnityEngine;

public class RubikLogic : MonoBehaviour
{
	public Transform[] cubes;

	public Rotatable rotatorR;

	public Rotatable rotatorL;

	public Rotatable rotatorU;

	public Rotatable rotatorD;

	public Transform container;

	private bool _rotatorRActive;

	private bool _rotatorLActive;

	private bool _rotatorUActive;

	private bool _rotatorDActive;

	private bool reset;

	private List<Transform> GetCubesR()
	{
		List<Transform> list = new List<Transform>();
		float z = cubes[0].position.z;
		Transform[] array = cubes;
		foreach (Transform transform in array)
		{
			if (transform.position.z < z)
			{
				list.Clear();
				z = transform.position.z;
			}
			if (transform.position.z == z)
			{
				list.Add(transform);
			}
		}
		return list;
	}

	private List<Transform> GetCubesL()
	{
		List<Transform> list = new List<Transform>();
		float x = cubes[0].position.x;
		Transform[] array = cubes;
		foreach (Transform transform in array)
		{
			if (transform.position.x < x)
			{
				list.Clear();
				x = transform.position.x;
			}
			if (transform.position.x == x)
			{
				list.Add(transform);
			}
		}
		return list;
	}

	private List<Transform> GetCubesU()
	{
		List<Transform> list = new List<Transform>();
		float y = cubes[0].position.y;
		Transform[] array = cubes;
		foreach (Transform transform in array)
		{
			if (transform.position.y > y)
			{
				list.Clear();
				y = transform.position.y;
			}
			if (transform.position.y == y)
			{
				list.Add(transform);
			}
		}
		return list;
	}

	private List<Transform> GetCubesD()
	{
		List<Transform> list = new List<Transform>();
		float y = cubes[0].position.y;
		Transform[] array = cubes;
		foreach (Transform transform in array)
		{
			if (transform.position.y < y)
			{
				list.Clear();
				y = transform.position.y;
			}
			if (transform.position.y == y)
			{
				list.Add(transform);
			}
		}
		return list;
	}

	private void Update()
	{
		if (!rotatorR.isStationary)
		{
			if (_rotatorRActive)
			{
				return;
			}
			rotatorL.DisableDrag();
			rotatorU.DisableDrag();
			rotatorD.DisableDrag();
			_rotatorRActive = true;
			{
				foreach (Transform item in GetCubesR())
				{
					item.parent = rotatorR.transform;
					Vector3 localPosition = item.localPosition;
					localPosition.Set(Mathf.Round(localPosition.x), Mathf.Round(localPosition.y), Mathf.Round(localPosition.z));
					item.localPosition = localPosition;
					Vector3 localEulerAngles = item.localEulerAngles;
					localEulerAngles.Set(Mathf.Round(localEulerAngles.x / 90f) * 90f, Mathf.Round(localEulerAngles.y / 90f) * 90f, Mathf.Round(localEulerAngles.z / 90f) * 90f);
					item.localEulerAngles = localEulerAngles;
				}
				return;
			}
		}
		if (!rotatorL.isStationary)
		{
			if (_rotatorLActive)
			{
				return;
			}
			rotatorR.DisableDrag();
			rotatorU.DisableDrag();
			rotatorD.DisableDrag();
			_rotatorLActive = true;
			{
				foreach (Transform item2 in GetCubesL())
				{
					item2.parent = rotatorL.transform;
					Vector3 localPosition2 = item2.localPosition;
					localPosition2.Set(Mathf.Round(localPosition2.x), Mathf.Round(localPosition2.y), Mathf.Round(localPosition2.z));
					item2.localPosition = localPosition2;
					Vector3 localEulerAngles2 = item2.localEulerAngles;
					localEulerAngles2.Set(Mathf.Round(localEulerAngles2.x / 90f) * 90f, Mathf.Round(localEulerAngles2.y / 90f) * 90f, Mathf.Round(localEulerAngles2.z / 90f) * 90f);
					item2.localEulerAngles = localEulerAngles2;
				}
				return;
			}
		}
		if (!rotatorU.isStationary)
		{
			if (_rotatorUActive)
			{
				return;
			}
			rotatorL.DisableDrag();
			rotatorR.DisableDrag();
			rotatorD.DisableDrag();
			_rotatorUActive = true;
			{
				foreach (Transform item3 in GetCubesU())
				{
					item3.parent = rotatorU.transform;
					Vector3 localPosition3 = item3.localPosition;
					localPosition3.Set(Mathf.Round(localPosition3.x), Mathf.Round(localPosition3.y), Mathf.Round(localPosition3.z));
					item3.localPosition = localPosition3;
					Vector3 localEulerAngles3 = item3.localEulerAngles;
					localEulerAngles3.Set(Mathf.Round(localEulerAngles3.x / 90f) * 90f, Mathf.Round(localEulerAngles3.y / 90f) * 90f, Mathf.Round(localEulerAngles3.z / 90f) * 90f);
					item3.localEulerAngles = localEulerAngles3;
				}
				return;
			}
		}
		if (!rotatorD.isStationary)
		{
			if (_rotatorDActive)
			{
				return;
			}
			rotatorR.DisableDrag();
			rotatorL.DisableDrag();
			rotatorU.DisableDrag();
			_rotatorDActive = true;
			{
				foreach (Transform item4 in GetCubesD())
				{
					item4.parent = rotatorD.transform;
					Vector3 localPosition4 = item4.localPosition;
					localPosition4.Set(Mathf.Round(localPosition4.x), Mathf.Round(localPosition4.y), Mathf.Round(localPosition4.z));
					item4.localPosition = localPosition4;
					Vector3 localEulerAngles4 = item4.localEulerAngles;
					localEulerAngles4.Set(Mathf.Round(localEulerAngles4.x / 90f) * 90f, Mathf.Round(localEulerAngles4.y / 90f) * 90f, Mathf.Round(localEulerAngles4.z / 90f) * 90f);
					item4.localEulerAngles = localEulerAngles4;
				}
				return;
			}
		}
		if (_rotatorRActive)
		{
			_rotatorRActive = false;
			ResetCubes();
		}
		else if (_rotatorLActive)
		{
			_rotatorLActive = false;
			ResetCubes();
		}
		else if (_rotatorUActive)
		{
			_rotatorUActive = false;
			ResetCubes();
		}
		else if (_rotatorDActive)
		{
			_rotatorDActive = false;
			ResetCubes();
		}
		rotatorR.EnableDrag();
		rotatorL.EnableDrag();
		rotatorU.EnableDrag();
		rotatorD.EnableDrag();
	}

	private void ResetCubes()
	{
		Transform[] array = cubes;
		foreach (Transform obj in array)
		{
			obj.parent = container;
			Vector3 localPosition = obj.localPosition;
			localPosition.Set(Mathf.Round(localPosition.x), Mathf.Round(localPosition.y), Mathf.Round(localPosition.z));
			obj.localPosition = localPosition;
			Vector3 localEulerAngles = obj.localEulerAngles;
			localEulerAngles.Set(Mathf.Round(localEulerAngles.x / 90f) * 90f, Mathf.Round(localEulerAngles.y / 90f) * 90f, Mathf.Round(localEulerAngles.z / 90f) * 90f);
			obj.localEulerAngles = localEulerAngles;
		}
	}
}
