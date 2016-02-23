using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public sealed class CardLibrary : MonoBehaviour
{
	[SerializeField] private Sprite[] cardSprites;
	private Card[] cards = new Card[2];

	private Image currentCard;

	private void Start()
	{
		currentCard = UIStore.GetGraphic<Image>("Card");

		cards[0] = new ForwardFiveCard();
		cards[1] = new FlipCard();
	}

	public void DrawCard(Entity entity)
	{
		entity.Wait = true;

		int index = Random.Range(0, cards.Length);
		currentCard.sprite = cardSprites[index];
		currentCard.enabled = true;

		StartCoroutine(PlayCard(index, entity));
	}

	private IEnumerator PlayCard(int index, Entity entity)
	{
		yield return new WaitForSeconds(2.0f);
		currentCard.enabled = false;
		cards[index].RunFunction(entity);
		entity.Wait = false;
	}
}
