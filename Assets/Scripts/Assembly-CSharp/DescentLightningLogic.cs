using System.Collections;
using UnityEngine;

public class DescentLightningLogic : MonoBehaviour
{
	public bool isEnabled = true;

	public float minLightningWaitTime;

	public float maxLightningWaitTime;

	public float minThunderWaitTime;

	public float maxThunderWaitTime;

	public Animation lightningAnimation;

	public Interpolation interp;

	public Material lightningMat;

	public Renderer waterRenderer;

	public ActionSequence actions = new ActionSequence();

	private bool _isRunning;

	private void Start()
	{
		lightningMat = Object.Instantiate(lightningMat);
		_isRunning = false;
	}

	[TriggerableAction]
	public IEnumerator FinishedSequence()
	{
		_isRunning = false;
		return null;
	}

	[TriggerableAction(true)]
	public IEnumerator LightningStrike()
	{
		if (isEnabled)
		{
			lightningAnimation.Play();
			yield return new WaitForSeconds(lightningAnimation.clip.length);
		}
		yield return null;
	}

	[TriggerableAction(true)]
	public IEnumerator LightningWait()
	{
		if (isEnabled)
		{
			yield return new WaitForSeconds(Random.Range(minLightningWaitTime, maxLightningWaitTime));
		}
		yield return null;
	}

	[TriggerableAction(true)]
	public IEnumerator ThunderWait()
	{
		if (isEnabled)
		{
			yield return new WaitForSeconds(Random.Range(minThunderWaitTime, maxThunderWaitTime));
		}
		yield return null;
	}

	[TriggerableAction]
	public IEnumerator Disable()
	{
		isEnabled = false;
		return null;
	}

	[TriggerableAction]
	public IEnumerator Enable()
	{
		isEnabled = true;
		return null;
	}

	private void LateUpdate()
	{
		waterRenderer.material.SetFloat("_Lightning", interp.interpAmount);
		if (!_isRunning && isEnabled)
		{
			_isRunning = true;
			StartCoroutine(actions.DoSequence());
		}
	}
}
