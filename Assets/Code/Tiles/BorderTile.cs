using UnityEngine;

public class BorderTile : OverlayTile 
{
	private ushort orientation = 0;
	private int dataIndex = 0;

	private int primaryMeshIndex;
	private int secondaryMeshIndex;

	private ushort[] dataOrder = { 0, 4, 1, 5, 2, 6, 3, 7 };

	public BorderTile(ushort ID, int mesh0, int mesh1, Map manager) : base(manager)
	{
		name = "Border";
		tileID = ID;

		primaryMeshIndex = mesh0;
		secondaryMeshIndex = mesh1;
		meshIndex = primaryMeshIndex;
	}

	public override void Build(Tile tile, int tX, int tY, MeshData data)
	{
		meshIndex = tile.Data > 3 ? secondaryMeshIndex : primaryMeshIndex;
		base.Build(tile, tX, tY, data);
	}

	public override void OnFunction(Vector2i pos)
	{
		dataIndex = (dataIndex + 1) & 7;
		orientation = dataOrder[dataIndex];

		boardManager.SetTileFast(pos, new Tile(tileID, orientation));

		boardManager.FlagChunkForRebuild(pos);
		boardManager.RebuildChunks();
	}

	public override Tile Preprocess(Tile tile, Vector2i pos)
	{
		tile.Data = orientation;
		return tile;
	}

	public override bool IsPassable(int layer, Tile main, Tile overlay)
	{
		return false;
	}

	public override bool CanAdd(Vector2i pos)
	{
		return true;
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
