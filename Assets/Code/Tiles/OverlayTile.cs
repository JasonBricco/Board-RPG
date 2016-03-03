using UnityEngine;

public class OverlayTile : TileType 
{
	public OverlayTile(BoardManager manager) : base(manager)
	{
		layer = 1;
	}

	public override bool CanAdd(Vector2i pos)
	{
		if (boardManager.GetTile(0, pos.x, pos.y).Equals(Tiles.Air))
			return false;

		return true;
	}
}
