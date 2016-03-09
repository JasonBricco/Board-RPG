using UnityEngine;

public sealed class BoardSettingsWindow : Window
{
	public override void Initialize()
	{
		enableKey = KeyCode.Alpha1;
		EventManager.StartListening("SelectingCards", SelectingCards);

		Engine.StartUpdating(this);
	}

	private void SelectingCards(Data data)
	{
		gameObject.SetActive(false);
		UIStore.GetGraphic("CardSelection").SetActive(true);
	}
}
