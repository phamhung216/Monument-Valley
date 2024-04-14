using UnityCommon;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(TextMesh))]
public class UIText : UILayoutContent
{
	public enum LocalisationMode
	{
		Localise = 0,
		UseDefault = 1
	}

	public enum CaseMode
	{
		Normal = 0,
		UpperCase = 1,
		LowerCase = 2
	}

	public Color color = Color.white;

	public Interpolation interp;

	public LocalisationMode localisationMode;

	private TextMesh _textMesh;

	private Material _material;

	private float _opacity = 1f;

	private Vector2 _contentSize;

	private Font _initialFont;

	private float _fontSize;

	private float _characterSize;

	[Multiline]
	public string content;

	public CaseMode caseMode;

	public bool insertSpaces;

	public bool isTitle;

	private Renderer _renderer;

	private Color _originalColor;

	private Color _lastSetMaterialColour;

	private const string MATERIAL_COLOR_NAME = "_Color";

	public Color originalColor => _originalColor;

	public override Vector2 size
	{
		get
		{
			return Vector2.zero;
		}
		set
		{
		}
	}

	public override float opacity
	{
		set
		{
			if (_opacity != value)
			{
				_opacity = value;
				UpdateColor();
			}
		}
	}

	public string text
	{
		get
		{
			return content;
		}
		set
		{
			if (content != value)
			{
				content = value;
				UpdateTextMesh();
			}
		}
	}

	public override Vector2 contentSize => _contentSize;

	private void Awake()
	{
		_textMesh = GetComponent<TextMesh>();
		_initialFont = _textMesh.font;
		_fontSize = _textMesh.fontSize;
		_characterSize = _textMesh.characterSize;
		_renderer = base.gameObject.GetComponent<Renderer>();
		_material = _renderer.material;
		_lastSetMaterialColour = _material.GetColor("_Color");
		_contentSize = Vector2.zero;
		_originalColor = color;
	}

	private void Start()
	{
		UpdateFont();
		UpdateTextMesh();
	}

	private void UpdateFont()
	{
		if ((!isTitle || LocalisationManager.Instance.fontInfo.localiseTitles) && localisationMode != LocalisationMode.UseDefault && (bool)_initialFont)
		{
			_textMesh.font = LocalisationManager.Instance.LocaliseFont(_initialFont);
		}
		if ((bool)_renderer)
		{
			_renderer.material = _textMesh.font.material;
			_material = _renderer.material;
		}
	}

	private void UpdateTextMesh()
	{
		string text = ((localisationMode != 0) ? content : LocalisationManager.Instance.LocaliseString(content));
		if (CaseMode.UpperCase == caseMode)
		{
			text = text.ToUpper();
		}
		else if (CaseMode.UpperCase == caseMode)
		{
			text = text.ToLower();
		}
		int num = Mathf.RoundToInt(_fontSize);
		LocalisedFonts localisedFonts = null;
		localisedFonts = ((localisationMode != LocalisationMode.UseDefault) ? LocalisationManager.Instance.fontInfo : LocalisationManager.Instance.defaultFontInfo);
		if ((!isTitle || LocalisationManager.Instance.fontInfo.localiseTitles) && localisedFonts != null)
		{
			num = Mathf.RoundToInt(_fontSize * localisedFonts.scale);
		}
		int num2 = 1;
		UICamera uICamera = Object.FindObjectOfType<UICamera>();
		if ((UICamera.IsTablet() && uICamera.scale == 2) || OrientationOverrideManager.IsLandscape())
		{
			num2 = 2;
		}
		_textMesh.fontSize = num2 * num;
		_textMesh.characterSize = _characterSize / (float)num2;
		_textMesh.text = text;
		if (_renderer != null)
		{
			_contentSize = _renderer.bounds.max - _renderer.bounds.min;
		}
	}

	public void SetText(string key)
	{
		content = key;
		if ((bool)_textMesh)
		{
			UpdateTextMesh();
		}
	}

	private void Update()
	{
		UpdateColor();
	}

	private void UpdateColor()
	{
		Color color = this.color;
		color.a *= _opacity;
		if ((bool)interp)
		{
			color.a *= interp.interpAmount;
		}
		color = (Color32)color;
		if (!(_lastSetMaterialColour == color))
		{
			if (_material != null)
			{
				_material.SetColor("_Color", color);
				_lastSetMaterialColour = color;
			}
			if ((bool)_renderer)
			{
				_renderer.enabled = color.a > 0f;
			}
		}
	}

	public override void UpdateRect(UILayout layout)
	{
		if ((bool)_textMesh)
		{
			Vector3 zero = Vector3.zero;
			switch (_textMesh.anchor)
			{
			case TextAnchor.LowerLeft:
			case TextAnchor.LowerCenter:
			case TextAnchor.LowerRight:
				zero.y = -0.5f * layout.layoutHeight;
				break;
			case TextAnchor.UpperLeft:
			case TextAnchor.UpperCenter:
			case TextAnchor.UpperRight:
				zero.y = 0.5f * layout.layoutHeight;
				break;
			}
			switch (_textMesh.anchor)
			{
			case TextAnchor.UpperLeft:
			case TextAnchor.MiddleLeft:
			case TextAnchor.LowerLeft:
				zero.x = -0.5f * layout.layoutWidth;
				break;
			case TextAnchor.UpperRight:
			case TextAnchor.MiddleRight:
			case TextAnchor.LowerRight:
				zero.x = 0.5f * layout.layoutWidth;
				break;
			}
			base.transform.localPosition = zero;
		}
	}

	public override void OnLanguageChanged()
	{
		UpdateFont();
		if ((bool)_textMesh)
		{
			UpdateTextMesh();
		}
	}
}
