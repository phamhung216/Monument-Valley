using System;
using System.Collections;

public class MultiTrigger : TriggerItem
{
	public ProximityTrigger[] triggers;

	private bool _triggered;

	public ActionSequence actions = new ActionSequence();

	public bool pollForTrigger = true;

	private void Update()
	{
		if (pollForTrigger)
		{
			CheckNow();
		}
	}

	[TriggerableAction]
	public IEnumerator CheckNow()
	{
		if (triggers.Length != 0 && !_triggered)
		{
			bool flag = false;
			ProximityTrigger[] array = triggers;
			for (int i = 0; i < array.Length; i++)
			{
				if (!array[i].triggered)
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				Trigger();
			}
		}
		return null;
	}

	public override void Trigger()
	{
		AnalyticsTrigger component = GetComponent<AnalyticsTrigger>();
		if (null != component)
		{
			component.SendAnalyticsEvent();
		}
		try
		{
			StartCoroutine(actions.DoSequence());
		}
		catch (Exception ex)
		{
			D.Error("Trigger " + base.name + " Coroutine threw exception " + ex, base.gameObject);
		}
		_triggered = true;
	}
}
