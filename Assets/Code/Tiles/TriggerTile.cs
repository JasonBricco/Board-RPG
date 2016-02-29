using UnityEngine;

public class TriggerTile : TileType 
{
	public TriggerTile(ushort ID, int mesh)
	{
		name = "Trigger (F)";
		tileID = ID;
		meshIndex = mesh;
		layer = 1;
	}

	public override void OnFunction(Vector2i pos)
	{
		Engine.CommandProcessor.LoadEditor(pos);
	}

	public override void OnEnter(int tX, int tY, Entity entity)
	{
		Engine.CommandProcessor.Process(tX, tY, entity);
	}

	public override void OnDeleted(BoardData data, Vector2i pos)
	{
		data.triggerData.Remove(pos);
	}
}
