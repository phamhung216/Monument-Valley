using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rotatable))]
public class GearRotator : MonoBehaviour
{
	public Rotatable powerSource;

	public float gearRatio = 1f;

	private Rotatable _target;

	private float _initialSourceAngle;

	private float _initialAngle;

	private bool _isMoving;

	private void Start()
	{
		_target = GetComponent<Rotatable>();
		Enable();
	}

	private void LateUpdate()
	{
		if (powerSource != null)
		{
			float currentAngle = _initialAngle + (powerSource.currentAngle - _initialSourceAngle) * gearRatio;
			_target.currentAngle = currentAngle;
			SetIsMoving(powerSource.dragging || powerSource.snapping);
		}
	}

	private void SetIsMoving(bool value)
	{
		if (_isMoving != value)
		{
			_isMoving = value;
			if (_isMoving)
			{
				GameScene.navManager.NotifyReconfigurationBegan(base.gameObject);
			}
			else
			{
				GameScene.navManager.NotifyReconfigurationEnded();
			}
		}
	}

	[TriggerableAction]
	public IEnumerator Disable()
	{
		base.enabled = false;
		return null;
	}

	[TriggerableAction]
	public IEnumerator Enable()
	{
		base.enabled = true;
		if (powerSource != null)
		{
			_initialSourceAngle = powerSource.currentAngle;
		}
		if ((bool)GetComponent<Rotatable>())
		{
			_initialAngle = _target.currentAngle;
		}
		return null;
	}
}
