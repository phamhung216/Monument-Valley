using System.Collections;
using UnityEngine;

public class IdaCloneFollower : MonoBehaviour
{
	private Transform _relativeSourceParent;

	private Transform _relativeTargetParent;

	public Animation source;

	public Animation target;

	public IdaTeleport teleport;

	public Vector3 diff = Vector3.zero;

	private IdaCloneRoot[] roots;

	private CharacterLocomotion _player;

	private GameObject _shadow;

	private Vector3 _cameraForward;

	private Renderer[] _renderers;

	private void Start()
	{
		roots = Object.FindObjectsOfType<IdaCloneRoot>();
		_player = GameScene.player.GetComponent<CharacterLocomotion>();
		GameObject gameObject = _player.GetComponentInChildren<DropShadow>().gameObject;
		_shadow = Object.Instantiate(gameObject);
		Object.Destroy(_shadow.GetComponent<DropShadow>());
		_shadow.transform.parent = target.transform.parent;
		_shadow.transform.localPosition = gameObject.transform.localPosition;
		_shadow.transform.localRotation = gameObject.transform.localRotation;
		_cameraForward = Camera.main.transform.forward;
		_renderers = target.transform.parent.GetComponentsInChildren<Renderer>();
	}

	private void LateUpdate()
	{
		NavBrushComponent lastValidBrush = _player.lastValidBrush;
		for (int i = 0; i < roots.Length; i++)
		{
			if (lastValidBrush.transform.IsChildOf(roots[i].transform))
			{
				_relativeSourceParent = roots[i].transform;
			}
			if (target.transform.IsChildOf(roots[i].transform))
			{
				_relativeTargetParent = roots[i].transform;
			}
		}
		target.transform.parent.localRotation = source.transform.parent.localRotation;
		Vector3 localPosition = _relativeSourceParent.InverseTransformPoint(source.transform.parent.position);
		target.transform.parent.localPosition = localPosition;
		bool flag = !lastValidBrush.transform.IsChildOf(_relativeTargetParent);
		for (int j = 0; j < _renderers.Length; j++)
		{
			_renderers[j].enabled = flag;
		}
		foreach (AnimationState item in source)
		{
			if (source.IsPlaying(item.name))
			{
				target.Play(item.name);
				target[item.name].time = item.time;
				target.Sample();
			}
		}
		_shadow.transform.position = target.transform.position + _cameraForward * -0.1f;
	}

	public void SwapCloneWithIda()
	{
		StartCoroutine(TrySwap());
	}

	public IEnumerator TrySwap()
	{
		yield return null;
		NavBrushComponent navBrushComponent = GameScene.navManager.FindNavBrushBelowPanPoint(GameScene.WorldToPanPoint(target.transform.position), touchableOnly: true);
		if (navBrushComponent != null && navBrushComponent != _player.lastValidBrush && navBrushComponent != _player.targetBrush)
		{
			teleport.destinationBrush = navBrushComponent;
			teleport.Teleport();
			target.GetComponentInChildren<Renderer>().enabled = false;
		}
	}
}
