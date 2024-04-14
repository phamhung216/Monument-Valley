using System.Xml.Serialization;

public class EmissionData
{
	[XmlArray("Data")]
	[XmlArrayItem("subData")]
	public SubData[] subData;
}
