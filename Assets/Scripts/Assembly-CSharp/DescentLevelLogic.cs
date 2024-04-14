using System.Collections;
using UnityEngine;

public class DescentLevelLogic : MonoBehaviour
{
	public Transform waterPlane;

	public Transform player;

	public GameObject islandParentObject;

	public Camera invertCam;

	public Transform audioListener;

	private Transform _keepBelowTransform;

	private bool _linkWaterLevelToPlayer;

	private bool _fastForward;

	private ValueSmoother _heightSmoother = new ValueSmoother();

	[TriggerableAction]
	public IEnumerator UseInvertCamForInput()
	{
		GameScene.player.GetComponent<PlayerInput>().currentCamera = invertCam;
		return null;
	}

	[TriggerableAction]
	public IEnumerator UseNormalCamForInput()
	{
		GameScene.player.GetComponent<PlayerInput>().currentCamera = Camera.main;
		return null;
	}

	[TriggerableAction]
	public IEnumerator DisableWaterLevelChange()
	{
		_linkWaterLevelToPlayer = false;
		return null;
	}

	[TriggerableAction]
	public IEnumerator EnableWaterLevelChange()
	{
		_heightSmoother.Reset(waterPlane.position.y);
		_linkWaterLevelToPlayer = true;
		_fastForward |= TriggerAction.FastForward;
		_keepBelowTransform = GameScene.player.transform;
		return null;
	}

	private void Start()
	{
		if (player.position.y < waterPlane.position.y - 10f)
		{
			waterPlane.gameObject.SetActive(value: false);
		}
		audioListener.parent = GameScene.player.transform;
		audioListener.localPosition = Vector3.zero;
		_heightSmoother.Reset(waterPlane.position.y);
		_heightSmoother.maxTarget = float.MaxValue;
		_heightSmoother.minTarget = float.MinValue;
		_heightSmoother.easeDownTime = 1f;
		_heightSmoother.clampDist = 0.01f;
		_keepBelowTransform = player.transform;
	}

	private void Update()
	{
		if (_linkWaterLevelToPlayer)
		{
			UpdateWaterLevel();
		}
	}

	public void SetHeightTarget(Transform target, float easeTime)
	{
		_keepBelowTransform = target;
		_heightSmoother.easeDownTime = easeTime;
	}

	private void UpdateWaterLevel()
	{
		Vector3 position = waterPlane.position;
		Vector3 vector = position;
		vector.y = Mathf.Min(vector.y, Mathf.Min(player.transform.position.y, _keepBelowTransform.position.y) - 1.91f);
		if (vector.y - position.y < 0f)
		{
			if (!_fastForward)
			{
				_heightSmoother.target = vector.y;
				_heightSmoother.Advance();
			}
			else
			{
				_fastForward = false;
				_heightSmoother.Reset(vector.y);
			}
			position.y = _heightSmoother.smoothedValue;
			waterPlane.position = position;
		}
	}
}
