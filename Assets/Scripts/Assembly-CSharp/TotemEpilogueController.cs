using System.Collections;
using UnityEngine;

public class TotemEpilogueController : MonoBehaviour
{
	public Animation anim;

	[TriggerableAction]
	public IEnumerator VibrateBlend()
	{
		anim.Play("IdleNoRotation");
		anim.CrossFade("Vibrating", 3f);
		return null;
	}
}
