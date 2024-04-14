using UnityCommon;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public class UIImage : UILayoutContent
{
	public UITextureAtlas atlas;

	public string subTextureName;

	public bool clampVertsToScreenPixels = true;

	private SubTextureInfo _subTexture;

	private Vector2 _quadSize = new Vector2(0f, 0f);

	public Color color = Color.white;

	private Color _quadColor = Color.white;

	public bool customMaterial;

	private float _opacity = 1f;

	private Vector2 _contentSize;

	private Color _originalColor;

	public bool canBeDimmedByButton = true;

	public bool rotateUVsInLandscape;

	public EditorButton updateMesh = new EditorButton("UpdateMesh");

	private MeshRenderer _renderer;

	private MeshFilter _meshFilter;

	private UILayout _layout;

	private Vector3[] _vertices = new Vector3[4];

	private Color[] _colors = new Color[4];

	private Vector2[] _uvs = new Vector2[4];

	public Color originalColor => _originalColor;

	public SubTextureInfo subTexture
	{
		get
		{
			if (_subTexture == null && atlas != null && subTextureName.Length > 0)
			{
				if (atlas.atlas.subTextures.ContainsKey(subTextureName))
				{
					_subTexture = atlas.atlas.subTextures[subTextureName];
					_contentSize = _subTexture.size / atlas.atlas.scale;
				}
				else
				{
					_subTexture = null;
					_contentSize = Vector2.zero;
				}
			}
			return _subTexture;
		}
		set
		{
			_subTexture = value;
			if (_subTexture != null)
			{
				_contentSize = _subTexture.size / atlas.atlas.scale;
			}
			else
			{
				_contentSize = Vector3.zero;
			}
			UpdateMesh(updatePosition: false, updateColor: false, updateUVS: true);
		}
	}

	public override Vector2 contentSize => _contentSize;

	public override Vector2 size
	{
		get
		{
			return _quadSize;
		}
		set
		{
			if (_quadSize != value)
			{
				_quadSize = value;
				UpdateMesh(updatePosition: true, updateColor: false, updateUVS: false);
			}
		}
	}

	public override float opacity
	{
		set
		{
			if (_opacity != value)
			{
				_opacity = value;
				UpdateMesh(updatePosition: false, updateColor: true, updateUVS: false);
			}
		}
	}

	public void SetSubTextureName(string name)
	{
		subTextureName = name;
		if ((bool)atlas)
		{
			subTexture = atlas.atlas.subTextures[name];
		}
	}

	private void Awake()
	{
		_originalColor = color;
	}

	private void Start()
	{
		_renderer = GetComponent<MeshRenderer>();
		_meshFilter = GetComponent<MeshFilter>();
		base.gameObject.GetComponent<MeshFilter>().sharedMesh = null;
		UpdateMesh(updatePosition: true, updateColor: true, updateUVS: true);
	}

	public void UpdateMesh(bool updatePosition, bool updateColor, bool updateUVS)
	{
		if (_renderer == null)
		{
			_renderer = GetComponent<MeshRenderer>();
		}
		if (_meshFilter == null)
		{
			_meshFilter = GetComponent<MeshFilter>();
		}
		if (null == _meshFilter.sharedMesh)
		{
			_meshFilter.sharedMesh = new Mesh();
			_meshFilter.sharedMesh.name = subTextureName + base.name;
			_meshFilter.sharedMesh.vertices = _vertices;
			_meshFilter.sharedMesh.triangles = new int[6] { 0, 1, 2, 2, 1, 3 };
		}
		Mesh sharedMesh = _meshFilter.sharedMesh;
		if (updatePosition)
		{
			_vertices[0].x = (0f - _quadSize.x) / 2f;
			_vertices[0].y = (0f - _quadSize.y) / 2f;
			_vertices[1].x = _quadSize.x / 2f;
			_vertices[1].y = (0f - _quadSize.y) / 2f;
			_vertices[2].x = (0f - _quadSize.x) / 2f;
			_vertices[2].y = _quadSize.y / 2f;
			_vertices[3].x = _quadSize.x / 2f;
			_vertices[3].y = _quadSize.y / 2f;
			if (clampVertsToScreenPixels && (bool)_layout && _subTexture != null)
			{
				UICamera rootLayout = _layout.rootLayout;
				Transform transform = base.transform;
				for (int i = 0; i < _vertices.Length; i++)
				{
					Vector3 worldPos = transform.TransformPoint(_vertices[i]);
					worldPos = rootLayout.ClampWorldSpacePointToScreenPixels(worldPos);
					_vertices[i] = transform.InverseTransformPoint(worldPos);
				}
			}
			sharedMesh.vertices = _vertices;
		}
		if (updateColor)
		{
			_quadColor = color;
			_quadColor.a *= _opacity;
			Color quadColor = _quadColor;
			_colors[0] = quadColor;
			_colors[1] = quadColor;
			_colors[2] = quadColor;
			_colors[3] = quadColor;
			sharedMesh.colors = _colors;
		}
		if (updateUVS && subTexture != null)
		{
			_uvs[0] = new Vector2(_subTexture.uvRect.xMin, 1f - _subTexture.uvRect.yMax);
			_uvs[1] = new Vector2(_subTexture.uvRect.xMax, 1f - _subTexture.uvRect.yMax);
			_uvs[2] = new Vector2(_subTexture.uvRect.xMin, 1f - _subTexture.uvRect.yMin);
			_uvs[3] = new Vector2(_subTexture.uvRect.xMax, 1f - _subTexture.uvRect.yMin);
			if (rotateUVsInLandscape && OrientationOverrideManager.IsLandscape())
			{
				Vector2 vector = _uvs[0];
				_uvs[0] = _uvs[2];
				_uvs[2] = _uvs[3];
				_uvs[3] = _uvs[1];
				_uvs[1] = vector;
			}
			sharedMesh.uv = _uvs;
			if (!customMaterial)
			{
				_renderer.material = atlas.atlasMaterial;
			}
		}
		if (updatePosition)
		{
			sharedMesh.RecalculateBounds();
		}
		if (updateColor)
		{
			_renderer.enabled = _quadColor.a > 0f;
		}
	}

	private void Update()
	{
		if (_quadColor.a <= 0.003f)
		{
			_renderer.enabled = false;
		}
		else
		{
			_renderer.enabled = true;
		}
		Color color = this.color;
		color.a *= _opacity;
		if (color != _quadColor)
		{
			UpdateMesh(updatePosition: false, updateColor: true, updateUVS: false);
		}
	}

	public override void UpdateRect(UILayout layout)
	{
		_layout = layout;
		UpdateMesh(updatePosition: true, updateColor: false, updateUVS: false);
	}
}
