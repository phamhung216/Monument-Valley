public class SceneEventHandler
{
	public delegate void EventHandler();

	public event EventHandler EventReceived;

	public void Send()
	{
		if (this.EventReceived != null)
		{
			this.EventReceived();
		}
	}
}
