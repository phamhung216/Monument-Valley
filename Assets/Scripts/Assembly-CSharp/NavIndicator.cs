using Fabric;
using UnityEngine;

public class NavIndicator : MonoBehaviour
{
	public ParticleSystem worldSpaceParticles;

	public ParticleSystem panSpaceParticles;

	public Transform panSpaceRoot;

	private float _worldOffsetDist = 0.1f;

	private NavBrushComponent _brush;

	public void PlayForBrush(NavBrushComponent brush)
	{
		_brush = brush;
		UpdateParticleSystemPositions();
		if ((bool)worldSpaceParticles)
		{
			worldSpaceParticles.Play();
		}
		if ((bool)panSpaceParticles)
		{
			panSpaceParticles.Play();
		}
		if ((bool)EventManager.Instance)
		{
			EventManager.Instance.PostEvent("User/NavDestination");
		}
	}

	private void UpdateParticleSystemPositions()
	{
		Vector3 navIndicatorPosition = _brush.GetNavIndicatorPosition();
		Camera currentCamera = GameScene.player.GetComponent<PlayerInput>().currentCamera;
		if ((bool)worldSpaceParticles)
		{
			worldSpaceParticles.transform.position = navIndicatorPosition + -currentCamera.transform.forward * _worldOffsetDist;
			Quaternion rotation = new Quaternion(0f, 0f, 0f, 0f);
			Vector3 vector = _brush.transform.TransformDirection(_brush.normal);
			Vector3 lhs = new Vector3(0f, 0f, 1f);
			if (Mathf.Abs(Vector3.Dot(lhs, vector)) > 0.99f)
			{
				lhs = new Vector3(1f, 0f, 0f);
			}
			Vector3 view = Vector3.Cross(lhs, vector);
			view.Normalize();
			rotation.SetLookRotation(view, vector);
			worldSpaceParticles.transform.rotation = rotation;
		}
		if ((bool)panSpaceParticles)
		{
			Vector3 position = currentCamera.transform.InverseTransformPoint(navIndicatorPosition);
			position.z = currentCamera.nearClipPlane + 1f;
			Vector3 position2 = currentCamera.transform.TransformPoint(position);
			RenderDebug.DrawSphere(position2, Color.red, 0.5f, 1f);
			RenderDebug.DrawSphere(GameScene.panSpaceTransform.position, Color.green, 0.5f, 1f);
			panSpaceRoot.position = position2;
		}
	}

	public void Clear()
	{
		if ((bool)worldSpaceParticles)
		{
			worldSpaceParticles.Clear();
		}
		if ((bool)panSpaceParticles)
		{
			panSpaceParticles.Clear();
		}
	}

	private void LateUpdate()
	{
		if ((bool)_brush)
		{
			UpdateParticleSystemPositions();
		}
	}
}
