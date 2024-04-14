using System;
using UnityEngine;

public class DropShadow : MonoBehaviour
{
	private MeshFilter mesh;

	private const int maxStairs = 7;

	private const int maxVerts = 14;

	private const float cameraOffset = 0.1f;

	private float halfShadowSize = 0.5f;

	private Vector3[] _verts;

	private Vector3[] _normals;

	private Color[] _color;

	private Vector3 _lastPosition = Vector3.zero;

	private Quaternion _lastRotation = Quaternion.identity;

	private bool _isInitialised;

	private BaseLocomotion _character;

	private void Start()
	{
		_character = base.transform.parent.GetComponent<BaseLocomotion>();
		_character.shadow = this;
		mesh = GetComponent<MeshFilter>();
		Vector3[] array = new Vector3[14];
		Vector2[] array2 = new Vector2[14];
		int[] array3 = new int[72];
		Color[] array4 = new Color[14];
		int num = 0;
		for (int i = 0; i < 14; i += 2)
		{
			float num2 = 6f;
			array[i] = new Vector3(0f - halfShadowSize, 0f, halfShadowSize * (float)i / num2 - halfShadowSize);
			array[i + 1] = new Vector3(halfShadowSize, 0f, halfShadowSize * (float)i / num2 - halfShadowSize);
			num2 = 13f;
			array2[i] = new Vector2(0f, (float)i / num2);
			array2[i + 1] = new Vector2(1f, (float)i / num2);
			array4[i] = new Color(1f, 1f, 1f, 1f);
			array4[i + 1] = new Color(1f, 1f, 1f, 1f);
			if (i + 2 < 14)
			{
				array3[num] = i;
				num++;
				array3[num] = i + 1;
				num++;
				array3[num] = i + 2;
				num++;
				array3[num] = i + 2;
				num++;
				array3[num] = i + 1;
				num++;
				array3[num] = i + 3;
				num++;
			}
		}
		mesh.mesh = new Mesh();
		mesh.mesh.vertices = array;
		mesh.mesh.triangles = array3;
		mesh.mesh.uv = array2;
		mesh.mesh.colors = array4;
		_verts = mesh.mesh.vertices;
		_color = mesh.mesh.colors;
		_normals = new Vector3[14];
		Color color = new Color(1f, 1f, 1f, _character.getShadowIntensity());
		for (int j = 0; j < 14; j += 2)
		{
			_color[j] = color;
			_color[j + 1] = color;
		}
		_isInitialised = false;
	}

	public void PosParentUpdate()
	{
		bool flag = _character is CharacterLocomotion;
		Transform transform = _character.transform;
		Transform transform2 = base.transform;
		bool flag2 = flag || !_isInitialised;
		_isInitialised = true;
		_lastPosition = transform.position;
		_lastRotation = transform.rotation;
		NavBrushComponent lastValidBrush = _character.lastValidBrush;
		NavBrushComponent targetBrush = _character.getTargetBrush();
		float interpT = _character.GetInterpT();
		if (flag2)
		{
			if (_character.lastValidBrush != _character.getTargetBrush())
			{
				float relHightAtNodePos = getRelHightAtNodePos(lastValidBrush, targetBrush, 0.5f * (interpT + 0.5f));
				for (int i = 0; i < 14; i += 2)
				{
					float num = halfShadowSize * (float)i / 6f - halfShadowSize;
					float relHightAtNodePos2 = getRelHightAtNodePos(lastValidBrush, targetBrush, 0.5f * (interpT + num + 0.5f));
					relHightAtNodePos2 -= relHightAtNodePos;
					_verts[i].y = relHightAtNodePos2;
					_verts[i + 1].y = relHightAtNodePos2;
				}
			}
			for (int j = 0; j < 14; j += 2)
			{
				Vector3 vector = new Vector3(0f, 0f, 0f);
				if (j > 0)
				{
					vector += Vector3.Cross(new Vector3(-1f, 0f, 0f), _verts[j] - _verts[j - 2]).normalized;
				}
				if (j < 12)
				{
					vector += Vector3.Cross(new Vector3(-1f, 0f, 0f), _verts[j + 2] - _verts[j]).normalized;
				}
				vector.Normalize();
				_normals[j] = vector;
				_normals[j + 1] = vector;
			}
		}
		if (_character.lastValidBrush != _character.getTargetBrush())
		{
			Vector3 vector2 = targetBrush.transform.position - _character.entryBoundary.transform.position;
			vector2.Normalize();
			float num2 = Vector3.Dot(vector2, _character.getShadowUp());
			vector2 -= num2 * _character.characterUp;
			if (vector2.magnitude > 0.1f)
			{
				vector2.Normalize();
				Quaternion rotation = new Quaternion(0f, 0f, 1f, 0f);
				rotation.SetLookRotation(vector2, _character.getShadowUp());
				transform2.rotation = rotation;
			}
		}
		Vector3 forward = Camera.main.transform.forward;
		transform2.position = _character.getShadowRootPosition() + forward * -0.1f;
		float shadowIntensity = _character.getShadowIntensity();
		bool flag3 = false;
		if (flag || _character.IsWalkingAroundCorner || _character.locoState == BaseLocomotion.LocomotionState.LocoCustomAnim)
		{
			Color color = new Color(1f, 1f, 1f, shadowIntensity);
			for (int k = 0; k < 14; k += 2)
			{
				float num3 = Vector3.Dot(transform2.TransformDirection(_normals[k]), -forward);
				float num4 = Mathf.Acos(1f - num3);
				float num5 = (float)Math.PI / 4f;
				num4 -= num5;
				num4 *= 5f;
				num4 = Mathf.Clamp(num4, 0f, 1f);
				Color color2 = color;
				color2.a *= num4;
				if (color2 != _color[k])
				{
					_color[k] = color2;
					_color[k + 1] = _color[k];
					flag3 = true;
				}
			}
		}
		if (flag2)
		{
			mesh.mesh.vertices = _verts;
		}
		if (flag3)
		{
			mesh.mesh.colors = _color;
		}
	}

