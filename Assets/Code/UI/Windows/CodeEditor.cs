using UnityEngine;

public sealed class CodeEditor : MonoBehaviour
{
	public void OnEnable()
	{
		StateManager.ChangeState(GameState.Window);
	}

	public void OnDisable()
	{
		StateManager.ChangeState(GameState.Editing);
	}
}
