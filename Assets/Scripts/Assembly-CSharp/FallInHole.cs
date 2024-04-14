using System;
using UnityEngine;

public class FallInHole : MonoBehaviour
{
	public NavBrushComponent fromBrush;

	public NavBrushComponent toBrush;

	private TotemPole pole;

	private void Start()
	{
		pole = UnityEngine.Object.FindObjectOfType(typeof(TotemPole)) as TotemPole;
	}

	private void Update()
	{
		if (pole.lastValidBrush == fromBrush)
		{
			throw new Exception("Not supported");
		}
	}
}
