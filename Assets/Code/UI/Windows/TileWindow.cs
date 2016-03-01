using UnityEngine;

public sealed class TileWindow : Window
{
	public override void Initialize()
	{
		enableKey = KeyCode.Alpha2;
		EventManager.StartListening("TileButtonPressed", TileButtonPressed);

		Engine.StartUpdating(this);
	}

	private void TileButtonPressed(int data)
	{
		boardManager.SetActiveTile((ushort)data);
		gameObject.SetActive(false);
	}
}
