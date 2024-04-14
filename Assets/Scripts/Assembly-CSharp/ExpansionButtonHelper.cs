using System;
using System.Collections;
using UnityEngine;

public class ExpansionButtonHelper : MonoBehaviour
{
	public bool expansion_Available = true;

	public bool redSuits_Available = true;

	public bool expansion_Unlocked;

	public bool redSuits_Unlocked;

	public bool expansion_UseExit;

	public ActionSequence enable_Expansion_Buy;

	public ActionSequence enable_RedSuits_Buy;

	public ActionSequence enable_Expansion_Enter;

	public ActionSequence enable_Expansion_Exit;

	public ActionSequence enable_RedSuits_Enter;

	[TriggerableAction]
	public IEnumerator CheckButtons()
	{
		if (expansion_UseExit)
		{
			try
			{
				StartCoroutine(enable_Expansion_Exit.DoSequence());
			}
			catch (Exception ex)
			{
				D.Error("ExpansionButtonHelper " + base.name + " Coroutine threw exception " + ex, base.gameObject);
			}
		}
		else if (expansion_Available)
		{
			if (expansion_Unlocked)
			{
				try
				{
					StartCoroutine(enable_Expansion_Enter.DoSequence());
				}
				catch (Exception ex2)
				{
					D.Error("ExpansionButtonHelper " + base.name + " Coroutine threw exception " + ex2, base.gameObject);
				}
			}
			else
			{
				try
				{
					StartCoroutine(enable_Expansion_Buy.DoSequence());
				}
				catch (Exception ex3)
				{
					D.Error("ExpansionButtonHelper " + base.name + " Coroutine threw exception " + ex3, base.gameObject);
				}
			}
		}
		if (redSuits_Available)
		{
			if (redSuits_Unlocked)
			{
				try
				{
					StartCoroutine(enable_RedSuits_Enter.DoSequence());
				}
				catch (Exception ex4)
				{
					D.Error("ExpansionButtonHelper " + base.name + " Coroutine threw exception " + ex4, base.gameObject);
				}
			}
			else
			{
				try
				{
					StartCoroutine(enable_RedSuits_Buy.DoSequence());
				}
				catch (Exception ex5)
				{
					D.Error("ExpansionButtonHelper " + base.name + " Coroutine threw exception " + ex5, base.gameObject);
				}
			}
		}
		return null;
	}
}
