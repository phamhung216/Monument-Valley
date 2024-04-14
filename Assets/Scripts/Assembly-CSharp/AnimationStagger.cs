using UnityEngine;

public class AnimationStagger : MonoBehaviour
{
	public Animation[] anis;

	public float staggerAmount = 0.1f;

	private void Start()
	{
		Animation[] array = anis;
		foreach (Animation animation in array)
		{
			animation["IntroPlants_Wiggle"].time = animation.transform.position.x * staggerAmount;
		}
	}

	private void Update()
	{
	}
}
