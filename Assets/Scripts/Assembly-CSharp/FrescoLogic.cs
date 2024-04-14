using UnityEngine;

public class FrescoLogic : MonoBehaviour
{
	public Rotatable frescoRotator;

	public ToggleController toggleController;

	public AutoRotator startPositionAutoRotator;

	public int revealAmount;

	private void Start()
	{
	}

	private void Update()
	{
		if (frescoRotator.currentAngle < 90f)
		{
			toggleController.ShowOnly(0);
		}
		else if (frescoRotator.currentAngle < 360f)
		{
			toggleController.ShowOnly(1);
		}
		else
		{
			toggleController.ShowOnly(2);
		}
	}
}
