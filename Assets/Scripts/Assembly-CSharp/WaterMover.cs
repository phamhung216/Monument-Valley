using UnityEngine;

public class WaterMover : MonoBehaviour
{
	public WaterSection waterSection;

	public Transform startPoint;

	public Transform endPoint;

	public Transform target;

	private bool hasDoneNavReconfig = true;

	public AnimationCurveDefinition snapAnimation;

	public bool useNavReconfig;

	private MoverAudio _moverAudio;

	private float _lastParam;

	private void Start()
	{
		if (snapAnimation == null)
		{
			GameObject gameObject = GameObject.Find("RotateSnapCurve");
			if ((bool)gameObject && (bool)gameObject.GetComponent<AnimationCurveDefinition>())
			{
				snapAnimation = gameObject.GetComponent<AnimationCurveDefinition>();
			}
		}
		_moverAudio = target.GetComponent<MoverAudio>();
	}

	private void Update()
	{
		float num = snapAnimation.curve.Evaluate(waterSection.fillAmount);
		if (useNavReconfig && (waterSection.fillAmount >= 1f || waterSection.fillAmount <= 0f))
		{
			if (!hasDoneNavReconfig)
			{
				GameScene.navManager.NotifyReconfigurationBegan(base.gameObject);
				target.position = Vector3.Lerp(startPoint.position, endPoint.position, num);
				GameScene.navManager.NotifyReconfigurationEnded();
			}
			hasDoneNavReconfig = true;
		}
		else
		{
			hasDoneNavReconfig = false;
			target.position = Vector3.Lerp(startPoint.position, endPoint.position, num);
		}
		if (num != _lastParam && (bool)_moverAudio && _moverAudio.motionType == MoverAudio.MotionType.Translate)
		{
			_moverAudio.NotifyMove();
		}
		_lastParam = num;
	}
}
