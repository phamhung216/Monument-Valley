using System;
using System.Collections;
using UnityEngine;

public class LSEPortal : MonoBehaviour
{
	public ActionSequence portal_Activation_Actions = new ActionSequence();

	public ActionSequence portal_Deactivation_Actions = new ActionSequence();

	public ActionSequence portal_Enter_Actions = new ActionSequence();

	public TriggerableActionSequence artifact_Appear_Sequence;

	public ParticleSystem startLevelParticles;

	public Material buttonCompleted_Mat;

	public Material buttonUncompleted_Mat;

	public GameObject button_GO;

	public MeshRenderer buttonVisual_MR;

	public AutoInterp artifactInterp;

	public LSEPortalManager lsePortalManager;

	public bool isActivated;

	public TouchTrigger touchHandler;

	public void ShowArtifactImmediate()
	{
		artifactInterp.SetToOne();
	}

	public void HideArtifactImmediate()
	{
		artifactInterp.SetToZero();
	}

	public void SetCompleted()
	{
		buttonVisual_MR.material = buttonCompleted_Mat;
	}

	public void SetUncompleted()
	{
		buttonVisual_MR.material = buttonUncompleted_Mat;
	}

	public void HideButton()
	{
		button_GO.SetActive(value: false);
	}

	public void ShowButton()
	{
		button_GO.SetActive(value: true);
	}

	public void SetEnableTouch(bool _enable)
	{
		touchHandler.enabled = _enable;
	}

	[TriggerableAction]
	public IEnumerator Touched()
	{
		if (isActivated)
		{
			try
			{
				GameScene.instance.eventHandlers[SceneEvent.DisableInput].Send();
				startLevelParticles.Play();
				StartCoroutine(portal_Enter_Actions.DoSequence());
			}
			catch (Exception ex)
			{
				D.Error("LSEPortal " + base.name + " Coroutine threw exception " + ex, base.gameObject);
			}
		}
		else
		{
			Activate();
		}
		lsePortalManager.DeactivateOtherPortals(this);
		return null;
	}

	public void Activate()
	{
		try
		{
			isActivated = true;
			StartCoroutine(portal_Activation_Actions.DoSequence());
		}
		catch (Exception ex)
		{
			D.Error("LSEPortal " + base.name + " Coroutine threw exception " + ex, base.gameObject);
		}
	}

	public void Deactivate()
	{
		if (isActivated)
		{
			try
			{
				isActivated = false;
				StartCoroutine(portal_Deactivation_Actions.DoSequence());
			}
			catch (Exception ex)
			{
				D.Error("LSEPortal " + base.name + " Coroutine threw exception " + ex, base.gameObject);
			}
		}
	}

	public void ArtifactAppear()
	{
		artifact_Appear_Sequence.TriggerActions();
	}
}
