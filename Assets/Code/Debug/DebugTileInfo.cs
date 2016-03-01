using UnityEngine;
using UnityEngine.UI;

public class DebugTileInfo : MonoBehaviour 
{
	[SerializeField] private BoardManager boardManager;
	[SerializeField] private Text posLabel;

	private void StateChangedHandler(int state)
	{
		switch ((GameState)state)
		{
		case GameState.Editing:
			EnableLabels();
			break;

		case GameState.Playing:
			DisableLabels();
			break;
		}
	}

	private void Update()
	{
		if (StateManager.CurrentState != GameState.Window && Input.GetKeyDown(KeyCode.Tab))
			ToggleLabels();

		if (StateManager.CurrentState != GameState.Editing) return;

		Vector2i pos = boardManager.GetCursorTilePos();
		posLabel.text = "Position: " + pos.x + ", " + pos.y;
	}

	private void EnableLabels()
	{
		posLabel.enabled = true;
	}

	private void DisableLabels()
	{
		posLabel.enabled = false;
	}

	private void ToggleLabels()
	{
		posLabel.enabled = !posLabel.enabled;
	}
}
