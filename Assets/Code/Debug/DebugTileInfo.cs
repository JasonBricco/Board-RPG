using UnityEngine;
using UnityEngine.UI;

public class DebugTileInfo : MonoBehaviour 
{
	private BoardEditor boardEditor;
	private BoardManager boardManager;

	[SerializeField] private Text nameLabel;
	[SerializeField] private Text posLabel;

	private void Awake()
	{
		GameObject engine = GameObject.FindWithTag("Engine");
		boardEditor = engine.GetComponent<BoardEditor>();
		boardManager = engine.GetComponent<BoardManager>();
	}

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
		if (Input.GetKeyDown(KeyCode.Tab))
			ToggleLabels();

		if (StateManager.CurrentState != GameState.Editing) return;

		Vector2i pos = boardEditor.GetCursorTilePos();
		Tile tile = boardManager.GetTileSafe(0, pos.x, pos.y);

		nameLabel.text = "Tile: " + tile.Name;
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
