using System.Collections.Generic;
using UnityEngine;

public class ParticleSystemScaler : MonoBehaviour
{
	public TextAsset sourceText;

	public GenerateWaterMesh waterMeshGenerator;

	private int count;

	private float _worldWidth;

	private float _worldHeight;

	private float _startHeight;

	public float minSize;

	public float maxSize;

	public float lifetime;

	public List<EmissionSlice> slices;

	public int FramesPerEmmision = 1;

	public static bool isGenerationEnabled => false;

	private void Awake()
	{
		_startHeight = base.transform.position.y;
	}

	private void Start()
	{
		_worldWidth = waterMeshGenerator.width;
		_worldHeight = waterMeshGenerator.heightMapDepth;
	}

	public void BuildEmissionPoints()
	{
		D.Error("ENABLE_EMISSION_POINT_BUILDING not defined");
	}

	private void Update()
	{
		int value = Mathf.FloorToInt((base.gameObject.transform.parent.position.y - _startHeight) * (255f / _worldHeight));
		value = Mathf.Clamp(value, 0, 255);
		int index = Random.Range(0, slices[value].positions.Count);
		if (Time.frameCount % FramesPerEmmision == 0)
		{
			Vector3 position = slices[value].positions[index];
			ParticleSystem.EmitParams emitParams = default(ParticleSystem.EmitParams);
			emitParams.velocity = new Vector3(Random.Range(-0.4f, 0.4f), Random.Range(1f, 1.5f), Random.Range(-0.4f, 0.4f));
			emitParams.startSize = Random.Range(minSize, maxSize);
			emitParams.startLifetime = lifetime;
			emitParams.startColor = Color.white;
			emitParams.position = position;
			GetComponent<ParticleSystem>().Emit(emitParams, 1);
		}
	}

	private void OnDrawGizmosSelected()
	{
		if (slices == null)
		{
			return;
		}
		Gizmos.color = Color.white;
		for (int i = 0; i < 255; i++)
		{
			for (int j = 0; j < slices[i].positions.Count; j++)
			{
				Gizmos.DrawCube(slices[i].positions[j], new Vector3(0.2f, 0.2f, 0.2f));
			}
		}
	}
}
