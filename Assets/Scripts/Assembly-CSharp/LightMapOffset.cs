using System.Collections.Generic;
using UnityEngine;

public class LightMapOffset : MonoBehaviour
{
	public struct OriginalPosition
	{
		public Vector3 worldPos;

		public Vector3 localPos;

		public OriginalPosition(GameObject obj)
		{
			worldPos = obj.transform.position;
			localPos = obj.transform.localPosition;
		}
	}

	public float spacing = 100f;

	public List<GameObject> gameObjectsArray = new List<GameObject>();

	public List<OriginalPosition> originalPositions = new List<OriginalPosition>();

	private bool moveButtonPressed;

	private void Start()
	{
	}

	private void Update()
	{
	}

	public void MoveGameObjects()
	{
		if (moveButtonPressed)
		{
			return;
		}
		moveButtonPressed = true;
		if (gameObjectsArray.Count > 0)
		{
			originalPositions.Clear();
			for (int i = 0; i < gameObjectsArray.Count; i++)
			{
				GameObject obj = gameObjectsArray[i];
				originalPositions.Add(new OriginalPosition(obj));
			}
			for (int j = 0; j < gameObjectsArray.Count; j++)
			{
				GameObject obj2 = gameObjectsArray[j];
				Vector3 worldPos = originalPositions[j].worldPos;
				float num = 1000f;
				float num2 = 1800f;
				float num3 = (float)(j + 1) * spacing;
				int num4 = Mathf.FloorToInt(num3 / (num2 - num));
				num3 = num + num3 % (num2 - num);
				float newZ = (float)num4 * spacing;
				worldPos.Set(num3, 0f, newZ);
				obj2.transform.position = worldPos;
			}
		}
	}

	public void RestoreGameObjectPositions()
	{
		if (gameObjectsArray.Count > 0 && originalPositions.Count > 0)
		{
			for (int i = 0; i < gameObjectsArray.Count; i++)
			{
				gameObjectsArray[i].transform.localPosition = originalPositions[i].localPos;
			}
		}
		moveButtonPressed = false;
	}
}
