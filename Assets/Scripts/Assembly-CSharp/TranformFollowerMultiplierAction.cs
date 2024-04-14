using System.Collections;
using UnityEngine;

[RequireComponent(typeof(TransformFollower))]
public class TranformFollowerMultiplierAction : MonoBehaviour
{
	[SerializeField]
	private float _multiplier = 1f;

	private TransformFollower _follower;

	private void Start()
	{
		_follower = GetComponent<TransformFollower>();
	}

	[TriggerableAction]
	public IEnumerator SetMultiplierToTransformFollow()
	{
		_follower.multiplier = _multiplier;
		return null;
	}
}
