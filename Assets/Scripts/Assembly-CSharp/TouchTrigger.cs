using UnityEngine;

public class TouchTrigger : GameTouchable
{
	public Animation targetAnimation;

	public ActionSequence actions = new ActionSequence();

	public bool repeatable;

	private bool triggered;

	private void Update()
	{
	}

	public override void OnTouchEnded(GameTouch touch)
	{
		if (!triggered)
		{
			Trigger();
			if ((bool)targetAnimation)
			{
				targetAnimation.Play();
			}
		}
	}

	public void Trigger()
	{
		if (!repeatable)
		{
			triggered = true;
		}
		StartCoroutine(actions.DoSequence());
	}
}