	private Vector3 findConnectionBoundaryPos(NavBrushComponent sourceBrush, NavBrushComponent targetBrush)
	{
		NavBoundaryComponent[] boundaries = targetBrush.boundaries;
		foreach (NavBoundaryComponent navBoundaryComponent in boundaries)
		{
			if (!(navBoundaryComponent != null))
			{
				continue;
			}
			for (int j = 0; j < navBoundaryComponent.links.Count; j++)
			{
				NavBoundaryComponent otherBoundary = navBoundaryComponent.links[j].GetOtherBoundary(navBoundaryComponent);
				if (otherBoundary != null && otherBoundary.transform.parent.GetComponent<NavBrushComponent>() == sourceBrush)
				{
					return otherBoundary.transform.position;
				}
			}
		}
		return new Vector3(0f, 0f, 0f);
	}

	private float getRelHightAtNodePos(NavBrushComponent fromNode, NavBrushComponent toNode, float interpT)
	{
		BaseLocomotion component = base.transform.parent.GetComponent<BaseLocomotion>();
		NavBrushComponent navBrushComponent = fromNode;
		NavBrushComponent navBrushComponent2 = toNode;
		if (interpT < 0.5f)
		{
			navBrushComponent = fromNode;
			navBrushComponent2 = toNode;
		}
		else
		{
			navBrushComponent = toNode;
			navBrushComponent2 = fromNode;
		}
		if (navBrushComponent.normal.y == 0f)
		{
			_ = navBrushComponent2.normal.y;
			_ = 0f;
			return 0f;
		}
		if (navBrushComponent2.normal.y == 0f)
		{
			return 0f;
		}
		if (navBrushComponent.normal.y > 0f && navBrushComponent.normal.y < 0.9f)
		{
			if (navBrushComponent2.normal.y > 0f && navBrushComponent2.normal.y < 0.9f)
			{
				float num = Vector3.Dot(findConnectionBoundaryPos(fromNode, toNode), component.characterUp);
				float num2 = Vector3.Dot(findConnectionBoundaryPos(toNode, fromNode), component.characterUp);
				float num3 = num - Vector3.Dot(fromNode.transform.position, component.characterUp);
				float num4 = num2 - Vector3.Dot(toNode.transform.position, component.characterUp);
				num4 -= num3;
				return num3 + (num4 - num3) * 1f * (1f - interpT);
			}
			float num5 = Vector3.Dot(findConnectionBoundaryPos(navBrushComponent, navBrushComponent2), component.characterUp);
			float num6 = num5 - Vector3.Dot(navBrushComponent.transform.position, component.characterUp);
			float num7 = 0f - (num5 - Vector3.Dot(navBrushComponent.transform.position, component.characterUp));
			if (interpT > 0.5f)
			{
				return num6 + (num7 - num6) * 2f * (interpT - 0.5f);
			}
			return num6 + (num7 - num6) * 2f * (0.5f - interpT);
		}
		if (navBrushComponent2.normal.y > 0f && navBrushComponent2.normal.y < 0.9f)
		{
			return Vector3.Dot(findConnectionBoundaryPos(navBrushComponent2, navBrushComponent), component.characterUp) - Vector3.Dot(navBrushComponent2.transform.position, component.characterUp);
		}
		return 0f;
	}
}
