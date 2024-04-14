using System;
using System.Collections;

public class MultiLampTrigger : TriggerItem
{
	public Lamp[] lamps;

	public bool pollForTrigger;

	private bool _triggered;

	public ActionSequence actions = new ActionSequence();

	private void Update()
	{
		if (pollForTrigger)
		{
			CheckNow();
		}
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
		catch (Exception)
		{
			D.Error("Trigger " + base.name + " Coroutine threw exception", base.gameObject);
		}
		_triggered = true;
	}

	[TriggerableAction]
	public IEnumerator DoTrigger()
	{
		if (!_triggered)
		{
			Trigger();
		}
		return null;
	}

	[TriggerableAction]
	public IEnumerator CheckNow()
	{
		if (lamps.Length != 0 && !_triggered)
		{
			bool flag = false;
			Lamp[] array = lamps;
			for (int i = 0; i < array.Length; i++)
			{
				if (!array[i].lit)
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
}
