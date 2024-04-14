using UnityEngine;

public class PressureSwitchMatSwap : MonoBehaviour
{
	public Material originalMat;

	public Material alternateMat;

	private Material currentMat;

	private void Start()
	{
		currentMat = originalMat;
		NavBrushComponent componentInChildren = base.transform.parent.GetComponentInChildren<NavBrushComponent>();
		if ((bool)componentInChildren)
		{
			GameObject gameObject = new GameObject("navIndicatorPos");
			gameObject.transform.parent = base.transform;
			gameObject.transform.localRotation = Quaternion.identity;
			gameObject.transform.localPosition = new Vector3(0f, 0f, 0.13f);
			componentInChildren.navIndicatorPosition = gameObject.transform;
			componentInChildren.useNavIndicatorPositionForLocomotion = true;
		}
	}

	public void SwapMatToAlternate()
	{
		currentMat = alternateMat;
	}

	public void SwapMatToOriginal()
	{
		currentMat = originalMat;
	}

	private void Update()
	{
		if (!(currentMat == null) && !currentMat.Equals(GetComponent<MeshRenderer>().sharedMaterial))
		{
			GetComponent<MeshRenderer>().sharedMaterial = currentMat;
		}
	}
}
