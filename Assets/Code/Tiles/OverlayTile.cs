using UnityEngine;

public class OverlayTile : TileType 
{
	public OverlayTile() : base()
	{
		layer = 1;
	}

	public override bool CanAdd(Vector2i pos)
	{
		if (Map.GetTile(0, pos.x, pos.y).Equals(Tiles.Air))
			return false;

		return true;
	}

	public override bool IsPassable(int x, int y)
	{
		return Map.GetTileType(0, x, y).IsPassable(x, y);
	}
}
