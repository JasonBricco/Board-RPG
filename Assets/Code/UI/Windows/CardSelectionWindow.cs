using UnityEngine;

public sealed class CardSelectionWindow : Window 
{
	public override void Initialize()
	{
		EventManager.StartListening("DoneSelectingCards", DoneHandler);
		EventManager.StartListening("CardToggled", CardToggled);
	}

	private void DoneHandler(int data)
	{
		map.ReplaceAllowedList();

		gameObject.SetActive(false);
		UIStore.GetGraphic("BoardSettings").SetActive(true);
	}

	private void CardToggled(int cardID)
	{
		map.ToggleCard(cardID);
	}
}
