using UnityEngine;

public class StartTile : OverlayTile 
{
	public StartTile(ushort ID)
	{
		name = "Start";
		tileID = ID;
	
		material = Resources.Load<Material>("TileMaterials/Start");
		meshIndex = material.GetInt("_ID");
	}

	public override void OnDeleted(Vector2i pos)
	{
		if (StateManager.CurrentState == GameState.Playing)
			EventManager.Notify("RemoveStartTile", new Data(pos.ToVector3()));
	}
}
