using UnityCommon;
using UnityEngine;

public class GenerateWaterMesh : MonoBehaviour
{
	private Material _seaMaterial;

	public int width = 56;

	public int height = 56;

	private int numSections;

	private int numVerts;

	public float linearMix = 0.76f;

	public float sineMix = 0.07f;

	public float noiseMix = 0.07f;

	public float sineWavelength = 0.3f;

	public bool useFadeout;

	public int fadeoutAmount = 8;

	public AutoInterp autoInterp;

	public Vector2 uvOrigin = Vector2.zero;

	public float uvScale = -1f;

	private Material _seaMat;

	public EditorButton buildMesh = new EditorButton("EditorMeshBuild");

	private float[] heightField;

	public float heightMapDepth
	{
		get
		{
			SetSeaMaterial();
			if (!_seaMat.HasProperty("_Depth"))
			{
				return 0f;
			}
			return _seaMat.GetFloat("_Depth");
		}
	}

	public float heightMapBottom
	{
		get
		{
			SetSeaMaterial();
			if (!_seaMat.HasProperty("_Bottom"))
			{
				return 0f;
			}
			return _seaMat.GetFloat("_Bottom");
		}
	}

	private void SetSeaMaterial()
	{
		if (_seaMat == null)
		{
			_seaMat = base.gameObject.GetComponent<Renderer>().sharedMaterial;
		}
	}

	private float GetHeightAtPoint(Vector3 point)
	{
		int num = Mathf.FloorToInt(point.x);
		int num2 = Mathf.FloorToInt(point.z);
		int num3 = num + num2 * (width - 1);
		float num4 = point.x / (float)width;
		float num5 = Mathf.Sin(point.z * sineWavelength);
		float num6 = heightField[num3];
		return linearMix * num4 + sineMix * num5 + noiseMix * num6;
	}

	public void EditorMeshBuild()
	{
		Awake();
		Start();
	}

	private void Awake()
	{
		_seaMaterial = GetComponent<MeshRenderer>().material;
		numSections = width * height;
		numVerts = numSections * 6;
		heightField = new float[(width + 1) * (height + 1)];
	}

	private void Start()
	{
		for (int i = 0; i < heightField.Length; i++)
		{
			heightField[i] = Random.value;
		}
		Mesh mesh = new Mesh();
		Vector3[] array = new Vector3[numVerts];
		int[] array2 = new int[numVerts];
		Color[] array3 = new Color[numVerts];
		Vector3[] array4 = new Vector3[numVerts];
		Vector2[] array5 = new Vector2[numVerts];
		Vector2[] array6 = new Vector2[numVerts];
		int num = 0;
		float uvSize = ((uvScale > 0f) ? uvScale : ((float)Mathf.Min(width, height)));
		for (int j = 0; j < width; j++)
		{
			for (int k = 0; k < height; k++)
			{
				Vector3 vector = new Vector3(j, 0f, k);
				Vector3 vector2 = new Vector3(j, 0f, k + 1);
				Vector3 vector3 = new Vector3(j + 1, 0f, k);
				Vector3 vector4 = new Vector3(j + 1, 0f, k + 1);
				float maskValue = GetMaskValue(j, k);
				float maskValue2 = GetMaskValue(j, k + 1);
				float maskValue3 = GetMaskValue(j + 1, k);
				float maskValue4 = GetMaskValue(j + 1, k + 1);
				array[num] = vector;
				array3[num] = new Color(GetHeightAtPoint(vector), GetHeightAtPoint(vector2), GetHeightAtPoint(vector3), 0f);
				array4[num] = default(Vector3);
				array5[num] = CalcUVs(array[num], uvSize);
				array6[num] = new Vector2(maskValue, 0f);
				array2[num] = num;
				num++;
				array[num] = vector2;
				array3[num] = new Color(GetHeightAtPoint(vector), GetHeightAtPoint(vector2), GetHeightAtPoint(vector3), 0.1f);
				array4[num] = default(Vector3);
				array5[num] = CalcUVs(array[num], uvSize);
				array6[num] = new Vector2(maskValue2, 0f);
				array2[num] = num;
				num++;
				array[num] = vector3;
				array3[num] = new Color(GetHeightAtPoint(vector), GetHeightAtPoint(vector2), GetHeightAtPoint(vector3), 0.2f);
				array4[num] = default(Vector3);
				array5[num] = CalcUVs(array[num], uvSize);
				array6[num] = new Vector2(maskValue3, 0f);
				array2[num] = num;
				num++;
				array[num] = vector4;
				array3[num] = new Color(GetHeightAtPoint(vector4), GetHeightAtPoint(vector3), GetHeightAtPoint(vector2), 0.3f);
				array4[num] = default(Vector3);
				array5[num] = CalcUVs(array[num], uvSize);
				array6[num] = new Vector2(maskValue4, 0f);
				array2[num] = num;
				num++;
				array[num] = vector3;
				array3[num] = new Color(GetHeightAtPoint(vector4), GetHeightAtPoint(vector3), GetHeightAtPoint(vector2), 0.4f);
				array4[num] = default(Vector3);
				array5[num] = CalcUVs(array[num], uvSize);
				array6[num] = new Vector2(maskValue3, 0f);
				array2[num] = num;
				num++;
				array[num] = vector2;
				array3[num] = new Color(GetHeightAtPoint(vector4), GetHeightAtPoint(vector3), GetHeightAtPoint(vector2), 0.5f);
				array4[num] = default(Vector3);
				array5[num] = CalcUVs(array[num], uvSize);
				array6[num] = new Vector2(maskValue2, 0f);
				array2[num] = num;
				num++;
			}
		}
		mesh.vertices = array;
		mesh.triangles = array2;
		mesh.colors = array3;
		mesh.normals = array4;
		mesh.uv = array5;
		if (useFadeout)
		{
			mesh.uv2 = array6;
		}
		GetComponent<MeshFilter>().mesh = mesh;
	}

	private Vector2 CalcUVs(Vector3 position, float uvSize)
	{
		position.x -= uvOrigin.x;
		position.z -= uvOrigin.y;
		return new Vector2(position.x / uvSize, position.z / uvSize);
	}

	private float GetMaskValue(int x, int z)
	{
		float num = width / 2;
		float num2 = Mathf.Max(Mathf.Abs((float)x - num), Mathf.Abs((float)z - num));
		num2 = Mathf.Clamp(num - num2, 0f, fadeoutAmount);
		return num2 / (float)fadeoutAmount;
	}

	private void Update()
	{
		if ((bool)autoInterp)
		{
			_seaMaterial.SetFloat("_Amount", autoInterp.interpAmount);
		}
	}

	private void OnDrawGizmos()
	{
		float y = heightMapBottom;
		float y2 = heightMapDepth;
		Vector3 position = base.transform.position;
		position.y = y;
		Vector3 vector = new Vector3(width, y2, height);
		position += 0.5f * vector;
		Gizmos.color = Color.blue;
		Gizmos.DrawWireCube(position, vector);
	}
}
