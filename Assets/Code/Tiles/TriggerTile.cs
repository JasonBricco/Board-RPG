//
//  ButtonTile.cs
//  BoardRPG
//
//  Created by Jason Bricco on 2/18/16.
//  Copyright © 2016 Jason Bricco. All rights reserved.
//

using UnityEngine;

public class TriggerTile : Tile 
{
	private CommandProcessor commandProcessor;

	private CommandProcessor Processor
	{
		get 
		{
			if (commandProcessor == null)
				commandProcessor = GameObject.FindWithTag("Engine").GetComponent<CommandProcessor>();

			return commandProcessor;
		}
	}

	public TriggerTile()
	{
		name = "Trigger";
		tileID = 5;
		meshIndex = 4;
		isOverlay = true;
	}

	public override void OnFunction()
	{
		Processor.LoadEditor();
	}

	public override void OnEnter(int tX, int tY, int entityID, int movesLeft)
	{
		Processor.Process(tX, tY, entityID, movesLeft);
	}

	public override void OnDeleted(BoardData data, Vector2i pos)
	{
		data.triggerData.Remove(pos);
	}
}
