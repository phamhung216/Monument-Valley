using UnityEngine;

public class StartupSequence : MonoBehaviour
{
	public ActionSequence sequence = new ActionSequence();

	private void Start()
	{
		StartCoroutine(sequence.DoSequence());
	}

	private void Update()
	{
	}
}
