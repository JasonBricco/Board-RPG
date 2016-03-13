using UnityEngine;

public class WaterTile : OverlayTile 
{
	private Material materialA, materialB;
	private int indexA, indexB;

	public WaterTile(ushort ID)
	{
		name = "Water";
		tileID = ID;

		materialA = Resources.Load<Material>("TileMaterials/Water");
		materialB = Resources.Load<Material>("TileMaterials/StoneWater");

		material = materialA;

		indexA = materialA.GetInt("_ID");
		indexB = materialB.GetInt("_ID");
	}

	public override void Build(Tile tile, int tX, int tY, MeshData data)
	{
		material = tile.Data > 0 ? materialB : materialA;
		meshIndex = tile.Data > 0 ? indexB : indexA;

		base.Build(tile, tX, tY, data);
	}

	public override void OnFunction(Vector2i pos)
	{
		ushort data = Map.GetTile(1, pos.x, pos.y).Data;
		Map.ChangeData(1, pos.x, pos.y, data == 0 ? (ushort)1 : (ushort)0);
		Map.RebuildChunks();
	}

	public override bool CanAdd(Vector2i pos)
	{
		return true;
	}

	public override bool IsPassable(int x, int y)
	{
		return Map.GetTile(1, x, y).Data != 0;
	}
}
