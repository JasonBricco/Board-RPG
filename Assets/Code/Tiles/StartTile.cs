using UnityEngine;

public class StartTile : OverlayTile 
{
	public StartTile(ushort ID, int mesh, BoardManager manager) : base(manager)
	{
		name = "Start";
		tileID = ID;
		meshIndex = mesh;
		layer = 1;
	}

	public override void OnDeleted(Vector2i pos)
	{
		if (StateManager.CurrentState == GameState.Playing)
			boardManager.RemoveStartTile(pos);
	}
}
