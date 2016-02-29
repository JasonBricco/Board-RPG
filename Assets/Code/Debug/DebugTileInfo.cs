using UnityEngine;
using UnityEngine.UI;

public class DebugTileInfo : MonoBehaviour 
{
	[SerializeField] private Text nameLabel;
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

		Vector2i pos = Engine.BoardEditor.GetCursorTilePos();
		Tile tile = Engine.BoardManager.GetTileSafe(0, pos.x, pos.y);

		nameLabel.text = "Tile: " + tile.Type.Name;
		posLabel.text = "Position: " + pos.x + ", " + pos.y;
	}

	private void EnableLabels()
	{
		nameLabel.enabled = true;
		posLabel.enabled = true;
	}

	private void DisableLabels()
	{
		nameLabel.enabled = false;
		posLabel.enabled = false;
	}

	private void ToggleLabels()
	{
		nameLabel.enabled = !nameLabel.enabled;
		posLabel.enabled = !posLabel.enabled;
	}
}
