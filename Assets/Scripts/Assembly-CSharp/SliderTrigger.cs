public class SliderTrigger : PlayerTrigger
{
	public RotatorSnapper rotator;

	public override void Trigger()
	{
		base.Trigger();
		rotator.limitRotation = true;
	}
}
