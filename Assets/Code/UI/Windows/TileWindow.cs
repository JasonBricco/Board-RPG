using UnityEngine;

public sealed class TileWindow : MonoBehaviour, IUpdatable
{
	private BoardEditor boardEditor;

	private void Start()
	{
		boardEditor = Engine.Instance.GetComponent<BoardEditor>();

		Engine.StartUpdating(this);
		gameObject.SetActive(false);

		EventManager.StartListening("TileButtonPressed", TileButtonPressed);
	}

	public void OnEnable()
	{
		StateManager.ChangeState(GameState.Window);
	}

	public void OnDisable()
	{
		StateManager.ChangeState(GameState.Editing);
	}

	private void TileButtonPressed(int data)
	{
		boardEditor.SetActiveTile(TileStore.GetTileByID(data));
		gameObject.SetActive(false);
	}

	public void UpdateFrame()
	{
		if (Input.GetKeyDown(KeyCode.Alpha2))
		{
			if (StateManager.CurrentState == GameState.Window)
			{
				if (gameObject.activeSelf)
					gameObject.SetActive(false);
			}
			else
				gameObject.SetActive(true);
		}
	}
}
