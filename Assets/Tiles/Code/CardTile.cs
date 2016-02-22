using UnityEngine;

public class CardTile : Tile 
{
	private CardLibrary cardLibrary;

	private CardLibrary CardLibrary
	{
		get 
		{
			if (cardLibrary == null)
				cardLibrary = Engine.Instance.GetComponent<CardLibrary>();

			return cardLibrary;
		}
	}

	public CardTile()
	{
		name = "Card";
		tileID = 7;
		meshIndex = 6;
		posIndex = 1;
	}

	public override void OnEnter(int tX, int tY, Entity entity)
	{
		if (entity.RemainingMoves == 0)
			CardLibrary.DrawCard(entity);
	}
}
