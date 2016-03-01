using UnityEngine;

public class BasicTile : TileType
{
	public BasicTile(ushort ID, int mesh, int layer, string name)
	{
		this.name = name;
		this.tileID = ID;
		this.meshIndex = mesh;
		this.layer = layer;
	}
}
