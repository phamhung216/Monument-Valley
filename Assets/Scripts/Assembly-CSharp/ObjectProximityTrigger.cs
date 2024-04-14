using UnityEngine;

public class ObjectProximityTrigger : MonoBehaviour
{
	public GameObject triggerObject;

	public Vector3 triggerVolumeSize = new Vector3(1f, 1f, 1f);

	public ActionSequence actions;

	public bool triggerOnce;

	private bool _isTriggered;

	public bool isTriggered => _isTriggered;

	private void Update()
	{
		if (!(triggerObject == null))
		{
			Vector3 vector = base.transform.InverseTransformPoint(triggerObject.transform.position);
			if (Mathf.Abs(vector.x) < triggerVolumeSize.x / 2f && Mathf.Abs(vector.y) < triggerVolumeSize.y / 2f && Mathf.Abs(vector.z) < triggerVolumeSize.z / 2f)
			{
				SetIsTriggered(triggered: true);
			}
			else
			{
				SetIsTriggered(triggered: false);
			}
		}
	}

	private void SetIsTriggered(bool triggered)
	{
		if (triggered)
		{
			if (!_isTriggered)
			{
				_isTriggered = true;
				StartCoroutine(actions.DoSequence());
			}
		}
		else if (!triggerOnce)
		{
			_isTriggered = false;
		}
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = (_isTriggered ? Color.white : Color.cyan);
		Gizmos.DrawCube(base.transform.position, new Vector3(0.2f, 0.2f, 0.2f));
		if (_isTriggered)
		{
			OnDrawGizmosSelected();
		}
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = (_isTriggered ? Color.white : Color.cyan);
		Gizmos.matrix = base.gameObject.transform.localToWorldMatrix;
		Gizmos.DrawWireCube(new Vector3(0f, 0f, 0f), new Vector3(triggerVolumeSize.x, triggerVolumeSize.y, triggerVolumeSize.z));
		Gizmos.matrix = Matrix4x4.identity;
	}
}
