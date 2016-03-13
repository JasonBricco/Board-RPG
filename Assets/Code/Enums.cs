using UnityEngine;

public enum GameState 
{ 
	Playing,
	Editing,
	Window,
	SelectingCoords
}

public enum EntityType
{
	Player,
	Monster
}
	
public enum CameraMode 
{ 
	Free, 
	Follow 
}

public enum EditMode
{
	Normal,
	MassEdit,
	SquareFill,
	AreaFill
}
