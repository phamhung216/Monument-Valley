using UnityEngine;

public class RotationToTexturePan : MonoBehaviour
{
	public Rotatable rotator;

	public float factor = 90f;

	public Renderer rend;

	private Material _instancedMat;

	private void Start()
	{
		_instancedMat = Object.Instantiate(rend.material);
		rend.material = _instancedMat;
	}

	private void LateUpdate()
	{
		_instancedMat.SetTextureOffset("_MainTex", new Vector2(rotator.currentAngle / factor, 0f));
	}
}
