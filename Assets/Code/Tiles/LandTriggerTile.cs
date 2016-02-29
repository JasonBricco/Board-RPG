using UnityEngine;

public class LandTriggerTile : TileType 
{
	public LandTriggerTile()
	{
		name = "Trigger (L)";
		tileID = 6;
		meshIndex = 5;
		layer = 1;
	}

	public override void OnFunction(Vector2i pos)
	{
		Engine.CommandProcessor.LoadEditor(pos);
	}

	public override void OnEnter(int tX, int tY, Entity entity)
	{
		if (entity.RemainingMoves == 0)
			Engine.CommandProcessor.Process(tX, tY, entity);
	}

	public override void OnDeleted(BoardData data, Vector2i pos)
	{
		data.triggerData.Remove(pos);
	}
}
