using System.Collections;
using UnityEngine;

public class WaterWheelParticlePlayer : MonoBehaviour
{
	public ParticleSystem target;

	public WaterSection waterSection;

	public float angleOffs;

	public int idx;

	public float angleRange;

	public Transform wheel;

	private bool _playing;

	[TriggerableAction]
	public IEnumerator PlayParticle()
	{
		target.Play();
		return null;
	}

	[TriggerableAction]
	public IEnumerator StopParticle()
	{
		target.Stop();
		return null;
	}

	private void Update()
	{
		if (!waterSection)
		{
			return;
		}
		bool flag = waterSection.hasInFlow();
		float num = angleOffs - (float)idx * 360f / 16f;
		Vector3 vector = Vector3.Cross(Vector3.up, wheel.up);
		float num2 = Vector3.Dot(Vector3.up, wheel.up);
		float num3 = 57.29578f * Mathf.Asin(vector.z);
		if (num2 < 0f)
		{
			num3 = 180f - num3;
		}
		if (num3 < 0f)
		{
			num3 = 360f + num3;
		}
		flag &= Mathf.DeltaAngle(num3, num) < 0f && 0f < Mathf.DeltaAngle(num3, num + angleRange);
		if (target.isPlaying)
		{
			if (!flag)
			{
				target.Stop();
			}
		}
		else if (flag)
		{
			target.Play();
		}
		_playing = flag;
	}
}
