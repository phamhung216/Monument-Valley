using System.Collections;
using UnityEngine;

public class HourglassRotator : Dragger
{
	private float dragCoefficient;

	private float duration = 0.15f;

	private float targetRotationZ;

	private float velocityZ;

	private Animation anim;

	private TimeControl timeControl;

	private Vector3 lastPosition = Vector3.zero;

	public override GameObject targetObject => base.gameObject;

	private void Awake()
	{
		dragCoefficient = 0.01f;
		if (Screen.height > 1024)
		{
			dragCoefficient *= 0.5f;
		}
		anim = GetComponentInChildren<Animation>();
		anim["HourglassFlip"].time = anim["HourglassFlip"].length;
		anim.Play();
		timeControl = GameObject.Find("TimeControl").GetComponent<TimeControl>();
	}

	public override void Drag(Vector3 position, Vector3 delta)
	{
		_snapping = false;
		if (lastPosition != Vector3.zero)
		{
			Vector3 vector = Camera.main.WorldToScreenPoint(base.transform.position);
			float num = Mathf.Atan2(lastPosition.y - vector.y, lastPosition.x - vector.x);
			float num2 = Mathf.Atan2(position.y - vector.y, position.x - vector.x);
			base.transform.Rotate(Vector3.forward, num2 - num);
		}
		lastPosition = position;
	}

	public override void Snap()
	{
		base.Snap();
		lastPosition = Vector3.zero;
		targetRotationZ = Mathf.Round(base.transform.eulerAngles.z / 180f) * 180f;
	}

	private void Update()
	{
		if (!base.snapping)
		{
			return;
		}
		float x = base.transform.eulerAngles.x;
		float y = base.transform.eulerAngles.y;
		if (Mathf.Abs(targetRotationZ - base.transform.eulerAngles.z) < 1f)
		{
			if (targetRotationZ == 360f)
			{
				targetRotationZ = 0f;
			}
			base.transform.eulerAngles = new Vector3(x, y, targetRotationZ);
			EndSnapping();
			if (Mathf.Approximately(base.transform.eulerAngles.z, 180f))
			{
				base.transform.eulerAngles = Vector3.zero;
				StartCoroutine(PlayAnimation());
				timeControl.ToggleTimePeriod();
			}
		}
		else
		{
			float z = Mathf.SmoothDampAngle(base.transform.eulerAngles.z, targetRotationZ, ref velocityZ, duration);
			base.transform.eulerAngles = new Vector3(x, y, z);
		}
	}

	private IEnumerator PlayAnimation()
	{
		anim["HourglassFlip"].speed = 1f;
		anim["HourglassFlip"].time = 0f;
		anim.Play();
		yield return new WaitForSeconds(0.5f);
		anim["HourglassFlip"].speed = 0.18f;
	}
}
