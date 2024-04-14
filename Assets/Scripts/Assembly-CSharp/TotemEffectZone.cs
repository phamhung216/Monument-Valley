using UnityEngine;

public class TotemEffectZone : MonoBehaviour
{
	public bool allowBounce;

	public bool allowHeadRotation;

	public bool forceHeadRight;

	public bool forceHeadLeft;

	private TotemPole _totem;

	public bool _totemInZone;

	private Collider _collider;

	private void Start()
	{
		_totem = Object.FindObjectOfType(typeof(TotemPole)) as TotemPole;
		_collider = _totem.GetComponent<Collider>();
	}

	private void Update()
	{
		DebugUtils.DebugAssert(_totem != null);
		if (_collider.bounds.Intersects(GetComponent<Collider>().bounds))
		{
			_totemInZone = true;
			_totem.effectZone = this;
			return;
		}
		if (_totem.effectZone == this)
		{
			_totem.effectZone = null;
		}
		_totemInZone = false;
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawCube(base.transform.position + new Vector3(0f, 0.5f, 0f), new Vector3(1f, 1f, 1f));
	}
}
