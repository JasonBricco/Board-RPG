using UnityEngine;

public class StoneTile : TileType
{
	public StoneTile(ushort ID)
	{
		name = "Stone";
		tileID = ID;

		material = Resources.Load<Material>("TileMaterials/Stone");
		meshIndex = material.GetInt("_ID");
	}
}
