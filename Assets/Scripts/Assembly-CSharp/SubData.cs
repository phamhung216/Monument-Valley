using System.Xml.Serialization;
using UnityEngine;

public class SubData
{
	[XmlArray("Data")]
	[XmlArrayItem("point")]
	public Vector3[] Positons;
}
