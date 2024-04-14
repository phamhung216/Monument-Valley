using System.Xml.Serialization;

[XmlRoot("TouchesXML")]
public class TouchesXML
{
	[XmlArray("Touches")]
	[XmlArrayItem("Touch")]
	public LoggedTouch[] touches;

	public int screenWidth;

	public int screenHeight;

	public void Save(string path)
	{
	}

	public static TouchesXML Load(string path)
	{
		return null;
	}

	public static TouchesXML LoadFromString(string xml)
	{
		return null;
	}
}
