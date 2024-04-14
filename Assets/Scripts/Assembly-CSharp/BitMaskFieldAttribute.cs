using System;
using UnityEngine;

public class BitMaskFieldAttribute : PropertyAttribute
{
	private Type _type;

	public Type type => _type;

	public BitMaskFieldAttribute(Type type)
	{
		_type = type;
	}
}
