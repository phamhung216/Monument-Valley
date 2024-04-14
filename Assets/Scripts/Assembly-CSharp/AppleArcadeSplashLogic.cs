using System;
using UnityCommon;
using UnityEngine;

public class AppleArcadeSplashLogic : MonoBehaviour
{
	[SerializeField]
	private AppleArcadeVideo _appleArcadeVideo;

	private void Start()
	{
		if (_appleArcadeVideo != null)
		{
			AppleArcadeVideo appleArcadeVideo = _appleArcadeVideo;
			appleArcadeVideo.OnVideoCompleted = (Action)Delegate.Combine(appleArcadeVideo.OnVideoCompleted, new Action(VideoCompleted));
		}
	}

	private void VideoCompleted()
	{
		if (_appleArcadeVideo != null)
		{
			AppleArcadeVideo appleArcadeVideo = _appleArcadeVideo;
			appleArcadeVideo.OnVideoCompleted = (Action)Delegate.Remove(appleArcadeVideo.OnVideoCompleted, new Action(VideoCompleted));
		}
		StartCoroutine(GameStartLogic.StartLoadAfterAFrame());
	}
}
