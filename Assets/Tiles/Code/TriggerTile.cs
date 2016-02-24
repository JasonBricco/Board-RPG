using UnityEngine;

public class TriggerTile : Tile 
{
	private CommandProcessor commandProcessor;

	private CommandProcessor Processor
	{
		get 
		{
			if (commandProcessor == null)
				commandProcessor = Engine.Instance.GetComponent<CommandProcessor>();

			return commandProcessor;
		}
	}

	public TriggerTile()
	{
		name = "Trigger (F)";
		tileID = 5;
		meshIndex = 4;
		posIndex = 1;
	}

	public override void OnFunction()
	{
		Processor.LoadEditor();
	}

	public override void OnEnter(int tX, int tY, Entity entity)
	{
		Processor.Process(tX, tY, entity);
	}

	public override void OnDeleted(BoardData data, Vector2i pos)
	{
		data.triggerData.Remove(pos);
	}
}
