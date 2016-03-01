using UnityEngine;

public sealed class AirTile : TileType 
{
	public AirTile(ushort ID, BoardManager manager) : base(manager)
	{
		name = "Air";
		tileID = ID;
	}
}
