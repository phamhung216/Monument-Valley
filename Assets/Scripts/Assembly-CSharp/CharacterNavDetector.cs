using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CharacterNavDetector
{
	public CharacterLocomotion character;

	public GameObject mainParent;

	public List<GameObject> additionalParents;

	public TotemPole totem;

	public void Init(GameObject parent)
	{
		if (null == character)
		{
			GameScene.instance.EnsurePlayer();
			if ((bool)GameScene.player)
			{
				character = GameScene.player.GetComponent<CharacterLocomotion>();
			}
		}
		if (null == mainParent)
		{
			mainParent = parent;
		}
		if (null == totem)
		{
			totem = UnityEngine.Object.FindObjectOfType(typeof(TotemPole)) as TotemPole;
		}
	}

	public bool IsCharacterPresent()
	{
		if (!character.lastValidBrush)
		{
			return false;
		}
		if (character.lastValidBrush.transform.IsChildOf(mainParent.transform))
		{
			return true;
		}
		if (character.getTargetBrush().transform.IsChildOf(mainParent.transform))
		{
			return true;
		}
		if (additionalParents != null)
		{
			for (int i = 0; i < additionalParents.Count; i++)
			{
				GameObject gameObject = additionalParents[i];
				if (gameObject != null)
				{
					if (character.lastValidBrush.transform.IsChildOf(gameObject.transform))
					{
						return true;
					}
					if (character.getTargetBrush().transform.IsChildOf(gameObject.transform))
					{
						return true;
					}
				}
			}
		}
		if (totem != null && totem.lastValidBrush != null && character.lastValidBrush.transform.IsChildOf(totem.transform))
		{
			if (totem.lastValidBrush.transform.IsChildOf(mainParent.transform))
			{
				return true;
			}
			for (int j = 0; j < additionalParents.Count; j++)
			{
				GameObject gameObject2 = additionalParents[j];
				if (totem.lastValidBrush.transform.IsChildOf(gameObject2.transform))
				{
					return true;
				}
			}
		}
		return false;
	}

	public void AddParent(GameObject parent)
	{
		if (!additionalParents.Contains(parent))
		{
			additionalParents.Add(parent);
		}
	}

	public void RemoveParent(GameObject parent)
	{
		if (additionalParents.Contains(parent))
		{
			additionalParents.Remove(parent);
		}
	}
}
