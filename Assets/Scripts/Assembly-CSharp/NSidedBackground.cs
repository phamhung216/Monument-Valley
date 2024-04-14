using System.Collections;
using UnityEngine;

public class NSidedBackground : MonoBehaviour
{
	public MeshRenderer targetMeshRenderer;

	private Material targetMaterial;

	public string targetColorName = "_Colour";

	public Color colorA;

	public Color colorB;

	public Color colorC;

	private Color srcColor;

	private Color destColor;

	public float blendTimeB = 5f;

	public float blendTimeC = 5f;

	private float blendTimeTotal;

	private float blendTimer;

	private void Start()
	{
		targetMaterial = targetMeshRenderer.material;
	}

	private void LateUpdate()
	{
		if (blendTimer > 0f)
		{
			blendTimer -= Time.deltaTime;
			if (blendTimer > blendTimeTotal)
			{
				blendTimer = 0f;
			}
			targetMaterial.SetColor(targetColorName, Color.Lerp(destColor, srcColor, blendTimer / blendTimeTotal));
		}
	}

	[TriggerableAction]
	public IEnumerator StartBlending_To_B()
	{
		blendTimeTotal = blendTimeB;
		blendTimer = blendTimeTotal;
		srcColor = colorA;
		destColor = colorB;
		return null;
	}

	[TriggerableAction]
	public IEnumerator StartBlending_To_C()
	{
		blendTimeTotal = blendTimeC;
		blendTimer = blendTimeTotal;
		srcColor = colorB;
		destColor = colorC;
		return null;
	}

	[TriggerableAction]
	public IEnumerator SetColorB_Immediate()
	{
		blendTimer = 0f;
		targetMaterial.SetColor(targetColorName, colorB);
		return null;
	}

	[TriggerableAction]
	public IEnumerator SetColorC_Immediate()
	{
		blendTimer = 0f;
		targetMaterial.SetColor(targetColorName, colorC);
		return null;
	}
}
