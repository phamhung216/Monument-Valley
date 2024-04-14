using System.Collections;
using UnityEngine;

public class FinaleMaterialsLogic : MonoBehaviour
{
	public MeshRenderer sky;

	public MeshRenderer gradientTop;

	public MeshRenderer gradientBottom;

	public Material skyMat1;

	public Material skyMat2;

	public Material skyMat3;

	public Material skyMat4;

	public Material towerMat1;

	public Material towerMat2;

	public Material towerMat3;

	public Material towerMat4;

	public Material gradientMat1;

	public Material gradientMat2;

	public Material gradientMat3;

	public Material gradientMat4;

	public AnimationCurveDefinition animationCurve;

	public float duration = 5f;

	private bool _changing;

	private Color _fromColorSky;

	private Color _toColorSky;

	private Color _fromLight0;

	private Color _toLight0;

	private Color _fromLight1;

	private Color _toLight1;

	private Color _fromLight2;

	private Color _toLight2;

	private Color _fromColorGradient;

	private Color _toColorGradient;

	private float _startTime;

	public MaterialInstantiator towerMaterialInstantiator;

	private Material _towerMaterial;

	private void Start()
	{
		sky.material.SetColor("_Colour", skyMat1.GetColor("_Colour"));
		gradientTop.material.SetColor("_TintColor", gradientMat1.GetColor("_TintColor"));
		gradientBottom.material.SetColor("_TintColor", gradientMat1.GetColor("_TintColor"));
		_towerMaterial = towerMaterialInstantiator.instantiatedMaterial;
		if (animationCurve == null)
		{
			GameObject gameObject = GameObject.Find("EaseInOutCurve");
			if ((bool)gameObject && (bool)gameObject.GetComponent<AnimationCurveDefinition>())
			{
				animationCurve = gameObject.GetComponent<AnimationCurveDefinition>();
			}
		}
	}

	private void Update()
	{
		if (_changing)
		{
			float num = ((duration <= 0f) ? 1f : ((Time.time - _startTime) / duration));
			float t = animationCurve.curve.Evaluate(num);
			if (num >= 1f)
			{
				t = 1f;
			}
			sky.material.SetColor("_Colour", Color.Lerp(_fromColorSky, _toColorSky, t));
			_towerMaterial.SetColor("_LightColour0", Color.Lerp(_fromLight0, _toLight0, t));
			_towerMaterial.SetColor("_LightColour1", Color.Lerp(_fromLight1, _toLight1, t));
			_towerMaterial.SetColor("_LightColour2", Color.Lerp(_fromLight2, _toLight2, t));
			gradientTop.material.SetColor("_TintColor", Color.Lerp(_fromColorGradient, _toColorGradient, t));
			gradientBottom.material.SetColor("_TintColor", Color.Lerp(_fromColorGradient, _toColorGradient, t));
			if (num >= 1f)
			{
				_changing = false;
			}
		}
	}

	[TriggerableAction]
	public IEnumerator ChangeToMaterial1()
	{
		_toColorSky = skyMat1.GetColor("_Colour");
		_toLight0 = towerMat1.GetColor("_LightColour0");
		_toLight1 = towerMat1.GetColor("_LightColour1");
		_toLight2 = towerMat1.GetColor("_LightColour2");
		_toColorGradient = gradientMat1.GetColor("_TintColor");
		StartColourChange();
		return null;
	}

	[TriggerableAction]
	public IEnumerator ChangeToMaterial2()
	{
		_toColorSky = skyMat2.GetColor("_Colour");
		_toLight0 = towerMat2.GetColor("_LightColour0");
		_toLight1 = towerMat2.GetColor("_LightColour1");
		_toLight2 = towerMat2.GetColor("_LightColour2");
		_toColorGradient = gradientMat2.GetColor("_TintColor");
		StartColourChange();
		return null;
	}

	[TriggerableAction]
	public IEnumerator ChangeToMaterial3()
	{
		_toColorSky = skyMat3.GetColor("_Colour");
		_toLight0 = towerMat3.GetColor("_LightColour0");
		_toLight1 = towerMat3.GetColor("_LightColour1");
		_toLight2 = towerMat3.GetColor("_LightColour2");
		_toColorGradient = gradientMat3.GetColor("_TintColor");
		StartColourChange();
		return null;
	}

	[TriggerableAction]
	public IEnumerator ChangeToMaterial4()
	{
		_toColorSky = skyMat4.GetColor("_Colour");
		_toLight0 = towerMat4.GetColor("_LightColour0");
		_toLight1 = towerMat4.GetColor("_LightColour1");
		_toLight2 = towerMat4.GetColor("_LightColour2");
		_toColorGradient = gradientMat4.GetColor("_TintColor");
		StartColourChange();
		return null;
	}

	private void StartColourChange()
	{
		_fromColorSky = sky.material.GetColor("_Colour");
		_fromLight0 = _towerMaterial.GetColor("_LightColour0");
		_fromLight1 = _towerMaterial.GetColor("_LightColour1");
		_fromLight2 = _towerMaterial.GetColor("_LightColour2");
		_fromColorGradient = gradientTop.material.GetColor("_TintColor");
		_startTime = Time.time;
		_changing = true;
	}
}
