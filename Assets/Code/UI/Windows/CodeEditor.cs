using UnityEngine;
using UnityEngine.UI;

public sealed class CodeEditor : InputField
{
	private bool moveToEnd;

	public void Load(string text)
	{
		this.text = text;
		ActivateInputField();
		Select();
	}

	protected override void OnEnable()
	{
		base.OnEnable();
		StateManager.ChangeState(GameState.Window);
	}

	protected override void OnDisable()
	{
		base.OnDisable();

		if (!Engine.IsQuitting)
			StateManager.ChangeState(GameState.Editing);
	}

	public override void OnSelect(UnityEngine.EventSystems.BaseEventData eventData)
	{
		base.OnSelect(eventData);
		moveToEnd = true;
	}
		
	protected override void LateUpdate()
	{
		base.LateUpdate();

		if (moveToEnd)
		{
			MoveTextEnd(false);
			moveToEnd = false;
		}
	}
}
