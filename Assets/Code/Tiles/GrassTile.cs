using UnityEngine;

public class GrassTile : TileType
{
	public GrassTile(ushort ID)
	{
		name = "Grass";
		tileID = ID;

		material = Resources.Load<Material>("TileMaterials/Grass");
		meshIndex = material.GetInt("_ID");
	}
}
