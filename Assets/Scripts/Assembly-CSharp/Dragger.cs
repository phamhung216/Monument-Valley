using UnityEngine;

public abstract class Dragger : GameTouchable, IHoverable
{
	protected bool _snapping;

	protected bool _dragging;

	private float _glowAmount;

	public MaterialInstantiator glowingMatInstr;

	public MaterialInstantiator[] extraGlowers;

	public string customBlendParam;

	private Draggable _targetDraggable;

	public override bool showTouchIndicator
	{
		get
		{
			if (base.showTouchIndicator)
			{
				return _dragging;
			}
			return false;
		}
	}

	public abstract GameObject targetObject { get; }

	public virtual Draggable targetDraggable
	{
		get
		{
			if (_targetDraggable == null)
			{
				_targetDraggable = targetObject.GetComponent<Draggable>();
			}
			return _targetDraggable;
		}
	}

	public bool dragging => _dragging;

	public bool snapping => _snapping;

	public Dragger()
	{
		claimOnTouchBegan = true;
		claimOnTouchNotTap = true;
		releaseOnTouchNotTap = false;
	}

	public void GlowDecrease()
	{
		if (_glowAmount != 0f)
		{
			_glowAmount -= Time.deltaTime * 2f;
			if (_glowAmount < 0f)
			{
				_glowAmount = 0f;
			}
			CheckBlendString();
			SetMaterialGlowValues(_glowAmount);
		}
	}

	private void SetMaterialGlowValues(float glowAmount)
	{
		if ((bool)glowingMatInstr)
		{
			glowingMatInstr.instantiatedMaterial.SetFloat(customBlendParam, glowAmount);
		}
		if (extraGlowers != null && extraGlowers.Length != 0)
		{
			for (int i = 0; i < extraGlowers.Length; i++)
			{
				extraGlowers[i].instantiatedMaterial.SetFloat(customBlendParam, glowAmount);
			}
		}
	}

	public void OnHover()
	{
		if (!(_glowAmount >= 0.5f))
		{
			GlowHalf();
		}
	}

	public void OnHoverEnd()
	{
	}

	public void GlowHalf()
	{
		SetGlow(0.5f);
	}

	public void GlowFull()
	{
		SetGlow(1f);
	}

	private void SetGlow(float value)
	{
		if (_glowAmount != value)
		{
			_glowAmount = value;
			CheckBlendString();
			SetMaterialGlowValues(_glowAmount);
		}
	}

	private void CheckBlendString()
	{
		if (string.IsNullOrEmpty(customBlendParam))
		{
			customBlendParam = "_Blend";
		}
	}

	public override void OnTouchBegan(GameTouch touch)
	{
		base.OnTouchBegan(touch);
		StartDrag(touch.initialPos);
		if (touch.initialPos != touch.pos)
		{
			Drag(touch.pos, touch.pos - touch.initialPos);
		}
	}

	public override void OnTouchMoved(GameTouch touch)
	{
		Drag(touch.pos, touch.pos - touch.lastPos);
	}

	public override void OnTouchEnded(GameTouch touch)
	{
		base.OnTouchEnded(touch);
		if (dragging)
		{
			Snap();
		}
	}

	public override void OnTouchCancelled(GameTouch touch)
	{
		base.OnTouchCancelled(touch);
		if (dragging)
		{
			CancelDrag();
		}
	}

	public virtual void CancelDrag()
	{
		Snap();
	}

	public virtual void StartDrag(Vector3 position)
	{
		_dragging = true;
		_snapping = false;
		if (this as Rotator == null)
		{
			GameScene.navManager.NotifyReconfigurationBegan(targetObject);
		}
		if ((bool)targetDraggable)
		{
			targetDraggable.StartDrag();
		}
	}

	public virtual void Drag(Vector3 position, Vector3 delta)
	{
	}

	public virtual void Snap()
	{
		_dragging = false;
		_snapping = true;
		if ((bool)targetDraggable)
		{
			targetDraggable.Snap();
		}
	}

	public virtual void EndSnapping()
	{
		_snapping = false;
		GameScene.navManager.NotifyReconfigurationEnded();
		if ((bool)targetDraggable)
		{
			targetDraggable.EndSnapping();
		}
	}
}
