public class InteractiveItem : GameTouchable
{
	public InteractiveItem()
	{
		claimOnTouchBegan = true;
		claimOnTouchNotTap = false;
		releaseOnTouchNotTap = true;
	}

	public override void OnTouchEnded(GameTouch touch)
	{
		Trigger();
	}

	public virtual void Trigger()
	{
		PlaySound();
		PlayAnimation();
	}

	public virtual void PlaySound()
	{
	}

	public virtual void PlayAnimation()
	{
	}
}
