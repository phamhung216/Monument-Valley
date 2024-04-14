using UnityEngine;

public class ScreenSpaceDragger : Dragger
{
	private Vector3 startPosition;

	private Vector3 dragPosition;

	private Vector3 targetPosition;

	private Vector3 offset;

	private float depth;

	private DraggerCavity[] allCavitys;

	private NavBrushComponent[] childNavs;

	public GameObject shadow;

	private DraggerCavity currentCavity;

	private CharacterLocomotion _player;

	private bool _dragAllowed;

	public override GameObject targetObject => base.gameObject;

	private void Start()
	{
		depth = Vector3.Dot(base.transform.position - Camera.main.transform.position, Camera.main.transform.forward);
		targetPosition = base.transform.position;
		allCavitys = Object.FindObjectsOfType(typeof(DraggerCavity)) as DraggerCavity[];
		childNavs = base.gameObject.GetComponentsInChildren<NavBrushComponent>();
		_player = Object.FindObjectOfType(typeof(CharacterLocomotion)) as CharacterLocomotion;
	}

	private bool IsCharacterOnDragger()
	{
		NavBrushComponent[] array = childNavs;
		foreach (NavBrushComponent navBrushComponent in array)
		{
			if (_player.lastValidBrush == navBrushComponent || _player.getTargetBrush() == navBrushComponent)
			{
				return true;
			}
		}
		return false;
	}

	public override void StartDrag(Vector3 position)
	{
		if (_dragAllowed)
		{
			base.StartDrag(position);
			startPosition = base.transform.position;
			offset = Camera.main.WorldToScreenPoint(base.gameObject.transform.position) - position;
			dragPosition = startPosition;
			targetPosition = startPosition;
			shadow.SetActive(value: true);
			if (currentCavity != null)
			{
				currentCavity.spaceUsed = false;
			}
			currentCavity = null;
		}
	}

	public override void Drag(Vector3 position, Vector3 delta)
	{
		if (_dragAllowed)
		{
			_snapping = false;
			_dragging = true;
			Vector3 vector = Camera.main.ScreenToWorldPoint(position) + (depth - 10f) * Camera.main.transform.forward;
			targetPosition = vector;
		}
	}

	public override void Snap()
	{
		if (!_dragAllowed)
		{
			return;
		}
		_dragging = false;
		_snapping = true;
		DraggerCavity[] array = allCavitys;
		foreach (DraggerCavity draggerCavity in array)
		{
			if (!draggerCavity.spaceUsed)
			{
				Vector3 vector = draggerCavity.transform.position - Camera.main.transform.forward * Vector3.Dot(Camera.main.transform.forward, draggerCavity.transform.position);
				Vector3 vector2 = targetPosition - Camera.main.transform.forward * Vector3.Dot(Camera.main.transform.forward, targetPosition);
				if ((vector - vector2).sqrMagnitude < 2f)
				{
					targetPosition = vector + Camera.main.transform.position;
					targetPosition += Camera.main.transform.forward * depth;
					currentCavity = draggerCavity;
					currentCavity.spaceUsed = true;
					shadow.SetActive(value: false);
					break;
				}
			}
		}
		EndSnapping();
	}

	private void Update()
	{
		_dragAllowed = !IsCharacterOnDragger();
		base.transform.position = targetPosition;
	}
}
