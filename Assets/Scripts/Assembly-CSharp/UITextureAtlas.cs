using UnityCommon;
using UnityEngine;

public class UITextureAtlas : MonoBehaviour
{
	public string resourceName;

	[Tooltip("If set, this atlas acts as a proxy for the reference atlas")]
	public UITextureAtlas referenceAtlas;

	private TextAsset _atlasMetadata;

	private Material _atlasMaterial;

	private TextureAtlas _atlas = new TextureAtlas();

	public TextureAtlas atlas => _atlas;

	public TextAsset atlasMetadata => _atlasMetadata;

	public Material atlasMaterial => _atlasMaterial;

	public void Clear()
	{
		_atlasMetadata = null;
		_atlasMaterial = null;
		_atlas = new TextureAtlas();
	}

	public void Load()
	{
		if ((bool)referenceAtlas)
		{
			referenceAtlas.Load();
			_atlas = referenceAtlas._atlas;
			_atlasMetadata = referenceAtlas._atlasMetadata;
			_atlasMaterial = referenceAtlas._atlasMaterial;
		}
		else if (_atlasMetadata == null)
		{
			UICamera component = GameObject.Find("UICamera").GetComponent<UICamera>();
			component.EnsureScreenArchetypeSelected();
			string text = "UI/" + resourceName;
			string text2 = text + "2x";
			string path = text;
			if ((UICamera.IsTablet() && component.scale >= 2) || OrientationOverrideManager.IsLandscape())
			{
				path = text2;
			}
			_atlasMetadata = Resources.Load<TextAsset>(path);
			if (_atlasMetadata == null)
			{
				path = text;
				_atlasMetadata = Resources.Load<TextAsset>(path);
			}
			_atlasMaterial = Resources.Load<Material>(text + "_mat");
			Texture texture = Resources.Load<Texture>(path);
			if (texture != null)
			{
				_atlasMaterial.mainTexture = texture;
			}
			DebugUtils.DebugAssert(_atlasMetadata);
			_atlas.LoadFromTextAsset(_atlasMetadata);
		}
	}

	private void Awake()
	{
		Load();
	}

	private void Update()
	{
	}
}
