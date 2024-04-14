using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EmissionSlice
{
	public List<Vector3> positions;

	public EmissionSlice()
	{
		positions = new List<Vector3>();
	}
}
