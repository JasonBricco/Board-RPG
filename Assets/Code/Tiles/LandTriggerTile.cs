//
//  LandTriggerTile.cs
//  BoardRPG
//
//  Created by Jason Bricco on 2/21/16.
//  Copyright © 2016 Jason Bricco. All rights reserved.
//

using UnityEngine;

public class LandTriggerTile : Tile 
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

	public LandTriggerTile()
	{
		name = "Land Trigger";
		tileID = 6;
		meshIndex = 5;
		posIndex = 1;
	}

	public override void OnFunction()
	{
		Processor.LoadEditor();
	}

	public override void OnEnter(int tX, int tY, Entity entity)
	{
		if (entity.RemainingMoves == 0)
			Processor.Process(tX, tY, entity);
	}

	public override void OnDeleted(BoardData data, Vector2i pos)
	{
		data.triggerData.Remove(pos);
	}
}
