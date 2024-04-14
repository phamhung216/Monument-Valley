using System.Collections;
using UnityEngine;

[RequireComponent(typeof(MoverAudio))]
public class Draggable : MonoBehaviour
{
	public bool lockWhenCharacterPresent = true;

	public CharacterNavDetector characterDetector = new CharacterNavDetector();

	public bool dragEnabled = true;

	protected bool _snapping;

	protected bool _dragging;

	public bool dragging => _dragging;

	public bool snapping => _snapping;

	public bool isStationary
	{
		get
		{
			if (!_snapping)
			{
				return !_dragging;
			}
			return false;
		}
	}

	protected void Awake()
	{
	}

	protected void Start()
	{
		characterDetector.Init(base.gameObject);
	}

	public virtual void StartDrag()
	{
		_dragging = true;
		_snapping = false;
		GameScene.navManager.NotifyReconfigurationBegan(base.gameObject);
	}

	public virtual void Snap()
	{
		_ = _snapping;
		_dragging = false;
		_snapping = true;
	}

	public virtual void EndSnapping()
	{
		_snapping = false;
		GameScene.navManager.NotifyReconfigurationEnded();
	}

	[TriggerableAction]
	public IEnumerator EnableDrag()
	{
		dragEnabled = true;
		Collider component = GetComponent<Collider>();
		if ((bool)component)
		{
			component.enabled = true;
		}
		return null;
	}

	[TriggerableAction]
	public IEnumerator DisableDrag()
	{
		dragEnabled = false;
		Collider component = GetComponent<Collider>();
		if ((bool)component)
		{
			component.enabled = false;
		}
		return null;
	}

	public bool AllowDrag()
	{
		if (!dragEnabled)
		{
			return false;
		}
		if (lockWhenCharacterPresent && characterDetector.IsCharacterPresent())
		{
			return false;
		}
		return true;
	}
}
