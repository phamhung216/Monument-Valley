using System;
using System.Collections;
using System.Reflection;
using Fabric;
using UnityEngine;

[Serializable]
public class TriggerAction
{
	public enum Type
	{
		TriggerableAction = 0,
		Animation = 1,
		Wait = 2,
		SendSceneEvent = 3,
		SingletonAction = 4,
		PlaySound = 5,
		RealTimeAnimation = 6
	}

	public enum OrientationBehaviour
	{
		ExecuteInBoth = 0,
		ExecuteInLandscape = 1,
		ExecuteInPortrait = 2
	}

	private static bool _FastForward;

	public Type type;

	public GameObject target;

	public string componentType;

	public string methodName;

	public Func<IEnumerator> action;

	public AnimationClip animationClip;

	public bool waitUntilComplete = true;

	public float waitTime;

	public SceneEvent sceneEvent;

	public string singletonName;

	public string soundEventName;

	public OrientationBehaviour orientationBehaviour;

	private bool _debugMe;

	private bool _isInitialised;

	public static bool FastForward
	{
		get
		{
			return _FastForward;
		}
		set
		{
			_FastForward = value;
		}
	}

	public bool debugMe
	{
		get
		{
			return _debugMe;
		}
		set
		{
			_debugMe = value;
		}
	}

	public override string ToString()
	{
		return type switch
		{
			Type.TriggerableAction => $"[TriggerAction Run {methodName} on {target.name}]", 
			Type.Animation => $"[TriggerAction Play {animationClip.name} on {target.name}]", 
			Type.Wait => $"[TriggerAction Wait {waitTime}]", 
			Type.SendSceneEvent => $"[TriggerAction Send Scene Event {sceneEvent}]", 
			Type.SingletonAction => $"[TriggerAction SingletonAction {methodName} on {singletonName}]", 
			Type.PlaySound => $"[TriggerAction Play Sound {soundEventName}]", 
			Type.RealTimeAnimation => $"[TriggerAction RealTimeAnimation play {animationClip.name} on {target.name}]", 
			_ => "[TriggerAction]", 
		};
	}

	public bool CreateDelegate()
	{
		action = null;
		if (!target)
		{
			return false;
		}
		System.Type type = System.Type.GetType(componentType);
		if (type == null)
		{
			return false;
		}
		UnityEngine.Component component = target.GetComponent(type);
		if (!component)
		{
			return false;
		}
		MethodInfo methodInfo = null;
		methodInfo = type.GetMethod(methodName);
		if (methodInfo == null)
		{
			return false;
		}
		action = (Func<IEnumerator>)Delegate.CreateDelegate(typeof(Func<IEnumerator>), component, methodInfo);
		_isInitialised = true;
		return true;
	}

	public IEnumerator RunAction()
	{
		if (type == Type.TriggerableAction || Type.SingletonAction == type)
		{
			if (!_isInitialised)
			{
				if (Type.SingletonAction == type)
				{
					target = GameScene.instance.GetSingleton(singletonName);
					_ = (bool)target;
				}
				if (!CreateDelegate())
				{
					DebugUtils.DebugAssert(condition: false);
				}
			}
			IEnumerator actionEnum = action();
			if (actionEnum != null)
			{
				if (GameScene.logActionSequences || debugMe)
				{
				}
				while (actionEnum.MoveNext())
				{
					if (!GameScene.logActionSequences)
					{
						_ = debugMe;
					}
					if (waitUntilComplete)
					{
						if (!GameScene.logActionSequences)
						{
							_ = debugMe;
						}
						if (FastForward)
						{
							DebugUtils.DebugAssert(actionEnum.Current == null);
							break;
						}
						yield return actionEnum.Current;
						continue;
					}
					if (!GameScene.logActionSequences && !debugMe)
					{
					}
					break;
				}
			}
			if (!GameScene.logActionSequences)
			{
				_ = debugMe;
			}
		}
		else if (Type.Animation == type)
		{
			if (!GameScene.logActionSequences)
			{
				_ = debugMe;
			}
			Animation anim = target.GetComponent<Animation>();
			anim.Play(animationClip.name);
			if (FastForward)
			{
				anim[animationClip.name].normalizedTime = 1f;
				anim.Sample();
				yield return null;
			}
			if (waitUntilComplete)
			{
				while (anim.isPlaying)
				{
					if (GameScene.logActionSequences || debugMe)
					{
						_ = anim[animationClip.name];
					}
					yield return null;
				}
			}
			if (!GameScene.logActionSequences)
			{
				_ = debugMe;
			}
		}
		else if (Type.RealTimeAnimation == type)
		{
			if (!GameScene.logActionSequences)
			{
				_ = debugMe;
			}
			RealTimeAnimation customAnimation = target.GetComponent<RealTimeAnimation>();
			customAnimation.animationClip = animationClip;
			customAnimation.Play();
			Animation anim = customAnimation.GetComponent<Animation>();
			if (FastForward)
			{
				anim[animationClip.name].normalizedTime = 1f;
				anim.Sample();
				yield return null;
			}
			if (waitUntilComplete)
			{
				while (customAnimation.isPlaying)
				{
					if (GameScene.logActionSequences || debugMe)
					{
						_ = anim[animationClip.name];
					}
					yield return null;
				}
			}
			if (!GameScene.logActionSequences)
			{
				_ = debugMe;
			}
		}
		else if (Type.Wait == type)
		{
			if (!FastForward)
			{
				yield return new WaitForSeconds(waitTime);
			}
		}
		else if (Type.SendSceneEvent == type)
		{
			GameScene.instance.eventHandlers[sceneEvent].Send();
			if (!FastForward)
			{
				yield return null;
			}
		}
		else if (Type.PlaySound == type && !FastForward && (bool)EventManager.Instance && soundEventName.Trim().Length > 0)
		{
			EventManager.Instance.PostEvent(soundEventName, GameScene.instance.gameObject);
		}
	}

	public void SanityCheck(int idx, GameObject owner)
	{
		switch (type)
		{
		case Type.TriggerableAction:
			if (!target)
			{
				D.Error("Missing target for action " + idx + " in " + owner, owner);
			}
			break;
		case Type.SingletonAction:
			_ = (bool)GameScene.instance.GetSingleton(singletonName);
			break;
		}
	}
}
