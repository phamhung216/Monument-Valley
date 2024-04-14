using System.Collections;

public class AlternatorTrigger : ProximityTrigger
{
	public bool use_actions_B;

	public ActionSequence actions_A = new ActionSequence();

	public ActionSequence actions_B = new ActionSequence();

	public override void Trigger()
	{
		actions = (use_actions_B ? actions_B : actions_A);
		base.Trigger();
	}

	[TriggerableAction]
	public IEnumerator Use_A()
	{
		use_actions_B = false;
		return null;
	}

	[TriggerableAction]
	public IEnumerator Use_B()
	{
		use_actions_B = true;
		return null;
	}
}
