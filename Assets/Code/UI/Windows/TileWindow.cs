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
		map.SetActiveTile((ushort)data);
		gameObject.SetActive(false);
	}
}
