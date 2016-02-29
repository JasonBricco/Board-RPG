using UnityEngine;
using System;

[Serializable]
public struct Tile
{
	[SerializeField] private ushort tileID;
	[SerializeField] private ushort data;

	public Tile(ushort ID)
	{
		this.tileID = ID;
		this.data = 0;
	}

	public Tile(ushort ID, ushort data)
	{
		this.tileID = ID;
		this.data = data;
	}

	public ushort ID
	{
		get { return tileID; }
	}

	public ushort Data
	{
		get { return data; }
	}

	public TileType Type
	{
		get { return TileStore.GetTileByID(ID); }
	}
}
