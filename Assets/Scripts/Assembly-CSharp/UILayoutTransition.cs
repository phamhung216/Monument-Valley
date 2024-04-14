using System.Collections;
using UnityEngine;

public class UILayoutTransition : MonoBehaviour
{
	private enum Transition
	{
		None = 0,
		SlideInFromTop = 1,
		SlideOutToTop = 2,
		SlideInFromBottom = 3,
		SlideOutToBottom = 4
	}

	public UILayout targetLayout;

	[HideInInspector]
	public float slideTime;

	private Transition _transition;

	private Animation _animation;

	private void Start()
	{
		_animation = GetComponent<Animation>();
	}

	private void LateUpdate()
	{
		if (_animation != null && _transition != 0)
		{
			Vector3 centre = targetLayout.centre;
			switch (_transition)
			{
			case Transition.SlideInFromTop:
				centre.y = (targetLayout.parentLayout.layoutHeight + targetLayout.layoutHeight) / 2f - targetLayout.layoutHeight * slideTime;
				break;
			case Transition.SlideOutToTop:
				centre.y = (targetLayout.parentLayout.layoutHeight + targetLayout.layoutHeight) / 2f - targetLayout.layoutHeight * (1f - slideTime);
				break;
			case Transition.SlideInFromBottom:
				centre.y = (0f - (targetLayout.parentLayout.layoutHeight + targetLayout.layoutHeight)) / 2f + targetLayout.layoutHeight * slideTime;
				break;
			case Transition.SlideOutToBottom:
				centre.y = (0f - (targetLayout.parentLayout.layoutHeight + targetLayout.layoutHeight)) / 2f + targetLayout.layoutHeight * (1f - slideTime);
				break;
			}
			targetLayout.centre = centre;
		}
	}

	[TriggerableAction]
	public IEnumerator SlideInFromTop()
	{
		_transition = Transition.SlideInFromTop;
		return null;
	}

	[TriggerableAction]
	public IEnumerator SlideOutToTop()
	{
		_transition = Transition.SlideOutToTop;
		return null;
	}

	[TriggerableAction]
	public IEnumerator SlideInFromBottom()
	{
		_transition = Transition.SlideInFromBottom;
		return null;
	}

	[TriggerableAction]
	public IEnumerator SlideOutToBottom()
	{
		_transition = Transition.SlideOutToBottom;
		return null;
	}
}
