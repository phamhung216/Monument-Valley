using UnityEngine;

public class InsideBox_CrowClip : MonoBehaviour
{
	public Renderer[] renderers;

	public Transform crow;

	public float clipTheshold;

	private bool visible;

	private void LateUpdate()
	{
		bool flag = crow.position.y > clipTheshold;
		if (flag != visible)
		{
			Renderer[] array = renderers;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].enabled = flag;
			}
			visible = flag;
		}
	}
}
