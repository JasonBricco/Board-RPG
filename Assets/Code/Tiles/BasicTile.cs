using UnityEngine;

public class BasicTile : TileType
{
	public BasicTile(ushort ID, string name, int mesh)
	{
		this.name = name;
		this.tileID = ID;
		this.meshIndex = mesh;
	}
}
