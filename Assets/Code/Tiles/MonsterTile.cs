using UnityEngine;

public class MonsterTile : OverlayTile 
{
	public MonsterTile(ushort ID)
	{
		name = "Monster";
		tileID = ID;

		material = Resources.Load<Material>("TileMaterials/Monster");
		meshIndex = material.GetInt("_ID");
	}
}
