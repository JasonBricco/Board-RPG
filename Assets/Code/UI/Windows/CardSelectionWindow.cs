using UnityEngine;

public sealed class CardSelectionWindow : Window 
{
	private CardManager cardManager;

	public override void Initialize()
	{
		cardManager = GameObject.FindWithTag("Engine").GetComponent<CardManager>();

		EventManager.StartListening("SelectingCards", SelectingCards);
		EventManager.StartListening("DoneSelectingCards", DoneHandler);
		EventManager.StartListening("CardToggled", CardToggled);
	}

	private void SelectingCards(Data data)
	{
		gameObject.SetActive(true);
	}

	private void DoneHandler(Data data)
	{
		cardManager.ReplaceAllowedList();

		gameObject.SetActive(false);
		UIStore.GetGraphic("BoardSettings").SetActive(true);
	}

	private void CardToggled(Data data)
	{
		cardManager.ToggleCard(data.num);
	}
}
