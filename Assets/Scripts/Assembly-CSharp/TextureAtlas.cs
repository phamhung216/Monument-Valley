using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class TextureAtlas
{
	private int _scale = 1;

	private string _name;

	private Dictionary<string, SubTextureInfo> _textures = new Dictionary<string, SubTextureInfo>();

	public string name => _name;

	public Dictionary<string, SubTextureInfo> subTextures => _textures;

	public int scale => _scale;

	public void LoadFromTextAsset(TextAsset asset)
	{
		_name = asset.name;
		Hashtable hashtable = asset.text.hashtableFromJson();
		IDictionary dictionary = (IDictionary)hashtable["frames"];
		IDictionary dictionary2 = (IDictionary)hashtable["meta"];
		IDictionary dictionary3 = (IDictionary)dictionary2["size"];
		try
		{
			float num = float.Parse(dictionary2["scale"].ToString(), CultureInfo.InvariantCulture);
			_scale = Mathf.RoundToInt(2f * num);
		}
		catch (FormatException)
		{
			_scale = 1;
		}
		_scale = Mathf.Max(_scale, 1);
		Vector2 vector = new Vector2(int.Parse(dictionary3["w"].ToString()), int.Parse(dictionary3["h"].ToString()));
		foreach (DictionaryEntry item in dictionary)
		{
			IDictionary obj = (IDictionary)((IDictionary)item.Value)["frame"];
			int num2 = int.Parse(obj["x"].ToString());
			int num3 = int.Parse(obj["y"].ToString());
			int num4 = int.Parse(obj["w"].ToString());
			int num5 = int.Parse(obj["h"].ToString());
			SubTextureInfo subTextureInfo = new SubTextureInfo();
			subTextureInfo.uvRect = new Rect((float)num2 / vector.x, (float)num3 / vector.y, (float)num4 / vector.x, (float)num5 / vector.y);
			subTextureInfo.size = new Vector2(num4, num5);
			_textures.Add(item.Key.ToString(), subTextureInfo);
		}
	}
}
