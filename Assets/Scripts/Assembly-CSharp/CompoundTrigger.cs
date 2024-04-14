using System.Collections.Generic;

public class CompoundTrigger : ProximityTrigger
{
	public enum Operation
	{
		And = 0,
		Or = 1
	}

	public List<ProximityTrigger> triggers;

	public Operation operation;

	private new void Start()
	{
	}

	private new void Update()
	{
		bool flag = false;
		for (int i = 0; i < triggers.Count; i++)
		{
			switch (operation)
			{
			case Operation.And:
				flag &= triggers[i].triggered;
				break;
			case Operation.Or:
				flag |= triggers[i].triggered;
				break;
			}
		}
		if (flag != base.triggered && flag)
		{
			Trigger();
		}
		if (isMultiTrigger && base.triggered && !flag)
		{
			UnTrigger();
		}
	}
}
