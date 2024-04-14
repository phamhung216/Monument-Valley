using System.Collections;
using UnityEngine;

public class DescentFadeLogic : MonoBehaviour
{
	public MaterialInstantiator fadeMaterialInstantiator;

	public AnimationCurveDefinition animationCurve;

	public float duration;

	private Material _fadeMaterial;

	private bool _fading;

	private bool _following = true;

	private Vector4 _fromPlane;

	private Vector4 _toPlane = new Vector4(0f, -1f, 0f, 149f);

	private float _startTime;

	private void Start()
	{
		_fadeMaterial = fadeMaterialInstantiator.instantiatedMaterial;
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
		float num = 0f - GameScene.player.GetComponent<CharacterLocomotion>().shadowRootTransform.position.y;
		if (_following)
		{
			_fromPlane = _fadeMaterial.GetVector("_Plane");
			float w = _fromPlane.w + (num - _fromPlane.w) * 0.1f;
			_fadeMaterial.SetVector("_Plane", new Vector4(0f, -1f, 0f, w));
		}
		if (_fading)
		{
			float num2 = ((duration <= 0f) ? 1f : ((Time.time - _startTime) / duration));
			float t = animationCurve.curve.Evaluate(num2);
			if (num2 >= 1f)
			{
				t = 1f;
			}
			_fadeMaterial.SetVector("_Plane", Vector4.Lerp(_fromPlane, _toPlane, t));
			if (num2 >= 1f)
			{
				_fading = false;
			}
		}
	}

	[TriggerableAction]
	public IEnumerator DoFade()
	{
		_following = false;
		_fading = true;
		_startTime = Time.time;
		return null;
	}
}
