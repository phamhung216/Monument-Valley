using System.Collections;
using UnityEngine;

public class InsideBox_LevelScript : MonoBehaviour
{
	public bool atRestPosition;

	public Transform lidChild;

	public Transform lidParentA;

	public Transform lidParentB;

	public Transform coverParentMain;

	public Transform coverChild;

	public Transform coverParentC;

	public Transform coverParentD;

	public Rotatable rotatableA;

	public Rotatable rotatableB;

	public AxisDragger draggerC;

	public AxisDragger draggerD;

	[SerializeField]
	private GameTouchable _handleA;

	[SerializeField]
	private GameTouchable _handleB;

	[SerializeField]
	private GameTouchable _handleC;

	[SerializeField]
	private GameTouchable _handleD;

	public CombinationLock combinationLock;

	public ToggleController toggleController;

	public ToggleController toggleController2;

	public ProximityTrigger combinationLockTrigger;

	public ProximityTrigger[] checksForSecondStage;

	public TriggerableActionSequence showSecondStage;

	public Transform handleAnimContainerA;

	public Transform handleAnimContainerB;

	public Transform handleAnimContainerC;

	public Transform handleAnimContainerD;

	public AIController crowC;

	public AIController crowD;

	private bool activeA;

	private bool activeB;

	private bool activeC;

	private bool activeD;

	private bool passedFirstStage;

	private void Awake()
	{
		RotationDebug.SetAllForGameplay();
	}

	private void Start()
	{
		crowC.SetCrowIdle();
		crowD.SetCrowIdle();
	}

	private void Update()
	{
		if (!passedFirstStage)
		{
			FirstStageLogic();
		}
	}

	private void EnsureCrowOnNav(AIController crow)
	{
		crow.Teleport(crow.lastValidBrush);
	}

	private void FirstStageLogic()
	{
		if (!rotatableA.AtMaximum())
		{
			if (!activeA)
			{
				activeA = true;
				toggleController.ShowOnly(0);
				DisableHandle(_handleB, handleAnimContainerB);
				DisableHandle(_handleC, handleAnimContainerC);
				DisableHandle(_handleD, handleAnimContainerD);
				lidChild.parent = lidParentA;
				lidChild.localPosition = Vector3.zero;
				lidChild.localEulerAngles = Vector3.zero;
			}
		}
		else if (!rotatableB.AtMinimum())
		{
			if (!activeB)
			{
				activeB = true;
				toggleController.ShowOnly(1);
				DisableHandle(_handleA, handleAnimContainerA);
				DisableHandle(_handleC, handleAnimContainerC);
				DisableHandle(_handleD, handleAnimContainerD);
				lidChild.parent = lidParentB;
				lidChild.localPosition = Vector3.zero;
				lidChild.localEulerAngles = Vector3.zero;
			}
		}
		else if (!draggerC.AtMinimum())
		{
			if (!activeC)
			{
				activeC = true;
				toggleController.ShowOnly(2);
				EnsureCrowOnNav(crowC);
				crowC.SetCrowWalking();
				DisableHandle(_handleA, handleAnimContainerA);
				DisableHandle(_handleB, handleAnimContainerB);
				DisableHandle(_handleD, handleAnimContainerD);
				draggerC.transform.parent = coverParentMain;
				draggerD.transform.parent = coverParentMain;
				draggerD.transform.parent = coverParentC;
				coverChild.parent = coverParentC;
				coverChild.localPosition = Vector3.zero;
				draggerC.transform.localPosition = Vector3.zero;
				draggerD.transform.localPosition = Vector3.zero;
			}
		}
		else if (!draggerD.AtMinimum())
		{
			if (!activeD)
			{
				activeD = true;
				toggleController.ShowOnly(3);
				EnsureCrowOnNav(crowD);
				crowD.SetCrowWalking();
				DisableHandle(_handleA, handleAnimContainerA);
				DisableHandle(_handleB, handleAnimContainerB);
				DisableHandle(_handleC, handleAnimContainerC);
				draggerC.transform.parent = coverParentMain;
				draggerD.transform.parent = coverParentMain;
				draggerC.transform.parent = coverParentD;
				coverChild.parent = coverParentD;
				coverChild.localPosition = Vector3.zero;
				draggerC.transform.localPosition = Vector3.zero;
				draggerD.transform.localPosition = Vector3.zero;
			}
		}
		else
		{
			if (activeC)
			{
				crowC.SetCrowIdle();
			}
			if (activeD)
			{
				crowD.SetCrowIdle();
			}
			if (!passedFirstStage)
			{
				EnableHandle(_handleA, handleAnimContainerA);
				EnableHandle(_handleB, handleAnimContainerB);
				EnableHandle(_handleC, handleAnimContainerC);
				EnableHandle(_handleD, handleAnimContainerD);
				activeA = false;
				activeB = false;
				activeC = false;
				activeD = false;
			}
		}
	}

	private void DisableHandle(GameTouchable handle, Transform handleAnim)
	{
		if (handle.isEnabled)
		{
			handle.Disable();
			PlayAnimationInChildren(handleAnim, "Close");
		}
	}

	private void EnableHandle(GameTouchable handle, Transform handleAnim)
	{
		if (!handle.isEnabled)
		{
			handle.Enable();
			PlayAnimationInChildren(handleAnim, "Open");
		}
	}

	private void PlayAnimationInChildren(Transform container, string animationName)
	{
		Animation[] componentsInChildren = container.GetComponentsInChildren<Animation>();
		foreach (Animation animation in componentsInChildren)
		{
			AnimationState animationState = animation[animationName];
			if ((bool)animationState)
			{
				animation.Stop();
				animationState.time = 0f;
				animation.Play(animationName);
			}
		}
	}

	[TriggerableAction]
	public IEnumerator PassedFirstStage()
	{
		passedFirstStage = true;
		DisableHandle(_handleA, handleAnimContainerA);
		DisableHandle(_handleB, handleAnimContainerB);
		DisableHandle(_handleC, handleAnimContainerC);
		DisableHandle(_handleD, handleAnimContainerD);
		crowC.SetCrowIdle();
		crowD.SetCrowIdle();
		return null;
	}
}
