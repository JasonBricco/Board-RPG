using UnityEngine;

public class WindowInit : MonoBehaviour 
{
	private void Start() 
	{
		UIStore.GetGraphic<TileWindow>("TilePanel").Initialize();
		UIStore.GetGraphic<BoardSettingsWindow>("BoardSettings").Initialize();
		UIStore.GetGraphic<CardSelectionWindow>("CardSelection").Initialize();
	}
}
