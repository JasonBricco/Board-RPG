using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public sealed class CardLibrary : MonoBehaviour
{
	[SerializeField] private Sprite[] cardSprites;
	private Card[] cards = new Card[7];

	private List<Card> allowedCards = new List<Card>();

	private Image currentImage;

	private void Start()
	{
		currentImage = UIStore.GetGraphic<Image>("Card");

		cards[0] = new ForwardFiveCard(cardSprites[0]);
		cards[1] = new FlipCard(cardSprites[1]);
		cards[2] = new SwapCard(cardSprites[2]);
		cards[3] = new ExtraRollCard(cardSprites[3]);
		cards[4] = new DoubleRollsCard(cardSprites[4]);
		cards[5] = new HalfRollsCard(cardSprites[5]);
		cards[6] = new SkipTurnCard(cardSprites[6]);

		for (int i = 0; i < cards.Length; i++)
			allowedCards.Add(cards[i]);
	}

	public void ToggleCard(int cardID)
	{
		Card card = cards[cardID];
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

	public void DrawCard(Entity entity)
	{
		if (allowedCards.Count == 0) return;

		entity.Wait = true;

		Card card = allowedCards[Random.Range(0, allowedCards.Count)];
		currentImage.sprite = card.sprite;
		currentImage.enabled = true;

		StartCoroutine(PlayCard(card, entity));
	}

	private IEnumerator PlayCard(Card card, Entity entity)
	{
		yield return new WaitForSeconds(2.0f);

		if (StateManager.CurrentState == GameState.Playing)
		{
			currentImage.enabled = false;
			card.RunFunction(entity);
			entity.Wait = false;
		}
	}
}
