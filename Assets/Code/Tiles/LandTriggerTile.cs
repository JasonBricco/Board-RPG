using UnityEngine;

public class LandTriggerTile : OverlayTile 
{
	private CommandProcessor processor;

	public LandTriggerTile(ushort ID, int mesh, BoardManager manager, CommandProcessor processor) : base(manager)
	{
		this.processor = processor;

		name = "Trigger (L)";
		tileID = ID;
		meshIndex = mesh;
	}

	public override void OnFunction(Vector2i pos)
	{
		processor.LoadEditor(pos);
	}

	public override void OnEnter(int tX, int tY, Entity entity)
	{
		if (entity.RemainingMoves == 0)
			processor.Process(tX, tY, entity);
	}

	public override void OnDeleted(Vector2i pos)
	{
		processor.DeleteCommands(pos);
	}
}
