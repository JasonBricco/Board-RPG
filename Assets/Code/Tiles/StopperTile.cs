using UnityEngine;

public class StopperTile : TileType
{
	public StopperTile(ushort ID)
	{
		name = "Stopper";
		tileID = ID;

		material = Resources.Load<Material>("TileMaterials/Stopper");
		meshIndex = material.GetInt("_ID");
	}
}
