using UnityEngine;

public sealed class StartTile : TileType 
{
	public StartTile(ushort ID, int mesh)
	{
		name = "Start";
		tileID = ID;
		meshIndex = mesh;
		layer = 1;
	}

	public override bool CanAdd(BoardData data, Vector2i pos)
	{
		return data.startTiles.Count < 20;
	}

	public override void OnAdded(BoardData data, Vector2i pos)
	{
		if (!data.startTiles.Contains(pos))
			data.startTiles.Add(pos);
	}

	public override void OnDeleted(BoardData data, Vector2i pos)
	{
		data.startTiles.Remove(pos);
	}
}
