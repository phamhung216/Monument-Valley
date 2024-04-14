using System.Collections.Generic;
using UnityEngine;

public class LevelProgressStrand : MonoBehaviour
{
	public List<Checkpoint> checkpoints;

	public void FastForwardToCheckpoint(Checkpoint checkpoint)
	{
		if (!checkpoints.Contains(checkpoint))
		{
			return;
		}
		for (int i = 0; i < checkpoints.Count; i++)
		{
			checkpoints[i].FastForwardToHere();
			if (checkpoints[i] == checkpoint)
			{
				break;
			}
		}
	}

	public Checkpoint GetCurrentCheckpoint()
	{
		Checkpoint result = null;
		for (int i = 0; i < checkpoints.Count && checkpoints[i].IsReached; i++)
		{
			result = checkpoints[i];
		}
		return result;
	}

	public Checkpoint GetLastCheckpoint()
	{
		Checkpoint currentCheckpoint = GetCurrentCheckpoint();
		Checkpoint result = null;
		for (int i = 0; i < checkpoints.Count - 1; i++)
		{
			if (checkpoints[i + 1] == currentCheckpoint)
			{
				result = checkpoints[i];
				break;
			}
		}
		return result;
	}

	public Checkpoint[] GetCheckpoints()
	{
		return checkpoints.ToArray();
	}
}
