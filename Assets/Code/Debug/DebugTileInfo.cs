using UnityEngine;
using UnityEngine.UI;

public class DebugTileInfo : MonoBehaviour 
{
	[SerializeField] private Text posLabel;

	private void Awake()
	{
		EventManager.StartListening("StateChanged", StateChangedHandler);
	}

	private void StateChangedHandler(Data data)
	{
		switch (data.state)
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
		GameState state = StateManager.CurrentState;

		if (state != GameState.Editing && state != GameState.SelectingCoords) return;

		if (state != GameState.Window && Input.GetKeyDown(KeyCode.Tab))
			ToggleLabels();

		Vector2i pos = Utils.GetCursorTilePos();
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
