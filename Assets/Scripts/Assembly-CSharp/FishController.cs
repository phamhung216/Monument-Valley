using UnityEngine;

public class FishController : MonoBehaviour
{
	private Vector3 _heading = new Vector3(1f, 0f, 0f);

	public float _speed = 1f;

	public float _circlingRate = 10f;

	public bool circleClockwise = true;

	public float wallAvoidDist = 2f;

	public float fishAvoidDist = 0.5f;

	public float circleWeighting = 0.5f;

	public float avoidWallWeighting = 0.01f;

	public float avoidFishWeighting = 0.03f;

	private FishPond _pond;

	private Vector3 _lastPos;

	private void Start()
	{
		Transform parent = base.gameObject.transform.parent;
		while (_pond == null && (bool)parent)
		{
			_pond = parent.gameObject.GetComponent<FishPond>();
			parent = parent.parent;
		}
		_heading = base.transform.forward;
		_speed *= Random.Range(0.9f, 1.1f);
	}

	private void Update()
	{
		if (_pond.gameObject.activeSelf)
		{
			UpdateHeading();
			Vector3 vector = _speed * Time.deltaTime * _heading;
			base.transform.position = base.transform.position + vector;
			base.transform.rotation = Quaternion.AngleAxis(57.29578f * Mathf.Atan2(_heading.x, _heading.z), Vector3.up);
			_lastPos = base.transform.position;
		}
	}

	private void UpdateHeading()
	{
		Vector3 zero = Vector3.zero;
		zero += circleWeighting * CalculateCircleVector();
		zero += avoidWallWeighting * CalculateAvoidWallsVector();
		zero += avoidFishWeighting * CalculateAvoidFishVector();
		zero.y = 0f;
		_heading = zero;
		_heading.Normalize();
	}

	private Vector3 CalculateCircleVector()
	{
		return Quaternion.AngleAxis(_circlingRate * Time.deltaTime, circleClockwise ? Vector3.up : Vector3.down) * _heading;
	}

	private Vector3 CalculateAvoidWallsVector()
	{
		Vector3 zero = Vector3.zero;
		Ray ray = new Ray(base.transform.position, base.transform.forward);
		FishPondWall[] walls = _pond.walls;
		foreach (FishPondWall fishPondWall in walls)
		{
			float distanceToPoint = fishPondWall.plane.GetDistanceToPoint(base.transform.position);
			float num = Vector3.Dot(fishPondWall.plane.normal, ray.direction);
			if (!(0f < distanceToPoint))
			{
				continue;
			}
			float num2 = 1f / (distanceToPoint / wallAvoidDist) - 1f;
			num2 *= 0.1f + 0.9f * Mathf.Clamp(-1f * num, 0f, 1f);
			if (!(num2 > 0f))
			{
				continue;
			}
			Vector3 point = base.transform.position - distanceToPoint * fishPondWall.plane.normal;
			float enter = 0f;
			if (!fishPondWall.plane.Raycast(ray, out enter))
			{
				continue;
			}
			Vector3 point2 = ray.GetPoint(enter);
			Vector3 vector = fishPondWall.transform.worldToLocalMatrix.MultiplyPoint(point2);
			if (!(Mathf.Abs(fishPondWall.transform.worldToLocalMatrix.MultiplyPoint(point).x) > 0.5f * fishPondWall.size.x + 1f))
			{
				if (fishPondWall.hasConvexCorners)
				{
					float num3 = 0.2f;
					float num4 = 1f - Mathf.Clamp((Mathf.Abs(vector.x) - (0.5f * fishPondWall.size.x + num3)) / num3, 0f, 1f);
					num2 *= num4;
				}
				circleClockwise = Vector3.Dot(Vector3.up, Vector3.Cross(fishPondWall.plane.normal, base.transform.forward)) < 0f;
				zero += num2 * fishPondWall.plane.normal;
			}
		}
		return zero;
	}

	private Vector3 CalculateAvoidFishVector()
	{
		Vector3 zero = Vector3.zero;
		FishController[] fish = _pond.fish;
		foreach (FishController fishController in fish)
		{
			if (!(fishController == this))
			{
				Vector3 lhs = fishController._lastPos - _lastPos;
				Vector3 vector = fishController._lastPos + 0.5f * fishController._heading - _lastPos;
				vector.y = 0f;
				if (!(Vector3.Dot(lhs, _heading) < 0f))
				{
					float num = 1f - vector.magnitude / fishAvoidDist;
					num = ((num < 0f) ? 0f : num);
					num = ((num > 1f) ? 1f : num);
					zero += -1f * num * vector;
				}
			}
		}
		return zero;
	}
}
