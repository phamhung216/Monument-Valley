using UnityEngine;

public class Cubewalker_Logic : MonoBehaviour
{
	public Rotatable rotatorX;

	public Rotatable rotatorY;

	public Rotatable rotatorZ;

	public Transform masterCube;

	public RotatorHandle[] handleSetX;

	public RotatorHandle[] handleSetY;

	public RotatorHandle[] handleSetZ;

	private void Update()
	{
		if (!rotatorX.isStationary)
		{
			ToggleHandleSet(handleSetY, b: false);
			ToggleHandleSet(handleSetZ, b: false);
			masterCube.parent = rotatorX.transform;
		}
		else if (!rotatorY.isStationary)
		{
			ToggleHandleSet(handleSetX, b: false);
			ToggleHandleSet(handleSetZ, b: false);
			masterCube.parent = rotatorY.transform;
		}
		else if (!rotatorZ.isStationary)
		{
			ToggleHandleSet(handleSetX, b: false);
			ToggleHandleSet(handleSetY, b: false);
			masterCube.parent = rotatorZ.transform;
		}
		else
		{
			ToggleHandleSet(handleSetX, b: true);
			ToggleHandleSet(handleSetY, b: true);
			ToggleHandleSet(handleSetZ, b: true);
		}
		Snap();
	}

	private void ToggleHandleSet(RotatorHandle[] handleSet, bool b)
	{
		for (int i = 0; i < handleSet.Length; i++)
		{
			handleSet[i].isEnabled = b;
		}
	}

	private void Snap()
	{
		masterCube.localEulerAngles = new Vector3(Mathf.RoundToInt(masterCube.localEulerAngles.x / 45f) * 45, Mathf.RoundToInt(masterCube.localEulerAngles.y / 45f) * 45, Mathf.RoundToInt(masterCube.localEulerAngles.z / 45f) * 45);
	}
}
