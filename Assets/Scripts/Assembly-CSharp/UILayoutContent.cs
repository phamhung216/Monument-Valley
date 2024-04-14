using UnityEngine;

public class UILayoutContent : MonoBehaviour
{
	public virtual Vector2 contentSize => Vector2.zero;

	public virtual Vector2 size
	{
		get
		{
			return Vector2.zero;
		}
		set
		{
		}
	}

	public virtual float opacity
	{
		set
		{
		}
	}

	public virtual void UpdateRect(UILayout layout)
	{
	}

	public virtual void OnLanguageChanged()
	{
	}
}
