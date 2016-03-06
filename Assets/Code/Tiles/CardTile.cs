using UnityEngine;

public class CardTile : OverlayTile 
{
	public CardTile(ushort ID, int mesh, Map manager) : base(manager)
	{
		name = "Card";
		tileID = ID;
		meshIndex = mesh;
	}

	public override void OnEnter(int tX, int tY, Entity entity)
	{
		if (entity.remainingMoves == 0)
			boardManager.DrawCard(entity);
	}
}
