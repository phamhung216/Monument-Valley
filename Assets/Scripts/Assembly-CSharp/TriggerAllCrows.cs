public class TriggerAllCrows : GameTouchable
{
	public AIController[] AIToTrigger;

	public bool initialState = true;

	private void Start()
	{
	}

	private void Update()
	{
	}

	public void StopAllCrows()
	{
		for (int i = 0; i < AIToTrigger.Length; i++)
		{
			AIToTrigger[i].ForceIdle = true;
		}
		initialState = true;
	}

	public override void OnTouchEnded(GameTouch touch)
	{
		if (initialState)
		{
			for (int i = 0; i < AIToTrigger.Length; i++)
			{
				AIToTrigger[i].ForceIdle = false;
			}
			initialState = false;
			return;
		}
		for (int j = 0; j < AIToTrigger.Length; j++)
		{
			AIToTrigger[j].ForceIdle = true;
			AIToTrigger[j].ResetToStart();
		}
		initialState = true;
	}
}
