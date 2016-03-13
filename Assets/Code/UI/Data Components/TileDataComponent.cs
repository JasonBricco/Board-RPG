using UnityEngine;
using UnityEngine.UI;

public sealed class TileDataComponent : MonoBehaviour 
{
	private int ID;

	private void Start()
	{
		ID = Map.GetTileType(name).ID;
		GetComponent<Button>().onClick.AddListener(SendTilePressedEvent);
	}

	private void SendTilePressedEvent()
	{
		EventManager.Notify("TileButtonPressed", new Data(ID));
	}
}
