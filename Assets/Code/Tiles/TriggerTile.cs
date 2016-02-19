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
	public TriggerTile()
	{
		name = "Trigger";
		tileID = 5;
		meshIndex = 4;
		isOverlay = true;
	}

	public override void OnFunction()
	{
		EventManager.TriggerEvent("LoadCode");
	}
}
