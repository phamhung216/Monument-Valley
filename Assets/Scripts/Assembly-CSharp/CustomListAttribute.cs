using System;
using UnityEngine;

public class CustomListAttribute : PropertyAttribute
{
	private Type _type;

	public Type type => _type;

	public CustomListAttribute(Type type)
	{
		_type = type;
	}
}
