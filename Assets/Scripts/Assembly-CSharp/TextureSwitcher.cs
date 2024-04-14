using UnityEngine;

public class TextureSwitcher : MonoBehaviour
{
	public Texture[] allSkins;

	private int currentSkin;

	public Material skinMaterial;

	public void ChangeSkin()
	{
		currentSkin++;
		if (currentSkin == allSkins.Length)
		{
			currentSkin = 0;
		}
		skinMaterial.SetTexture("_MainTex", allSkins[currentSkin]);
	}
}
