using UnityEngine;

public class TriggerTile : OverlayTile 
{
	private CommandProcessor processor;

	public TriggerTile(ushort ID, int mesh, Map manager, CommandProcessor processor) : base(manager)
	{
		this.processor = processor;
		name = "Trigger (F)";
		tileID = ID;
		meshIndex = mesh;
		layer = 1;
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
