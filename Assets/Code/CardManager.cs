using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public sealed class CardManager : MonoBehaviour
{
	[SerializeField] private Sprite[] cardSprites;

	private Card[] cards = new Card[5];
	private List<Card> allowedCards = new List<Card>();

	private Image currentCardImage;

	private void Awake()
	{
		CreateCards();
	}

	private void Start()
	{
		currentCardImage = SceneItems.GetItem<Image>("DisplayedCard");
		EventManager.StartListening("DrawCard", DrawCard);
		EventManager.StartListening("CardToggled", ToggleCard);
	}

	public void ToggleCard(Data data)
	{
		Card card = cards[data.num];
		card.allowed = !card.allowed;
	}

	public void ReplaceAllowedList()
	{
		allowedCards.Clear();

		for (int i = 0; i < cards.Length; i++)
		{
			Card card = cards[i];

			if (card.allowed)
				allowedCards.Add(card);
		}
	}

	public void DrawCard(Data data)
	{
		Entity entity = data.entity;

		if (allowedCards.Count == 0) return;

		entity.wait = true;

		Card card = allowedCards[Random.Range(0, allowedCards.Count)];
		
		currentCardImage.sprite = card.sprite;
		currentCardImage.enabled = true;

		StartCoroutine(PlayCard(card, entity));
	}

	private IEnumerator PlayCard(Card card, Entity entity)
	{
		yield return new WaitForSeconds(2.0f);

		currentCardImage.enabled = false;
		entity.wait = false;

		if (StateManager.CurrentState == GameState.Playing)
		{
			if (entity != null)
				card.RunFunction(entity);
		}
	}

	private void CreateCards()
	{
		cards[0] = new ExtraMP(cardSprites[0]);
		cards[1] = new SwapCard(cardSprites[1]);
		cards[2] = new ExtraRandomMP(cardSprites[2]);
		cards[3] = new SkipTurnCard(cardSprites[3]);
		cards[4] = new LoseMPCard(cardSprites[4]);

		for (int i = 0; i < cards.Length; i++)
			allowedCards.Add(cards[i]);
	}
}
