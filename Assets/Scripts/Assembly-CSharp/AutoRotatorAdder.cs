using System.Collections;
using UnityEngine;

public class AutoRotatorAdder : MonoBehaviour
{
	public float deltaRotationX;

	public float deltaRotationY;

	public float deltaRotationZ;

	private float targetRotationX;

	private float targetRotationY;

	private float targetRotationZ;

	private float _startRotationX;

	private float _startRotationY;

	private float _startRotationZ;

	public float snapDuration = 0.6f;

	private float dragCoefficient;

	private float velocityX;

	private float velocityY;

	private float velocityZ;

	public bool snapping;

	public bool reverting;

	private void Start()
	{
		_startRotationX = base.transform.eulerAngles.x;
		_startRotationY = base.transform.eulerAngles.y;
		_startRotationZ = base.transform.eulerAngles.z;
	}

	[TriggerableAction]
	public IEnumerator DoTrigger()
	{
		Trigger();
		return null;
	}

	public void Trigger()
	{
		if (!snapping)
		{
			GameScene.navManager.NotifyReconfigurationBegan(base.gameObject);
			targetRotationX = base.transform.eulerAngles.x;
			targetRotationY = base.transform.eulerAngles.y;
			targetRotationZ = base.transform.eulerAngles.z;
			targetRotationX += deltaRotationX;
			targetRotationY += deltaRotationY;
			targetRotationZ += deltaRotationZ;
		}
		snapping = true;
	}

	private void Update()
	{
		if (snapping)
		{
			float x = base.transform.eulerAngles.x;
			float z = base.transform.eulerAngles.z;
			float y = base.transform.eulerAngles.y;
			if (Mathf.Abs(Mathf.DeltaAngle(x, targetRotationX)) < 0.2f && Mathf.Abs(Mathf.DeltaAngle(y, targetRotationY)) < 0.2f && Mathf.Abs(Mathf.DeltaAngle(z, targetRotationZ)) < 0.2f)
			{
				base.transform.eulerAngles = new Vector3(targetRotationX, targetRotationY, targetRotationZ);
				snapping = false;
				GameScene.navManager.NotifyReconfigurationEnded();
			}
			else
			{
				float x2 = Mathf.SmoothDampAngle(x, targetRotationX, ref velocityX, snapDuration);
				float y2 = Mathf.SmoothDampAngle(y, targetRotationY, ref velocityY, snapDuration);
				float z2 = Mathf.SmoothDampAngle(z, targetRotationZ, ref velocityZ, snapDuration);
				base.transform.eulerAngles = new Vector3(x2, y2, z2);
			}
		}
		else if (reverting)
		{
			float x3 = base.transform.eulerAngles.x;
			float z3 = base.transform.eulerAngles.z;
			float y3 = base.transform.eulerAngles.y;
			if (Mathf.Abs(Mathf.DeltaAngle(x3, _startRotationX)) < 0.2f && Mathf.Abs(Mathf.DeltaAngle(y3, _startRotationY)) < 0.2f && Mathf.Abs(Mathf.DeltaAngle(z3, _startRotationZ)) < 0.2f)
			{
				base.transform.eulerAngles = new Vector3(_startRotationX, _startRotationY, _startRotationZ);
				reverting = false;
				GameScene.navManager.NotifyReconfigurationEnded();
			}
			else
			{
				float x4 = Mathf.SmoothDampAngle(x3, _startRotationX, ref velocityX, snapDuration);
				float y4 = Mathf.SmoothDampAngle(y3, _startRotationY, ref velocityY, snapDuration);
				float z4 = Mathf.SmoothDampAngle(z3, _startRotationZ, ref velocityZ, snapDuration);
				base.transform.eulerAngles = new Vector3(x4, y4, z4);
			}
		}
	}
}
