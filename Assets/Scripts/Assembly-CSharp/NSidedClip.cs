using UnityEngine;

public class NSidedClip : MonoBehaviour
{
	public MaterialInstantiator[] mats;

	public MaterialInstantiator[] floorMats;

	public NSidedClipPlanes clipPlanes;

	private Transform _planeL;

	private Transform _planeR;

	private Transform _planeTL;

	private Transform _planeTR;

	private Transform _planeBL;

	private Transform _planeBR;

	private Transform _pivot;

	private Vector3 _camForward;

	private Vector3 _crossVec;

	private Vector4 _p0 = Vector4.zero;

	private Vector4 _p1 = Vector4.zero;

	private Vector4 _p2 = Vector4.zero;

	private Vector4 _p3 = Vector4.zero;

	private Vector4 _p4 = Vector4.zero;

	private Vector4 _p5 = Vector4.zero;

	private void Start()
	{
		_planeL = clipPlanes.planeL;
		_planeR = clipPlanes.planeR;
		_planeTL = clipPlanes.planeTL;
		_planeTR = clipPlanes.planeTR;
		_planeBL = clipPlanes.planeBL;
		_planeBR = clipPlanes.planeBR;
		_pivot = clipPlanes.pivot;
		base.transform.parent.localRotation = clipPlanes.transform.localRotation;
	}

	private void LateUpdate()
	{
		_camForward = Camera.main.transform.forward;
		_crossVec = Vector3.Cross(_camForward, Vector3.up);
		_planeR.up = _crossVec;
		_p0.Set(_crossVec.x, _crossVec.y, _crossVec.z, 0f);
		_p0.w = Vector3.Dot(_crossVec, _planeR.position);
		_crossVec = Vector3.Cross(_camForward, -Vector3.up);
		_planeL.up = _crossVec;
		_p1.Set(_crossVec.x, _crossVec.y, _crossVec.z, 0f);
		_p1.w = Vector3.Dot(_crossVec, _planeL.position);
		bool flag = true;
		_crossVec = Vector3.Cross(_camForward, _pivot.forward);
		if (Vector3.Dot(_crossVec, Vector3.up) > 0f)
		{
			flag = false;
		}
		_planeTR.up = _crossVec;
		_p2.Set(_crossVec.x, _crossVec.y, _crossVec.z, 0f);
		_p2.w = Vector3.Dot(_crossVec, _planeTR.position);
		bool flag2 = true;
		_crossVec = Vector3.Cross(_camForward, -_pivot.right);
		if (Vector3.Dot(_crossVec, Vector3.up) > 0f)
		{
			flag2 = false;
		}
		_planeTL.up = _crossVec;
		_p3.Set(_crossVec.x, _crossVec.y, _crossVec.z, 0f);
		_p3.w = Vector3.Dot(_crossVec, _planeTL.position);
		bool flag3 = true;
		_crossVec = Vector3.Cross(_camForward, -_pivot.forward);
		if (Vector3.Dot(_crossVec, Vector3.up) < 0f)
		{
			flag3 = false;
		}
		_planeBL.up = _crossVec;
		_p4.Set(_crossVec.x, _crossVec.y, _crossVec.z, 0f);
		_p4.w = Vector3.Dot(_crossVec, _planeBL.position);
		if (!flag3)
		{
			_p4.Set(-1f, 0f, 0f, -1000f);
		}
		bool flag4 = true;
		_crossVec = Vector3.Cross(_camForward, _pivot.right);
		if (Vector3.Dot(_crossVec, Vector3.up) < 0f)
		{
			flag4 = false;
		}
		_planeBR.up = _crossVec;
		_p5.Set(_crossVec.x, _crossVec.y, _crossVec.z, 0f);
		_p5.w = Vector3.Dot(_crossVec, _planeBR.position);
		if (!flag4)
		{
			_p5.Set(-1f, 0f, 0f, -1000f);
		}
		for (int i = 0; i < mats.Length; i++)
		{
			MaterialInstantiator materialInstantiator = mats[i];
			materialInstantiator.instantiatedMaterial.SetVector("_Plane0", _p0);
			materialInstantiator.instantiatedMaterial.SetVector("_Plane1", _p1);
			if (flag)
			{
				materialInstantiator.instantiatedMaterial.SetVector("_Plane2", _p2);
			}
			if (flag2)
			{
				materialInstantiator.instantiatedMaterial.SetVector("_Plane3", _p3);
			}
			if (flag3)
			{
				materialInstantiator.instantiatedMaterial.SetVector("_Plane4", _p4);
			}
			if (flag4)
			{
				materialInstantiator.instantiatedMaterial.SetVector("_Plane5", _p5);
			}
		}
		Vector2 vector = Camera.main.WorldToScreenPoint(_planeR.position);
		Vector2 vector2 = Camera.main.WorldToScreenPoint(_planeL.position);
		if (vector.y < vector2.y)
		{
			_p1.Set(-1f, 0f, 0f, -1000f);
		}
		else if (vector.y > vector2.y)
		{
			_p0.Set(-1f, 0f, 0f, -1000f);
		}
		for (int j = 0; j < floorMats.Length; j++)
		{
			MaterialInstantiator obj = floorMats[j];
			obj.instantiatedMaterial.SetVector("_Plane0", _p0);
			obj.instantiatedMaterial.SetVector("_Plane1", _p1);
			obj.instantiatedMaterial.SetVector("_Plane4", _p4);
			obj.instantiatedMaterial.SetVector("_Plane5", _p5);
		}
	}
}
