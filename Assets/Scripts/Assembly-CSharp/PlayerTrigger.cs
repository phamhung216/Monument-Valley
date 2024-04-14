using System.Collections;
using UnityEngine;

public class PlayerTrigger : TriggerItem
{
	public ActionSequence actions = new ActionSequence();

	public Transform player;

	public Transform[] triggeringEntities;

	private bool triggered;

	public bool isMultiTrigger;

	private static string s_iconTexture = "GizmoTrigger";

	private Collider _collider;

	public bool isTriggered => triggered;

	protected void Start()
	{
		if (player == null && (triggeringEntities == null || triggeringEntities.Length == 0))
		{
			player = GameScene.player.transform;
		}
		_collider = GetComponent<Collider>();
	}

	protected void Update()
	{
		bool flag = false;
		if (_collider.bounds.Contains(player.position))
		{
			if (!triggered)
			{
				Trigger();
			}
			flag = true;
		}
		if (triggeringEntities != null)
		{
			for (int i = 0; i < triggeringEntities.Length; i++)
			{
				if (_collider.bounds.Contains(triggeringEntities[i].position))
				{
					if (!triggered)
					{
						Trigger();
					}
					flag = true;
				}
			}
		}
		if (isMultiTrigger && triggered && !flag)
		{
			triggered = false;
		}
	}

	[TriggerableAction]
	public IEnumerator DoTrigger()
	{
		triggered = true;
		PlaySound();
		PlayAnimation();
		StartCoroutine(actions.DoSequence());
		return null;
	}

	public override void Trigger()
	{
		triggered = true;
		PlaySound();
		PlayAnimation();
		StartCoroutine(actions.DoSequence());
	}

	public virtual void PlaySound()
	{
	}

	public virtual void PlayAnimation()
	{
	}

	private void OnDrawGizmos()
	{
		Gizmos.DrawIcon(base.transform.position, s_iconTexture);
	}
}
