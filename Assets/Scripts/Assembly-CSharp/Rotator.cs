using UnityEngine;

public class Rotator : Dragger
{
	public Rotatable target;

	public override GameObject targetObject
	{
		get
		{
			if (!target)
			{
				return null;
			}
			return target.gameObject;
		}
	}

	public float currentAngle => target.currentAngle;

	protected void Start()
	{
		if (target == null)
		{
			target = base.gameObject.GetComponent<Rotatable>();
		}
		DebugUtils.DebugAssert(target, base.gameObject);
	}

	protected virtual void Update()
	{
		if ((bool)target)
		{
			_snapping = target.snapping;
		}
	}

	public override void StartDrag(Vector3 position)
	{
		base.StartDrag(position);
	}

	public override void Snap()
	{
		base.Snap();
		target.Snap();
	}

	public override void EndSnapping()
	{
		base.EndSnapping();
	}
}
