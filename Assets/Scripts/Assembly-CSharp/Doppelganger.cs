using UnityEngine;

public class Doppelganger : MonoBehaviour
{
	public Transform player;

	public Transform doppel;

	public float rayCastSourceUpper;

	public float rayCastSourceLower;

	public float rayCastSourceCurrent;

	private bool doppelGangerAbove;

	private void Start()
	{
		rayCastSourceCurrent = rayCastSourceLower;
	}

	public void Flip(DayNightControl.TimePeriod period)
	{
		doppel.rotation = player.rotation;
		doppel.position = player.position;
		if (period == DayNightControl.TimePeriod.DAY)
		{
			rayCastSourceCurrent = rayCastSourceUpper;
		}
		else
		{
			rayCastSourceCurrent = rayCastSourceLower;
		}
		if (Physics.Raycast(new Ray(new Vector3(player.position.x, rayCastSourceCurrent, player.position.z), Vector3.down), out var hitInfo, 1000f, 1024))
		{
			player.GetComponent<CharacterLocomotion>().Teleport(new Vector3(player.position.x, hitInfo.point.y, player.position.z));
		}
	}
}
