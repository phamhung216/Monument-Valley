using System;

[Serializable]
public class RestrictiveNavZone
{
	public int zone;

	[BitMaskField(typeof(NavAccessFlags))]
	public NavAccessFlags accessors;
}
