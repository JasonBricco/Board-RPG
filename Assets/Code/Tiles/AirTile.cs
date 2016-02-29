using UnityEngine;

public sealed class AirTile : TileType 
{
	public AirTile(ushort ID)
	{
		name = "Air";
		tileID = ID;
	}
}
