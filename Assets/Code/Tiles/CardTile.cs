using UnityEngine;

public class CardTile : TileType 
{
	public CardTile(ushort ID, int mesh)
	{
		name = "Card";
		tileID = ID;
		meshIndex = mesh;
		layer = 1;
	}

	public override void OnEnter(int tX, int tY, Entity entity)
	{
		if (entity.RemainingMoves == 0)
			Engine.CardLibrary.DrawCard(entity);
	}
}
