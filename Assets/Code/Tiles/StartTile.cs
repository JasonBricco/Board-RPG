//
//  StartTile.cs
//  BoardRPG
//
//  Created by Jason Bricco on 2/5/16.
//  Copyright © 2016 Jason Bricco. All rights reserved.
//

public sealed class StartTile : Tile 
{
	public StartTile()
	{
		name = "Start";
		tileID = 2;
		meshIndex = 1;
		isOverlay = true;
	}
}
