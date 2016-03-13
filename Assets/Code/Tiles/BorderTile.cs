using UnityEngine;

public class BorderTile : TileType 
{
	private ushort orientation = 0;
	private int dataIndex = 0;

	private Material materialA, materialB;
	private int indexA, indexB;

	private ushort[] dataOrder = { 0, 4, 1, 5, 2, 6, 3, 7 };

	public BorderTile(ushort ID)
	{
		name = "Border";
		tileID = ID;

		materialA = Resources.Load<Material>("TileMaterials/BorderSide");
		materialB = Resources.Load<Material>("TileMaterials/BorderCorner");

		material = materialA;

		indexA = materialA.GetInt("_ID");
		indexB = materialB.GetInt("_ID");
	}

	public override void Build(Tile tile, int tX, int tY, MeshData data)
	{
		material = tile.Data > 3 ? materialB : materialA;
		meshIndex = tile.Data > 3 ? indexB : indexA;

		base.Build(tile, tX, tY, data);
	}

	public override void OnFunction(Vector2i pos)
	{
		dataIndex = (dataIndex + 1) & 7;
		orientation = dataOrder[dataIndex];

		Map.ChangeData(0, pos.x, pos.y, orientation);
		Map.RebuildChunks();
	}

	public override Tile Preprocess(Tile tile, Vector2i pos)
	{
		tile.Data = orientation;
		return tile;
	}

	public override bool IsPassable(int x, int y)
	{
		return false;
	}

	public override void SetUVs(Tile tile, MeshData data)
	{
		switch (tile.Data)
		{
		case 3:
		case 7:
			data.AddUV(meshIndex, new Vector2(1.0f, 1.0f));
			data.AddUV(meshIndex, new Vector2(0.0f, 1.0f));
			data.AddUV(meshIndex, new Vector2(0.0f, 0.0f));
			data.AddUV(meshIndex, new Vector2(1.0f, 0.0f));
			break;

		case 0:
		case 4:
			data.AddUV(meshIndex, new Vector2(0.0f, 1.0f));
			data.AddUV(meshIndex, new Vector2(0.0f, 0.0f));
			data.AddUV(meshIndex, new Vector2(1.0f, 0.0f));
			data.AddUV(meshIndex, new Vector2(1.0f, 1.0f));
			break;

		case 1:
		case 5:
			data.AddUV(meshIndex, new Vector2(0.0f, 0.0f));
			data.AddUV(meshIndex, new Vector2(1.0f, 0.0f));
			data.AddUV(meshIndex, new Vector2(1.0f, 1.0f));
			data.AddUV(meshIndex, new Vector2(0.0f, 1.0f));
			break;

		default:
			base.SetUVs(tile, data);
			break;
		}
	}
}
