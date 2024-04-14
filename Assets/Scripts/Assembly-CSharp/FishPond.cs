using UnityEngine;

public class FishPond : MonoBehaviour
{
	private FishController[] _fish;

	private FishPondWall[] _walls;

	private Vector3 _cachedPosition;

	public FishController[] fish => _fish;

	public FishPondWall[] walls => _walls;

	private void Start()
	{
		_fish = base.gameObject.GetComponentsInChildren<FishController>();
		_walls = base.gameObject.GetComponentsInChildren<FishPondWall>();
		_cachedPosition = base.transform.position;
	}

	private void Update()
	{
		if (_cachedPosition != base.transform.position)
		{
			FishPondWall[] array = _walls;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].UpdatePlane();
			}
		}
	}
}
