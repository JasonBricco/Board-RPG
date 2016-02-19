//
//  Enums.cs
//  BoardRPG
//
//  Created by Jason Bricco on 2/18/16.
//  Copyright © 2016 Jason Bricco. All rights reserved.
//

public enum GameState 
{ 
	Playing,
	Editing,
	Window
}

public enum Direction
{
	Left,
	Right,
	Up,
	Down
}

public enum CommandError
{
	None,
	InvalidArgCount,
	InvalidArgType,
	InvalidArgValue
}
