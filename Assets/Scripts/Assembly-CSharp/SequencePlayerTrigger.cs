using System.Collections.Generic;
using UnityEngine;

public class SequencePlayerTrigger : TriggerItem
{
	public List<AutoMover> triggerItems;

	public Transform player;

	private bool triggered;

	public float delay = 0.02f;

	private Collider _collider;

	private void Start()
	{
		_collider = GetComponent<Collider>();
	}

	private void Update()
	{
		if (!triggered && _collider.bounds.Contains(player.position))
		{
			Trigger();
		}
		else if (triggered && !_collider.bounds.Contains(player.position))
		{
			triggered = false;
		}
	}

	public override void Trigger()
	{
		triggered = true;
		PlaySound();
		PlayAnimation();
	}

	public virtual void PlaySound()
	{
	}

	public virtual void PlayAnimation()
	{
	}
}
