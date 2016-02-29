using UnityEngine;

public class CardTile : TileType 
{
	public CardTile()
	{
		name = "Card";
		tileID = 7;
		meshIndex = 6;
		layer = 1;
	}

	public override void OnEnter(int tX, int tY, Entity entity)
	{
		if (entity.RemainingMoves == 0)
			Engine.CardLibrary.DrawCard(entity);
	}
}
