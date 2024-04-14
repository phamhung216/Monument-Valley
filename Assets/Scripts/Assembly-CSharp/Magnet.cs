using UnityEngine;

public class Magnet : MonoBehaviour
{
	public enum followState
	{
		NONE = 0,
		FOLLOWING = 1,
		ENGAGING = 2,
		DISENGAGING = 3
	}

	public MeshRenderer lightBeamMeshRenderer;

	public Transform followTarget;

	public followState currentFollowState;

	public bool hasCheckedNav;

	public bool movedLastFrame;

	private float magnetEngageDistance = 0.1f;

	private void Start()
	{
	}

	private void OnTriggerEnter(Collider collider)
	{
		if (collider.GetComponent<MagnetBlocker>() != null && currentFollowState == followState.FOLLOWING)
		{
			base.transform.position = snapVector3(base.transform.position);
			currentFollowState = followState.DISENGAGING;
		}
	}

	private Vector3 snapVector3(Vector3 input)
	{
		return new Vector3(Mathf.Round(input.x), Mathf.Round(input.y), Mathf.Round(input.z));
	}

	private void Update()
	{
		switch (currentFollowState)
		{
		case followState.NONE:
			if (Vector3.Distance(base.transform.position, followTarget.position) < magnetEngageDistance)
			{
				currentFollowState = followState.ENGAGING;
				lightBeamMeshRenderer.enabled = true;
			}
			break;
		case followState.ENGAGING:
			if (Vector3.Distance(base.transform.position, followTarget.position) > magnetEngageDistance)
			{
				currentFollowState = followState.FOLLOWING;
			}
			break;
		case followState.DISENGAGING:
			if (Vector3.Distance(base.transform.position, followTarget.position) > magnetEngageDistance)
			{
				currentFollowState = followState.NONE;
				CheckNav();
			}
			break;
		case followState.FOLLOWING:
			break;
		}
	}

	private void LateUpdate()
	{
		if (currentFollowState == followState.FOLLOWING)
		{
			if (base.transform.position != followTarget.position)
			{
				movedLastFrame = true;
				hasCheckedNav = false;
				base.transform.position = followTarget.position;
			}
			else if (!hasCheckedNav)
			{
				CheckNav();
			}
		}
	}

	private void CheckNav()
	{
		GameScene.navManager.NotifyReconfigurationBegan(base.gameObject);
		GameScene.navManager.NotifyReconfigurationEnded();
		hasCheckedNav = true;
	}
}
