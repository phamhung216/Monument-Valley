using UnityEngine;

public class FinaleDraggersLogic : MonoBehaviour
{
	public AxisDragger draggerA;

	public AxisDragger draggerB;

	public AxisDragger stairsDragger;

	private void Update()
	{
		if (!draggerA.dragging && !draggerA.snapping && !draggerB.dragging && !draggerB.snapping && draggerA.AtMinimum() && draggerB.AtMinimum())
		{
			if (!stairsDragger.targetDraggable.dragEnabled)
			{
				stairsDragger.targetDraggable.EnableDrag();
			}
		}
		else if (stairsDragger.targetDraggable.dragEnabled)
		{
			stairsDragger.targetDraggable.DisableDrag();
		}
	}
}
