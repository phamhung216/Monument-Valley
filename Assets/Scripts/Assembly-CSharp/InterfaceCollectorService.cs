using System;
using System.Collections.Generic;
using UnityEngine;

public class InterfaceCollectorService : Service<InterfaceCollectorService>, ISerializationCallbackReceiver
{
	private List<List<MonoBehaviour>> listPerInterface = new List<List<MonoBehaviour>>();

	private List<Type> listTypes = new List<Type>();

	[SerializeField]
	[HideInInspector]
	private List<MonoBehaviour> allSceneBehaviours = new List<MonoBehaviour>();

	protected override void Awake()
	{
		base.Awake();
		MonoBehaviour[] collection = UnityEngine.Object.FindObjectsOfType<MonoBehaviour>();
		allSceneBehaviours.AddRange(collection);
		RegisterInterfaces();
	}

	private void BuildAccessorList<TI>() where TI : class
	{
		List<MonoBehaviour> list = new List<MonoBehaviour>();
		foreach (MonoBehaviour allSceneBehaviour in allSceneBehaviours)
		{
			if (allSceneBehaviour is TI)
			{
				list.Add(allSceneBehaviour);
			}
		}
		listTypes.Add(typeof(TI));
		listPerInterface.Add(list);
	}

	public List<MonoBehaviour> GetListOfBehavioursForInterface<T>() where T : class
	{
		for (int i = 0; i < listTypes.Count; i++)
		{
			if (listTypes[i] == typeof(T) && listPerInterface.Count > i)
			{
				return listPerInterface[i];
			}
		}
		return new List<MonoBehaviour>();
	}

	private void RegisterInterfaces()
	{
		listPerInterface.Clear();
		listTypes.Clear();
		BuildAccessorList<IHoverable>();
	}

	void ISerializationCallbackReceiver.OnBeforeSerialize()
	{
	}

	void ISerializationCallbackReceiver.OnAfterDeserialize()
	{
		Service<InterfaceCollectorService>._instance = this;
		RegisterInterfaces();
	}

	public void RegisterInterface<T>(MonoBehaviour obj) where T : class
	{
		List<MonoBehaviour> listOfBehavioursForInterface = GetListOfBehavioursForInterface<T>();
		if (!listOfBehavioursForInterface.Contains(obj))
		{
			listOfBehavioursForInterface.Add(obj);
		}
	}
}
