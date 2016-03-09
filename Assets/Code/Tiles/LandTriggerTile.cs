using UnityEngine;

public class LandTriggerTile : OverlayTile 
{
	private CommandProcessor processor;

	public LandTriggerTile(ushort ID)
	{
		processor = GameObject.FindWithTag("Engine").GetComponent<CommandProcessor>();

		name = "Trigger (L)";
		tileID = ID;

		material = Resources.Load<Material>("TileMaterials/LandTrigger");
		meshIndex = material.GetInt("_ID");
	}

	public override void OnFunction(Vector2i pos)
	{
		processor.LoadEditor(pos);
	}

	public override void OnEnter(int tX, int tY, Entity entity)
	{
		if (entity.remainingMoves == 0)
			processor.Process(tX, tY, entity);
	}

	public override void OnDeleted(Vector2i pos)
	{
		processor.DeleteCommands(pos);
	}
}
