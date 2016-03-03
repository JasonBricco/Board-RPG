using UnityEngine;

public class WaterTile : OverlayTile 
{
	private int primaryMeshIndex;
	private int secondaryMeshIndex;

	public WaterTile(ushort ID, int mesh0, int mesh1, BoardManager manager) : base(manager)
	{
		name = "Water";
		tileID = ID;

		primaryMeshIndex = mesh0;
		secondaryMeshIndex = mesh1;
		meshIndex = primaryMeshIndex;
	}

	public override void Build(Tile tile, int tX, int tY, MeshData data)
	{
		meshIndex = tile.Data > 0 ? secondaryMeshIndex : primaryMeshIndex;
		base.Build(tile, tX, tY, data);
	}

	public override void OnFunction(Vector2i pos)
	{
		ushort data = boardManager.GetTile(1, pos.x, pos.y).Data;
		Tile newTile = new Tile(tileID, data == 0 ? (ushort)1 : (ushort)0);

		boardManager.SetTileFast(pos, newTile);
		boardManager.FlagChunkForRebuild(pos);
		boardManager.RebuildChunks();
	}

	public override bool CanAdd(Vector2i pos)
	{
		return true;
	}

	public override bool IsPassable(int layer, Tile main, Tile overlay)
	{
		return overlay.Data != 0;
	}
}
