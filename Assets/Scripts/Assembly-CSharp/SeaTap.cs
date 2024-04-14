using UnityEngine;

public class SeaTap : GameTouchable
{
	public Splash splashPrefab;

	private Splash[] splashes;

	private int nextSplash;

	public SeaTap()
	{
		claimOnTouchBegan = true;
		claimOnTouchNotTap = false;
		releaseOnTouchNotTap = true;
	}

	private void Start()
	{
		splashes = new Splash[3];
		for (int i = 0; i < splashes.Length; i++)
		{
			splashes[i] = Object.Instantiate(splashPrefab, Vector3.zero, Quaternion.identity);
		}
	}

	public override void OnTouchBegan(GameTouch touch)
	{
		Ray ray = Camera.main.ScreenPointToRay(touch.pos);
		Vector3 pos = ray.origin - ray.direction * ((ray.origin.y - base.transform.position.y) / ray.direction.y);
		pos -= 0.9f / ray.direction.magnitude * ray.direction;
		Splash(pos);
	}

	public void Splash(Vector3 pos)
	{
		splashes[nextSplash].transform.position = pos;
		splashes[nextSplash].Play();
		nextSplash++;
		if (nextSplash == splashes.Length)
		{
			nextSplash = 0;
		}
	}
}
