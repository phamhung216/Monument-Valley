using UnityEngine;

public class NSidedCubeColumn : MonoBehaviour
{
	public Transform[] sources;

	private void Start()
	{
	}

	private void Update()
	{
		Vector3 localPosition = base.transform.localPosition;
		localPosition.y = 0f;
		Transform[] array = sources;
		foreach (Transform transform in array)
		{
			localPosition.y += 2f - transform.localPosition.y;
		}
		base.transform.localPosition = localPosition;
	}
}
