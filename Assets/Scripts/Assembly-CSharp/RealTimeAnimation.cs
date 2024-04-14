using UnityEngine;

public class RealTimeAnimation : MonoBehaviour
{
	private float _lastRealtimeSinceStartup;

	private float _deltaTime;

	private bool _isPlaying;

	private float _accumulatedTime;

	private AnimationState _animationState;

	[HideInInspector]
	public AnimationClip animationClip;

	private Animation _animation;

	public bool isPlaying => _isPlaying;

	private void Start()
	{
		_isPlaying = false;
		_animation = GetComponent<Animation>();
	}

	private void Update()
	{
		if (_isPlaying)
		{
			_deltaTime = Time.realtimeSinceStartup - _lastRealtimeSinceStartup;
			_lastRealtimeSinceStartup = Time.realtimeSinceStartup;
			_accumulatedTime += _deltaTime;
			float a = _accumulatedTime / _animationState.length;
			a = Mathf.Min(a, 1f);
			_animationState.normalizedTime = a;
			GetComponent<Animation>().Sample();
			if (a >= 1f)
			{
				_isPlaying = false;
			}
		}
	}

	public void Play()
	{
		if (_animation != null)
		{
			_animation.clip = animationClip;
			_animation.Play();
			_animationState = _animation[animationClip.name];
			_ = _animationState.length;
			_animation.Sample();
			if (!_isPlaying)
			{
				_isPlaying = true;
				_lastRealtimeSinceStartup = Time.realtimeSinceStartup;
			}
			_deltaTime = Time.realtimeSinceStartup - _lastRealtimeSinceStartup;
			_lastRealtimeSinceStartup = Time.realtimeSinceStartup;
			_accumulatedTime = 0f;
		}
	}
}
