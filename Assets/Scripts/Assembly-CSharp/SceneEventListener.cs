public class SceneEventListener : TriggerableActionSequence
{
	public SceneEvent eventName = SceneEvent.QuitLevel;

	private void Start()
	{
		GameScene.instance.eventHandlers[eventName].EventReceived += OnEvent;
	}

	private void OnEvent()
	{
		TriggerActions();
	}
}
