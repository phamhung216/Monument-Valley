using System.Collections;
using UnityEngine;

public class DescentWaterHeightMarker : MonoBehaviour
{
	public DescentLevelLogic descentLevelLogic;

	public float easeTime;

	[TriggerableAction]
	public IEnumerator UseMe()
	{
		descentLevelLogic.SetHeightTarget(base.transform, easeTime);
		return null;
	}
}
