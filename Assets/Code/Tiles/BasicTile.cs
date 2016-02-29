using UnityEngine;

public class BasicTile : TileType
{
	public BasicTile(ushort ID, int mesh, string name)
	{
		this.name = name;
		this.tileID = ID;
		this.meshIndex = mesh;
	}
}
