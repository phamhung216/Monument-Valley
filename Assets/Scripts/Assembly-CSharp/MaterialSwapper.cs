using System.Collections;
using UnityEngine;

public class MaterialSwapper : MonoBehaviour
{
	public Renderer meshRenderer;

	public Material originalMaterial;

	public Material alternateMaterial;

	public float flashInterval = 0.1f;

	private bool _flashing;

	private float _flashTimer;

	private Material _swapMaterial;

	private void Start()
	{
		if (originalMaterial == null && (bool)meshRenderer)
		{
			originalMaterial = meshRenderer.material;
		}
	}

	[TriggerableAction]
	public IEnumerator SetAlternateMaterial()
	{
		meshRenderer.material = alternateMaterial;
		return null;
	}

	[TriggerableAction]
	public IEnumerator SetOriginalMaterial()
	{
		meshRenderer.material = originalMaterial;
		return null;
	}

	[TriggerableAction]
	public IEnumerator StartFlashing()
	{
		_flashing = true;
		_flashTimer = flashInterval;
		_swapMaterial = ((meshRenderer.material == originalMaterial) ? alternateMaterial : originalMaterial);
		return null;
	}

	[TriggerableAction]
	public IEnumerator StopFlashing()
	{
		_flashing = false;
		return null;
	}

	private void Update()
	{
		if (_flashing)
		{
			if (_flashTimer <= 0f)
			{
				meshRenderer.material = _swapMaterial;
				_swapMaterial = ((_swapMaterial == originalMaterial) ? alternateMaterial : originalMaterial);
				_flashTimer = flashInterval;
			}
			else
			{
				_flashTimer -= Time.deltaTime;
			}
		}
	}
}
