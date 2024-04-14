using UnityEngine;

public class Spinner : GameTouchable
{
	public MeshRenderer[] renderers;

	public Material mat;

	private float speed;

	private float interp;

	public float minSpeed = 2f;

	public float maxSpeed = 12f;

	public float decay = 5f;

	public float impulse = 5f;

	public Transform target;

	public Color colorA;

	public Color colorB;

	private void Start()
	{
		mat = Object.Instantiate(mat);
		MeshRenderer[] array = renderers;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].material = mat;
		}
	}

	private void Update()
	{
		speed = Mathf.Clamp(Mathf.MoveTowards(speed, minSpeed, decay * Time.deltaTime), 0f, maxSpeed);
		target.localEulerAngles = new Vector3(target.localEulerAngles.x, target.localEulerAngles.y + speed, target.localEulerAngles.z);
		interp = speed / maxSpeed;
		mat.color = Color.Lerp(colorA, colorB, interp);
	}

	public override void OnTouchBegan(GameTouch touch)
	{
		speed += impulse;
	}
}
