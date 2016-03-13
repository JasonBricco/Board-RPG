using UnityEngine;

public class OverlayTile : TileType 
{
	public OverlayTile() : base()
	{
		layer = 1;
	}

	public override bool CanAdd(Vector2i pos)
	{
		Tile tile = Map.GetTile(0, pos.x, pos.y);

		if (tile.Equals(Tiles.Air) || tile.Equals(Tiles.Border))
			return false;

		return true;
	}

	public override bool IsPassable(int x, int y)
	{
		return Map.GetTileType(0, x, y).IsPassable(x, y);
	}
}
