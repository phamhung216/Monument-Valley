using UnityEngine;

public class RenderAndDispose : MonoBehaviour
{
	public Camera disposeCamera;

	public GameObject[] disposeObjects;

	public int liveFrames = 2;

	private int _frameCount;

	private void OnPostRender()
	{
		_frameCount++;
		if (_frameCount > liveFrames)
		{
			disposeCamera.enabled = false;
			GameObject[] array = disposeObjects;
			for (int i = 0; i < array.Length; i++)
			{
				Object.Destroy(array[i]);
			}
		}
	}
}
