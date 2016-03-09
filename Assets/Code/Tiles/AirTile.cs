using UnityEngine;

public sealed class AirTile : TileType 
{
	public AirTile(ushort ID)
	{
		name = "Air";
		tileID = ID;
	}

	public override bool IsPassable(int x, int y)
	{
		Tile main = Map.GetTile(0, x, y); 

		if (main.Equals(Tiles.Air))
			return false;
		
		return Map.GetTileType(main).IsPassable(x, y);
	}
}
