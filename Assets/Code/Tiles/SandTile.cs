using UnityEngine;

public class SandTile : TileType
{
	public SandTile(ushort ID)
	{
		name = "Sand";
		tileID = ID;

		material = Resources.Load<Material>("TileMaterials/Sand");
		meshIndex = material.GetInt("_ID");
	}
}
