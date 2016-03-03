using UnityEngine;

public sealed class AirTile : TileType 
{
	public AirTile(ushort ID, BoardManager manager) : base(manager)
	{
		name = "Air";
		tileID = ID;
	}

	public override bool IsPassable(int layer, Tile main, Tile overlay)
	{
		if (layer == 0)
		{
			if (overlay.Equals(Tiles.Air))
				return false;
			
			return boardManager.GetTileType(overlay).IsPassable(1, main, overlay);
		}
		else
			return true;
	}
}
