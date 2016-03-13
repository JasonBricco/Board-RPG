using UnityEngine;

public sealed class EditModeWindow : Window 
{
	public override void Initialize()
	{
		enableKey = KeyCode.Alpha3;
		EventManager.StartListening("EditModeChanged", EditModeChanged);
		Engine.StartUpdating(this);
	}

	private void EditModeChanged(Data data)
	{
		SceneItems.GetItem<MapEditor>("Map").SetEditMode(data.num);
		gameObject.SetActive(false);
	}
}
