using System.Collections;
using UnityEngine;

public class HatPickup : MonoBehaviour
{
	private CharacterLocomotion _ida;

	private bool _holdHat;

	private bool _hatOnHead;

	public Transform holdingOffset;

	public Transform onHeadOffset;

	public Transform _hatMeshTransform;

	public float lerpSpeed = 3f;

	private float _lerpTime;

	public float hatShrinkTime = 1f;

	public float hatShrinkEndOscillation = 0.1f;

	public AnimationCurve hatShrinkCurve;

	private bool _shrinking;

	private OscillatingMover _floatParent;

	private Vector3 _floatScale;

	private Vector3 _floatOscillation;

	private Vector3 _onHeadScale = Vector3.one;

	private float _shrinkTime;

	public Material[] idaMaterials;

	private AnimationState _idolReplaceAnimState;

	private void Start()
	{
		EnsureCharacterRef();
	}

	private void EnsureCharacterRef()
	{
		if (!_ida && (bool)GameScene.player)
		{
			_ida = GameScene.player.GetComponent<CharacterLocomotion>();
		}
	}

	[TriggerableAction]
	public IEnumerator HoldHat()
	{
		EnsureCharacterRef();
		Transform parent = _ida.transform.Find("Ida").Find("IdaArmature");
		base.transform.parent = parent;
		base.transform.localRotation = new Quaternion(0f, 0f, 0f, 1f);
		base.transform.localPosition = Vector3.zero;
		_hatMeshTransform.localPosition = holdingOffset.localPosition;
		_hatMeshTransform.localRotation = holdingOffset.localRotation;
		_hatMeshTransform.localScale = holdingOffset.localScale;
		_lerpTime = 0f;
		_holdHat = true;
		return null;
	}

	[TriggerableAction]
	public IEnumerator ShrinkHat()
	{
		_floatParent = base.transform.parent.GetComponent<OscillatingMover>();
		if (Vector3.Dot(Camera.main.transform.forward, _floatParent.transform.forward) < 0f)
		{
			_floatParent.MoveCentreOfMotion(new Vector3(-1f, 1f, -1f));
		}
		_floatScale = _floatParent.transform.localScale;
		_floatOscillation = _floatParent.deltaPosition;
		_shrinking = true;
		_shrinkTime = 0f;
		return null;
	}

	[TriggerableAction]
	public IEnumerator HatOnHead()
	{
		EnsureCharacterRef();
		_hatOnHead = true;
		_lerpTime = 0f;
		return null;
	}

	private void LateUpdate()
	{
		if (_shrinking)
		{
			float num = Mathf.Clamp01(_shrinkTime / hatShrinkTime);
			float t = hatShrinkCurve.Evaluate(num);
			_floatParent.transform.localScale = Vector3.Lerp(_floatScale, _onHeadScale, t);
			_floatParent.deltaPosition = Vector3.Lerp(_floatOscillation, hatShrinkEndOscillation * Vector3.up, t);
			if (num >= 1f)
			{
				_shrinking = false;
			}
			_shrinkTime += Time.deltaTime;
		}
		if (_holdHat)
		{
			base.transform.position = _ida.HatBone.position;
			base.transform.rotation = _ida.HatBone.rotation;
			if (!_hatOnHead)
			{
				_hatMeshTransform.localPosition = holdingOffset.localPosition;
				_hatMeshTransform.localRotation = holdingOffset.localRotation;
				_hatMeshTransform.localScale = holdingOffset.localScale;
			}
		}
		if (_hatOnHead)
		{
			_lerpTime += lerpSpeed * Time.deltaTime;
			_hatMeshTransform.localPosition = Vector3.Lerp(_hatMeshTransform.localPosition, onHeadOffset.localPosition, _lerpTime);
			_hatMeshTransform.localRotation = Quaternion.Lerp(_hatMeshTransform.localRotation, onHeadOffset.localRotation, _lerpTime);
			_hatMeshTransform.localScale = Vector3.Lerp(_hatMeshTransform.localScale, onHeadOffset.localScale, _lerpTime);
		}
		if (!_hatOnHead)
		{
			return;
		}
		if (_idolReplaceAnimState == null)
		{
			_idolReplaceAnimState = _ida.animSystem["IdolReplace"];
		}
		if ((bool)_idolReplaceAnimState && _idolReplaceAnimState.normalizedTime > 0.01f)
		{
			float t2 = 1f;
			if (_idolReplaceAnimState.normalizedTime < 0.1f)
			{
				t2 = _idolReplaceAnimState.normalizedTime / 0.1f;
			}
			else if (_idolReplaceAnimState.normalizedTime > 0.9f)
			{
				t2 = 1f - (_idolReplaceAnimState.normalizedTime - 0.9f) / 0.1f;
			}
			_hatMeshTransform.localPosition = Vector3.Lerp(onHeadOffset.localPosition, holdingOffset.localPosition, t2);
			_hatMeshTransform.localRotation = Quaternion.Lerp(onHeadOffset.localRotation, holdingOffset.localRotation, t2);
		}
	}
}
