using UnityEngine;

[ExecuteInEditMode]
public class Zoetrope : MonoBehaviour
{
	private float angle;

	public float speed = 5f;

	public MeshRenderer mr;

	private void Start()
	{
	}

	private void FixedUpdate()
	{
		angle += speed;
		base.transform.eulerAngles = new Vector3(0f, angle, 0f);
		mr.enabled = Mathf.Abs(angle) % 45f > 5f;
	}
}
