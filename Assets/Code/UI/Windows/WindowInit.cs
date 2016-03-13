using UnityEngine;

public class WindowInit : MonoBehaviour 
{
	private void Start() 
	{
		SceneItems.GetItem<TileWindow>("TilePanel").Initialize();
		SceneItems.GetItem<BoardSettingsWindow>("BoardSettings").Initialize();
		SceneItems.GetItem<CardSelectionWindow>("CardSelection").Initialize();
		SceneItems.GetItem<EditModeWindow>("EditModePanel").Initialize();
	}
}
