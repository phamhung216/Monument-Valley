using System;

public class TriggerableActionAttribute : Attribute
{
	private bool _waitable;

	public bool waitable => _waitable;

	public TriggerableActionAttribute()
	{
		_waitable = false;
	}

	public TriggerableActionAttribute(bool waitable)
	{
		_waitable = waitable;
	}
}
