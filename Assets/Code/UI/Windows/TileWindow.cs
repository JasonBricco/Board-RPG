using UnityEngine;

public sealed class TileWindow : Window
{
	private MapEditor mapEditor;

	public override void Initialize()
	{
		mapEditor = SceneItems.GetItem<MapEditor>("Map");

		enableKey = KeyCode.Alpha2;
		EventManager.StartListening("TileButtonPressed", TileButtonPressed);

		Engine.StartUpdating(this);
	}

	private void TileButtonPressed(Data data)
	{
		mapEditor.SetActiveTile((ushort)data.num);
		gameObject.SetActive(false);
	}
}
