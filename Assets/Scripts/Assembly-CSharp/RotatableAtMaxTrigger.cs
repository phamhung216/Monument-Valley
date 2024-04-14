using System;

public class RotatableAtMaxTrigger : TriggerItem
{
	public Rotatable rot;

	private bool _triggered;

	public ActionSequence actions = new ActionSequence();

	private void Update()
	{
		if (!_triggered && rot.AtMaximum())
		{
			Trigger();
		}
	}

	public override void Trigger()
	{
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
