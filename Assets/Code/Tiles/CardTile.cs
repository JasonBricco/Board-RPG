using UnityEngine;

public class CardTile : OverlayTile 
{
	public CardTile(ushort ID)
	{
		name = "Card";
		tileID = ID;

		material = Resources.Load<Material>("TileMaterials/Card");
		meshIndex = material.GetInt("_ID");
	}

	public override void OnEnter(int tX, int tY, Entity entity)
	{
		if (entity.TargetMP == 0)
			EventManager.Notify("DrawCard", new Data(entity));
	}
}
