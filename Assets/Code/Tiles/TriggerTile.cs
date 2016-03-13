using UnityEngine;

public class TriggerTile : OverlayTile 
{
	private CommandProcessor processor;

	public TriggerTile(ushort ID)
	{
		processor = SceneItems.GetItem<CommandProcessor>("CommandProcessor");

		name = "Trigger (F)";
		tileID = ID;

		material = Resources.Load<Material>("TileMaterials/Trigger");
		meshIndex = material.GetInt("_ID");
	}

	public override void OnFunction(Vector2i pos)
	{
		processor.LoadEditor(pos);
	}

	public override void OnEnter(int tX, int tY, Entity entity)
	{
		processor.Process(tX, tY, entity);
	}

	public override void OnDeleted(Vector2i pos)
	{
		processor.DeleteCommands(pos);
	}
}
