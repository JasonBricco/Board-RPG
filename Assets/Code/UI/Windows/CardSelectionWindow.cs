using UnityEngine;

public sealed class CardSelectionWindow : Window 
{
	public override void Initialize()
	{
		EventManager.StartListening("SelectingCards", SelectingCards);
		EventManager.StartListening("DoneSelectingCards", DoneHandler);
	}

	private void SelectingCards(Data data)
	{
		gameObject.SetActive(true);
	}

	private void DoneHandler(Data data)
	{
		SceneItems.GetItem<CardManager>("CardManager").ReplaceAllowedList();

		gameObject.SetActive(false);
		SceneItems.GetItem("BoardSettings").SetActive(true);
	}
}
