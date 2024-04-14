using UnityEngine;

public class MassCubeRenamer : MonoBehaviour
{
	public Transform[] targets;

	public bool rename;

	[ExecuteInEditMode]
	public void Rename()
	{
		Transform[] array = targets;
		foreach (Transform transform in array)
		{
			transform.name = transform.localPosition.x + "_" + Mathf.Abs(transform.localPosition.z);
		}
	}

	private void Update()
	{
		if (rename)
		{
			Rename();
			rename = false;
		}
	}
}
