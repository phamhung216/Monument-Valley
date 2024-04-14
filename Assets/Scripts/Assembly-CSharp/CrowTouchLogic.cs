public class CrowTouchLogic : GameTouchable
{
	public AIController crow;

	private void Start()
	{
		crow = GetComponent<AIController>();
	}

	private void Update()
	{
	}

	public override bool CanOverRideNonPhysicals()
	{
		return true;
	}

	public override bool AcceptTouch(GameTouch touch)
	{
		return true;
	}

	public override void OnTouchBegan(GameTouch touch)
	{
		crow.TriggerScream();
	}
}
